using System;
using System.Linq;
using Dot.Extension;
using ZooKeeperNet;

namespace Dot.ZooKeeper
{
    public static class ZooKeeperClientExtension
    {
        public static string Create(this ZooKeeperClient client, string path)
        {
            var data = new byte[0];
            return client.Create(path, data, CreateMode.Persistent);
        }

        public static string Create(this ZooKeeperClient client, string path, CreateMode mode)
        {
            var data = new byte[0];
            return client.Create(path, data, mode);
        }

        public static void EnsurePath(this ZooKeeperClient client, string path)
        {
            if (client.Exists(path, false) == false)
                client.Create(path);
        }

        public static void EnsurePath(this ZooKeeperClient client, string path, CreateMode mode)
        {
            if (client.Exists(path, false) == false)
                client.Create(path, mode);
        }

        public static void EnsurePathRecursive(this ZooKeeperClient client, string path, bool ignoreLast = true)
        {
            var chunks = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            var parentLevel = ignoreLast ? chunks.Length - 1 : chunks.Length;
            var parentPaths = Enumerable.Range(1, parentLevel).Select(len => "/" + string.Join("/", chunks.Left(len)));
            parentPaths.ForEach(p => client.Create(p));
        }

        public static byte[] GetData(this ZooKeeperClient client, string path)
        {
            return client.GetData(path, false, null);
        }
    }
}