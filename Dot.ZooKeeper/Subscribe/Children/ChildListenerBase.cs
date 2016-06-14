using System;
using System.Collections.Generic;

namespace Dot.ZooKeeper.Subscribe
{
    public abstract class ChildListenerBase : IChildListener
    {
        public string GroupPath { get; protected set; }

        public ChildListenerBase(string groupPath)
        {
            this.GroupPath = groupPath;
        }

        public abstract void OnChildrenChanged(List<string> children);
    }
}