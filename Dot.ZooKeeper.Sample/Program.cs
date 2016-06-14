using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dot.ZooKeeper;
using Dot.ZooKeeper.Sample.Support;

namespace DotZooKeeperSample
{
    class Program
    {
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            var client = GetClient();
            SubscribeChildrenListener(client);

            Console.ReadKey();
        }

        static ZooKeeperClient GetClient()
        {
            // 连接到 ZooKeeper 服务器，如果集群离线，将产生异常
            var client = ZooKeeperClient.Instance;

            // 监听连接状态变化，可以通过关闭 or 恢复集群，观察控制台窗口的输出
            client.SubscribeStateListener(new DebugStateSubscriber());

            return client;
        }

        static void SubscribeChildrenListener(ZooKeeperClient client)
        {
            var rootPath = "/cluster/calc";
            client.CreatePersistent(rootPath, new byte[0]);

            var childrenListener = new DebugChildrenSubscriber(rootPath);
            client.SubscribeChildListener(childrenListener);

            // create child
            for (int i = 11; i <= 15; i++)
            {
                client.CreatePersistent("/cluster/calc/" + i, new byte[0]);
                Thread.Sleep(1000);
            }

            // delete child
            for (int i = 11; i <= 15; i++)
            {
                client.Delete("/cluster/calc/" + i);
                Thread.Sleep(1000);
            }
        }
    }
}