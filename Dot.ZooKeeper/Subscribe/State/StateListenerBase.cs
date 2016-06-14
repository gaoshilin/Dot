using ZooKeeperNet;

namespace Dot.ZooKeeper.Subscribe
{
    public abstract class StateListenerBase : IStateListener
    {
        public abstract void OnStateChanged(KeeperState state);
        public abstract void OnNewSession();
    }
}