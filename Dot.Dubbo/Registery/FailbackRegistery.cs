using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dot.Extension;
using Dot.ServiceModel;

namespace Dot.Dubbo.Registery
{
    public abstract class FailbackRegistery : RegisteryBase
    {
        protected Task _retry;
        protected CancellationTokenSource _retryCancellation = new CancellationTokenSource();
        protected ManualResetEventSlim _retryCompleteEvent = new ManualResetEventSlim(false);

        protected ConcurrentDictionary<string, ServiceMetadata> _failedRegistered = new ConcurrentDictionary<string, ServiceMetadata>();
        protected ConcurrentDictionary<string, ServiceMetadata> _failedUnregistered = new ConcurrentDictionary<string, ServiceMetadata>();
        protected ConcurrentDictionary<string, List<INotifyListener>> _failedSubscribed = new ConcurrentDictionary<string, List<INotifyListener>>();
        protected ConcurrentDictionary<string, List<INotifyListener>> _failedUnsubscribed = new ConcurrentDictionary<string, List<INotifyListener>>();
        protected ConcurrentDictionary<string, ConcurrentDictionary<INotifyListener, List<ServiceMetadata>>> _failedNotified = new ConcurrentDictionary<string, ConcurrentDictionary<INotifyListener, List<ServiceMetadata>>>();

        public FailbackRegistery()
        {
            this.InitRetryTask();
        }
        protected virtual void InitRetryTask()
        {
            _retry = Task.Factory.StartNew(() =>
            {
                while (_retryCancellation.IsCancellationRequested == false)
                {
                    try
                    {
                        this.Retry();
                    }
                    catch // 防御性容错
                    {
                        // todo: log
                    }
                    Thread.Sleep(3000); // todo: configuration
                }
                _retryCompleteEvent.Set();
            });
        }

        public override void Register(ServiceMetadata metadata, bool checkOnStart)
        {
            base.Register(metadata, checkOnStart);
            _failedRegistered.TryRemove(metadata.FullPath);
            _failedUnregistered.TryRemove(metadata.FullPath);

            try
            {
                this.DoRegister(metadata);
            }
            catch (Exception ex)
            {
                // 如果开启了启动时检测，直接抛出异常
                if (checkOnStart) throw ex;
                // 将失败的注册请求记录到失败列表，定时重试
                _failedRegistered.TryAdd(metadata.FullPath, metadata);
            }
        }
        public override void Unregister(ServiceMetadata metadata, bool checkOnStart)
        {
            base.Unregister(metadata, checkOnStart);
            _failedRegistered.TryRemove(metadata.FullPath);
            _failedUnregistered.TryRemove(metadata.FullPath);

            try
            {
                this.DoUnregister(metadata);
            }
            catch (Exception ex)
            {
                // 如果开启了启动时检测，直接抛出异常
                if (checkOnStart) throw ex;
                // 将失败的取消注册请求记录到失败列表，定时重试
                _failedUnregistered.TryAdd(metadata.FullPath, metadata);
            }
        }
        public override void Subscribe(string groupPath, INotifyListener listener, bool checkOnStart)
        {
            base.Subscribe(groupPath, listener, checkOnStart);
            var emptyListeners = new List<INotifyListener>();
            _failedSubscribed.GetOrAdd(groupPath, emptyListeners).Remove(listener);
            _failedUnsubscribed.GetOrAdd(groupPath, emptyListeners).Remove(listener);

            try
            {
                this.DoSubscribe(groupPath, listener);
            }
            catch
            {
                // 如果开启了启动时检测，直接抛出异常
                if (checkOnStart) throw;
                // 将失败的订阅请求记录到失败列表，定时重试
                _failedSubscribed.GetOrAdd(groupPath, emptyListeners).Add(listener);
            }
        }
        public override void Unsubscribe(string groupPath, INotifyListener listener, bool checkOnStart)
        {
            base.Unsubscribe(groupPath, listener, checkOnStart);
            var emptyListeners = new List<INotifyListener>();
            _failedSubscribed.GetOrAdd(groupPath, emptyListeners).Remove(listener);
            _failedUnsubscribed.GetOrAdd(groupPath, emptyListeners).Remove(listener);

            try
            {
                this.DoUnsubscribe(groupPath, listener);
            }
            catch (Exception ex)
            {
                // 如果开启了启动时检测，直接抛出异常
                if (checkOnStart) throw ex;
                // 将失败的订阅请求记录到失败列表，定时重试
                _failedUnsubscribed.GetOrAdd(groupPath, emptyListeners).Add(listener);
            }
        }
        protected override void Notify(string groupPath, INotifyListener listener, List<ServiceMetadata> metadatas)
        {
            try
            {
                base.Notify(groupPath, listener, metadatas);
            }
            catch
            {
                _failedNotified.GetOrAdd(groupPath, new ConcurrentDictionary<INotifyListener, List<ServiceMetadata>>())
                               .AddOrReplace(listener, metadatas);
            }
        }
        protected virtual void Retry()
        {
            _failedRegistered.Values.ForEach(meta => this.Register(meta, false));

            _failedUnregistered.Values.ForEach(meta => this.Unregister(meta, false));

            _failedSubscribed.ForEach(kv =>
            {
                var groupPath = kv.Key;
                var listeners = kv.Value;
                listeners.ForEach(listener => this.Subscribe(groupPath, listener, false));
            });

            _failedUnsubscribed.ForEach(kv =>
            {
                var groupPath = kv.Key;
                var listeners = kv.Value;
                listeners.ForEach(listener => this.Unsubscribe(groupPath, listener, false));
            });

            _failedNotified.ForEach(kv =>
            {
                var groupPath = kv.Key;
                var listeners = kv.Value;
                foreach (var keyValue in listeners)
                {
                    var listener = keyValue.Key;
                    var metadatas = keyValue.Value;
                    this.Notify(groupPath, listener, metadatas);
                }
            });
        }

        protected abstract void DoRegister(ServiceMetadata metadata);
        protected abstract void DoUnregister(ServiceMetadata metadata);
        protected abstract void DoSubscribe(string groupPath, INotifyListener listener);
        protected abstract void DoUnsubscribe(string groupPath, INotifyListener listener);
    }
}