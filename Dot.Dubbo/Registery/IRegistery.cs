using System.Collections.Generic;
using Dot.ServiceModel;
using Dot.ZooKeeper.Unsubscribe;

namespace Dot.Dubbo.Registery
{
    public interface IRegistery
    {
        void Register(ServiceMetadata metadata, bool checkOnStart);
        void Unregister(ServiceMetadata metadata, bool checkOnStart);
        void Subscribe(string groupPath, INotifyListener listener, bool checkOnStart);
        void Unsubscribe(string groupPath, INotifyListener listener, bool checkOnStart);
        List<ServiceMetadata> Lookup(string groupPath);
    }
}