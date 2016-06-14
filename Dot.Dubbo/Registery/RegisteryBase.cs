using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Dot.Extension;
using Dot.ServiceModel;
using Dot.Threading.Atomic;

namespace Dot.Dubbo.Registery
{
    public abstract class RegisteryBase : IRegistery
    {
        protected static readonly string ROOT_PATH = "/dotdubbo";
        protected ConcurrentDictionary<string, ServiceMetadata> _registered;
        protected ConcurrentDictionary<string, List<INotifyListener>> _subscribed;
        protected ConcurrentDictionary<string, List<ServiceMetadata>> _notified;

        public RegisteryBase()
        {
            _registered = new ConcurrentDictionary<string, ServiceMetadata>();
            _subscribed = new ConcurrentDictionary<string, List<INotifyListener>>();
            _notified = new ConcurrentDictionary<string, List<ServiceMetadata>>();
        }

        public virtual void Register(ServiceMetadata metadata, bool checkOnStart)
        {
            if (metadata == null)
                throw new ArgumentNullException("metadata", "register metadata == null");

            _registered.TryAdd(metadata.FullPath, metadata);
        }
        public virtual void Unregister(ServiceMetadata metadata, bool checkOnStart)
        {
            if (metadata == null)
                throw new ArgumentNullException("metadata", "register metadata == null");

            _registered.TryRemove(metadata.FullPath);
        }
        public virtual void Subscribe(string groupPath, INotifyListener listener, bool checkOnStart)
        {
            if (listener == null)
                throw new ArgumentNullException("listener", "notify listener == null");

            if (string.IsNullOrEmpty(groupPath))
                throw new ArgumentNullException("groupPath", "groupPath is null or empty");

            _subscribed.GetOrAdd(groupPath, new List<INotifyListener>()).Add(listener);
        }
        public virtual void Unsubscribe(string groupPath, INotifyListener listener, bool checkOnStart)
        {
            if (string.IsNullOrEmpty(groupPath) || listener == null)
                return;

            List<INotifyListener> listeners;
            if (_subscribed.TryGetValue(groupPath, out listeners))
                listeners.TryRemove(listener);
        }
        public List<ServiceMetadata> Lookup(string groupPath)
        {
            if (string.IsNullOrEmpty(groupPath))
                throw new ArgumentNullException("groupPath", "groupPath is null or empty");

            var metadatas = new List<ServiceMetadata>();
            if (_notified.TryGetValue(groupPath, out metadatas) == false || metadatas.Any() == false)
            {
                var reference = new AtomicReference<List<ServiceMetadata>>(null);
                var listener = new NotifyListener();
                listener.OnMetadataChanged += (metas) => reference.CompareAndSet(null, metas);
                this.Subscribe(groupPath, listener, true); // 订阅逻辑需要保证 Notify 后再返回
                metadatas = reference.Value;
            }

            return metadatas;
        }

        protected void Notify(string rootPath)
        {
            var groupPaths = this.GetGroupPaths(rootPath);
            foreach (var groupPath in groupPaths)
            {
                var metadatas = this.GetServiceMetadatas(groupPath);
                this.Notify(groupPath, metadatas);
            }
        }
        protected void Notify(string groupPath, List<ServiceMetadata> metadatas)
        {
            List<INotifyListener> listeners;
            if (_subscribed.TryGetValue(groupPath, out listeners))
                listeners.ForEach(listener => this.Notify(groupPath, listener, metadatas));

            _notified.AddOrReplace(groupPath, metadatas);
        }
        protected virtual void Notify(string groupPath, INotifyListener listener, List<ServiceMetadata> metadatas)
        {
            listener.Notify(metadatas);
        }
        protected abstract List<string> GetGroupPaths(string rootPath);
        protected abstract List<ServiceMetadata> GetServiceMetadatas(string groupPath);
        protected virtual void Recover()
        {
            // 重新注册
            var recoverRegistered = new HashSet<ServiceMetadata>(_registered.Values);
            recoverRegistered.ForEach(metadata => this.Register(metadata, false));

            // 重新订阅
            var recoverSubscribed = new Dictionary<string, List<INotifyListener>>(_subscribed);
            recoverSubscribed.ForEach(kv =>
            {
                var groupPath = kv.Key;
                var listeners = kv.Value;
                listeners.ForEach(listener => this.Subscribe(groupPath, listener, false));
            });
        }
    }
}