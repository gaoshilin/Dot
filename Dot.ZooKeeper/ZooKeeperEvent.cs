using System;

namespace Dot.ZooKeeper
{
    internal class ZooKeeperEvent
    {
        public string Description { get; set; }
        public Action<ZooKeeperEvent> Handle { get; set; }

        public ZooKeeperEvent(string description, Action<ZooKeeperEvent> handle)
        {
            this.Description = description;
            this.Handle = handle;
        }

        public void Run()
        {
            this.Handle(this);
        }

        public override string ToString()
        {
            return string.Format("ZooKeeperEvent[{0}]", this.Description);
        }
    }
}