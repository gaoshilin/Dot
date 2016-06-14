using System;

namespace Dot.ZooKeeper.Subscribe
{
    public abstract class DataListenerBase : IDataListener
    {
        public string ServicePath { get; protected set; }

        public DataListenerBase(string servicePath)
        {
            this.ServicePath = servicePath;
        }

        public abstract void OnDataChanged(string servicePath, byte[] data);
    }
}