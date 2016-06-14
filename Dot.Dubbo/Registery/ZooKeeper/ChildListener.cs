using System.Collections.Generic;
using System.Linq;
using Dot.ServiceModel;
using Dot.ZooKeeper;
using Dot.ZooKeeper.Subscribe;

namespace Dot.Dubbo.Registery.ZooKeeper
{
    public class ChildListener : ChildListenerBase
    {
        public delegate void OnMetadataChangedHandler(List<ServiceMetadata> metadatas);
        public event OnMetadataChangedHandler OnMetadataChanged;
        private ZooKeeperClient _zkClient;

        public List<ServiceMetadata> Metadatas { get; private set; }

        public ChildListener(ZooKeeperClient zkClient, string servicePath) : base(servicePath)
        {
            _zkClient = zkClient;
        }

        public override void OnChildrenChanged(List<string> children)
        {
            System.Console.WriteLine("ChildListener.OnChildrenChanged = [{0}]", string.Join(",", children));
            var metadataBytes = children.Select(child => _zkClient.GetData(child, false, null));
            this.Metadatas = metadataBytes.Select(bytes => bytes.ToMetadata()).ToList();
            this.OnMetadataChangedHandle(this.Metadatas);
        }

        private void OnMetadataChangedHandle(List<ServiceMetadata> metadatas)
        {
            if (this.OnMetadataChanged != null)
                this.OnMetadataChanged(metadatas);
        }

        public override int GetHashCode()
        {
            return base.GroupPath.GetHashCode();
        }
    }
}