using System;

namespace Dot.ServiceModel
{
    public class ServiceNotFoundException : Exception
    {
        public Type ServiceType { get; private set; }

        public ServiceNotFoundException(Type serviceType)
            : base("Can not found service[" + serviceType.FullName + "]")
        {
            this.ServiceType = serviceType;
        }
    }
}