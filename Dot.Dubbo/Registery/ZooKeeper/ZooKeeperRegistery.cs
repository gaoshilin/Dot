using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dot.Extension;
using Dot.ServiceModel;
using Dot.ZooKeeper;
using Dot.ZooKeeper.Unsubscribe;
using ZooKeeperNet;

namespace Dot.Dubbo.Registery.ZooKeeper
{
    public class ZooKeeperRegistery : FailbackRegistery
    {
        private ZooKeeperClient _zkClient;
        private StateListener _zkStateListener;
        private ConcurrentDictionary<string, ConcurrentDictionary<INotifyListener, ChildListener>> _zkListeners;

        public bool IsAvailable 
        { 
            get { return _zkClient.IsConnected; } 
        }
        
        public ZooKeeperRegistery(ZooKeeperClient zkClient)
        {
            _zkListeners = new ConcurrentDictionary<string,ConcurrentDictionary<INotifyListener,ChildListener>>();

            _zkStateListener = new StateListener();
            _zkStateListener.OnReconnected += this.Recover;

            _zkClient = zkClient;
            _zkClient.EnsurePath(ROOT_PATH);
            _zkClient.SubscribeStateListener(_zkStateListener);
        }

        protected override void DoRegister(ServiceMetadata metadata)
        {
            try
            {
                _zkClient.EnsurePathRecursive(metadata.FullPath, true);
                _zkClient.CreateOrReplace(metadata.FullPath, metadata.ToBytes(), CreateMode.Ephemeral);
            }
            catch (Exception ex)
            {
                var message = string.Format("Fail to register metadata[{0}] to zookeeper, cause : {1}", metadata, ex.Message);
                throw new Exception(message, ex);
            }
        }
        protected override void DoUnregister(ServiceMetadata metadata)
        {
            try
            {
                _zkClient.Delete(metadata.FullPath);
            }
            catch (Exception ex)
            {
                var message = string.Format("Fail to unregister metadata[{0}] to zookeeper, cause : {1}", metadata, ex.Message);
                throw new Exception(message, ex);
            }
        }
        protected override void DoSubscribe(string groupPath, INotifyListener listener)
        {
            _zkClient.EnsurePathRecursive(groupPath, false);

            ConcurrentDictionary<INotifyListener, ChildListener> listeners;
            if (_zkListeners.TryGetValue(groupPath, out listeners) == false)
                listeners = _zkListeners.GetOrAdd(groupPath, new ConcurrentDictionary<INotifyListener, ChildListener>());

            ChildListener childListener;
            if (listeners.TryGetValue(listener, out childListener) == false)
            {
                childListener = new ChildListener(_zkClient, groupPath);
                childListener.OnMetadataChanged += (metas) => this.Notify(groupPath, listener, metas);
                listeners.TryAdd(listener, childListener);
            }

            var children = _zkClient.SubscribeChildListener(childListener);
            var metadatas = children.Select(child => _zkClient.GetData(groupPath + "/" + child, false, null).ToMetadata()).ToList();
            this.Notify(groupPath, listener, metadatas);
        }
        protected override void DoUnsubscribe(string groupPath, INotifyListener listener)
        {
            ConcurrentDictionary<INotifyListener, ChildListener> listeners;
            if (_zkListeners.TryGetValue(groupPath, out listeners))
            {
                ChildListener childListener;
                if (listeners.TryGetValue(listener, out childListener))
                {
                    _zkClient.UnsubscribeChildListener(childListener);
                    listeners.TryRemove(listener);
                }
            }
        }

        protected override List<string> GetGroupPaths(string rootPath)
        {
            return _zkClient.GetChildren(rootPath, false).ToList();
        }
        protected override List<ServiceMetadata> GetServiceMetadatas(string groupPath)
        {
            return _zkClient.GetChildren(groupPath, false)
                            .Select(child => _zkClient.GetData(child, false, null).ToMetadata())
                            .ToList();
        }
    }
}