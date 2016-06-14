using System;

namespace Dot.ZooKeeper.Unsubscribe
{
    public interface IUnsubscriber<T> : IDisposable
    {
        T Data { get; set; }
    }
}