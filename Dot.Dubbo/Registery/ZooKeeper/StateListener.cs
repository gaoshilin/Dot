using Dot.ZooKeeper.Subscribe;
using ZooKeeperNet;

namespace Dot.Dubbo.Registery.ZooKeeper
{
    public class StateListener : StateListenerBase
    {
        public delegate void OnConnectedHandler();
        public event OnConnectedHandler OnConnected;

        public delegate void OnDisconnectedHandler();
        public event OnDisconnectedHandler OnDisconnected;

        public delegate void OnReconnectedHandler();
        public event OnReconnectedHandler OnReconnected;

        public override void OnStateChanged(KeeperState state)
        {
            System.Console.WriteLine("Zookeeper connection state changed to {0}", state);
            if (state == KeeperState.Disconnected)
                this.DisconnectedHandle();
            else if (state == KeeperState.SyncConnected)
                this.ConnectedHandle();
        }

        public override void OnNewSession()
        {
            this.ReconnectedHandle();
        }

        private void ConnectedHandle()
        {
            if (this.OnConnected != null)
                this.OnConnected();
        }

        private void DisconnectedHandle()
        {
            if (this.OnDisconnected != null)
                this.OnDisconnected();
        }

        private void ReconnectedHandle()
        {
            if (this.OnReconnected != null)
                this.OnReconnected();
        }
    }
}