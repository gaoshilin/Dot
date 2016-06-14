using System;
using System.Collections.Generic;
using System.Configuration;
using System.ServiceModel;
using Dot.Dubbo.Demo.Support.LoadBalance;
using Dot.Dubbo.Registery;
using Dot.Dubbo.Registery.ZooKeeper;
using Dot.Dubbo.Rpc;
using Dot.LoadBalance;
using Dot.ServiceModel;
using Dot.ZooKeeper;

namespace Dot.Dubbo.Demo.Support.Contract
{
    [ServiceContract]
    public interface ICalculate
    {
        [OperationContract]
        double Add(double x, double y);

        [OperationContract]
        void Multicast(double x);
    }

    public class Calculate : ICalculate
    {
        private UnaryServiceInvoker _unaryInvoker;

        public Calculate()
        {
            var weightCalculator = new ServiceMetadataWeightCalculator(30);
            var loadBalance = new RoundRobinLoadBalance<ServiceMetadata>(weightCalculator);
            var registery = new ZooKeeperRegistery(ZooKeeperClient.Instance);
            _unaryInvoker = new UnaryServiceInvoker(registery, "/dotdubbo/unary/providers", loadBalance);
        }

        public double Add(double x, double y)
        {
            return x + y;
        }

        public void Multicast(double x)
        {
            System.Console.WriteLine("Multicast {0}", _unaryInvoker.Negate(x));
        }
    }

    public class CalculateServiceInvoker : ServiceInvokerBase<ICalculate>, ICalculate
    {
        public CalculateServiceInvoker(IRegistery registery, string groupPath, ILoadBalance<ServiceMetadata> loadBalance)
            : base(registery, groupPath, loadBalance)
        {
        }

        public double Add(double x, double y)
        {
            using (var proxy = base.Open())
            {
                return proxy.Client.Add(x, y);
            }
        }

        public void Multicast(double x)
        {
            foreach (var proxy in base.OpenAll())
            {
                using (proxy)
                {
                    proxy.Client.Multicast(x);
                }
            }
        }
    }
}