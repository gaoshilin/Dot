using System;

namespace Dot.ZooKeeper.Unsubscribe
{
    public class Unsubscriber<T> : IUnsubscriber<T>
    {
        private Action _unsubscribe;
        public T Data { get; set; }

        public Unsubscriber(Action unsubscribe = null)
        {
            _unsubscribe = unsubscribe;
        }

        public void Dispose()
        {
            if (_unsubscribe != null)
                _unsubscribe();
        }
    }
}