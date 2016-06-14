using System;
using System.Collections.Generic;
using Dot.ZooKeeper.Subscribe;

namespace Dot.ZooKeeper.Sample.Support
{
    public class DebugChildrenSubscriber : ChildListenerBase
    {
        public DebugChildrenSubscriber(string dir)
            : base(dir)
        {
        }

        public override void OnChildrenChanged(List<string> children)
        {
            Console.WriteLine("[DebugChildrenSubscriber.OnChildrenChanged]({0})", string.Join(",", children));
            Console.WriteLine("--------------------------");
        }
    }
}