using ZooKeeperNet;

namespace Dot.ZooKeeper.Subscribe
{
    public interface IStateListener
    {
        void OnStateChanged(KeeperState state);

        void OnNewSession();
    }
}