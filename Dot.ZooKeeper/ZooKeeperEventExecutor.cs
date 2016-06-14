using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Dot.Threading.Atomic;

namespace Dot.ZooKeeper
{
    internal class ZooKeeperEventExecutor
    {
        private BlockingCollection<ZooKeeperEvent> _eventQueue;
        private AtomicInteger _eventId;
        private AtomicBoolean _shutdown = new AtomicBoolean(true);
        private AutoResetEvent _shutdownCompleted = new AutoResetEvent(false);

        public ZooKeeperEventExecutor(bool autoStart = true)
        {
            if (autoStart)
                this.Start();
        }

        public void Add(ZooKeeperEvent @event)
        {
            if (_shutdown.Value == false)
            {
                try { _eventQueue.TryAdd(@event); }
                catch { }
            }
        }

        public void Start()
        {
            if (_shutdown.CompareAndSet(true, false))
            {
                _eventId = new AtomicInteger(0);
                _eventQueue = new BlockingCollection<ZooKeeperEvent>();

                Task.Factory.StartNew(() =>
                {
                    var events = _eventQueue.GetConsumingEnumerable();
                    foreach (var @event in events)
                    {
                        var eventId = _eventId.Increment();
                        @event.Description = string.Format("Delivering event #{0} {1}", eventId, @event.Description);
                        @event.Run();
                    }

                    _shutdownCompleted.Set();
                });
            }
        }

        public void Shutdown()
        {
            if (_shutdown.CompareAndSet(false, true))
            {
                _eventQueue.CompleteAdding();
                _shutdownCompleted.WaitOne();
                _eventQueue.Dispose();
            }
        }
    }
}