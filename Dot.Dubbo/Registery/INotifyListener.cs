using System.Collections.Generic;
using Dot.ServiceModel;

namespace Dot.Dubbo.Registery
{
    public interface INotifyListener
    {
        void Notify(List<ServiceMetadata> metadatas);
    }
}