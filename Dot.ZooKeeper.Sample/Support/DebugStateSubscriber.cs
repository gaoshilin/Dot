using System;
using Dot.ZooKeeper.Subscribe;
using ZooKeeperNet;

namespace Dot.ZooKeeper.Sample.Support
{
    public class DebugStateSubscriber : StateListenerBase
    {
        public override void OnStateChanged(KeeperState state)
        {
            Console.WriteLine("DebugStateListener.OnStateChanged current state = {0}", state);
        }

        public override void OnNewSession()
        {
            Console.WriteLine("DebugStateListener.OnNewSession");
        }
    }
}