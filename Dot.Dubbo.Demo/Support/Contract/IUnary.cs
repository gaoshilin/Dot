using System;
using System.Collections.Generic;
using System.ServiceModel;
using Dot.Dubbo.Registery;
using Dot.Dubbo.Registery.ZooKeeper;
using Dot.Dubbo.Rpc;
using Dot.LoadBalance;
using Dot.ServiceModel;
using Dot.ZooKeeper;

namespace Dot.Dubbo.Demo.Support.Contract
{
    [ServiceContract]
    public interface IUnary
    {
        [OperationContract]
        double Negate(double x);
    }

    public class Unary : IUnary
    {
        public double Negate(double x)
        {
            return -x;
        }
    }

    public class UnaryServiceInvoker : ServiceInvokerBase<IUnary>, IUnary
    {
        public UnaryServiceInvoker(IRegistery registery, string groupPath, ILoadBalance<ServiceMetadata> loadBalance)
            : base(registery, groupPath, loadBalance)
        {
        }

        public double Negate(double x)
        {
            using (var proxy = base.Open())
            {
                return proxy.Client.Negate(x);
            }
        }
    }
}