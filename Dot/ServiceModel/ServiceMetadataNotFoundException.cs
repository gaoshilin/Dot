using System;

namespace Dot.ServiceModel
{
    public class ServiceMetadataNotFoundException : Exception
    {
        public Type ServiceType { get; private set; }

        public ServiceMetadataNotFoundException(Type serviceType)
            : base("Can not found any service metadata for service[" + serviceType.FullName + "]")
        {
            this.ServiceType = serviceType;
        }
    }
}