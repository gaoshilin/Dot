using System;
using System.Text;
using Dot.ZooKeeper.Subscribe;

namespace Dot.ZooKeeper.Sample.Support
{
    public class DebugDataSubscriber : DataListenerBase
    {
        public DebugDataSubscriber(string servicePath)
            : base(servicePath)
        {
        }

        public override void OnDataChanged(string servicePath, byte[] data)
        {
            Console.WriteLine("[DebugDataListener.OnDataChanged] node of path {0} changed. node value = {1}", servicePath, Encoding.UTF8.GetString(data));
        }
    }
}