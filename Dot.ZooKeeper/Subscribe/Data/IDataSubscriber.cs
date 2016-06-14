using System;

namespace Dot.ZooKeeper.Subscribe
{
    public interface IDataListener
    {
        string ServicePath { get; }

        void OnDataChanged(string servicePath, byte[] data);
    }
}