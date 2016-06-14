using System.Collections.Generic;

namespace Dot.ZooKeeper.Subscribe
{
    public interface IChildListener
    {
        string GroupPath { get; }

        void OnChildrenChanged(List<string> children);
    }
}