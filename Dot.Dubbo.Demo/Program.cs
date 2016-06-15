using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using Dot.Dubbo.Demo.Support.Contract;
using Dot.Dubbo.Demo.Support.Listener;
using Dot.Dubbo.Demo.Support.LoadBalance;
using Dot.Dubbo.Registery.ZooKeeper;
using Dot.Extension;
using Dot.LoadBalance;
using Dot.ServiceModel;
using Dot.ZooKeeper;

namespace Dot.Dubbo.Demo
{
    class Program
    {
        static string CALC_SERVICE_PROVIDER_PATH = "/dotdubbo/calc/providers";
        static int CALC_SERVICE_MIN_PORT = 7666;
        static int CALC_SERVICE_MAX_COUNT = 3;

        static string UNARY_SERVICE_PROVIDER_PATH = "/dotdubbo/unary/providers";
        static int UNARY_SERVICE_MIN_PORT = 8666;
        static int UNARY_SERVICE_MAX_COUNT = 2;

        static void Main2(string[] args)
        {
            // 创建注册中心
            var registery = new ZooKeeperRegistery(ZooKeeperClient.Instance);

            // 监听 UnaryService 变化（推模式获取服务元数据）
            var listener = new DebugNotifyListener();
            registery.Subscribe(UNARY_SERVICE_PROVIDER_PATH, listener, true);

            // 启动 UnaryService
            var ports = Enumerable.Range(UNARY_SERVICE_MIN_PORT, UNARY_SERVICE_MAX_COUNT).ToList();
            var metadatas = ports.Select(port => HostUnaryService(port)).ToList();

            // 注册 UnaryService
            metadatas.ForEach(metadata => registery.Register(metadata, true), 1000);

            // 取消注册 UnaryService
            metadatas.ForEach(metadata => registery.Unregister(metadata, true), 1000);

            // 取消监听 UnaryService 变化
            registery.Unsubscribe(UNARY_SERVICE_PROVIDER_PATH, listener, true);

            // 再次注册 UnaryService
            metadatas.ForEach(metadata => registery.Register(metadata, true), 1000);

            // 拉模式获取 UnaryService 元数据
            var metas = registery.Lookup(CALC_SERVICE_PROVIDER_PATH);
            Console.WriteLine(string.Join(",", metas.Select(m => m.FullPath)));
            Console.WriteLine("-------------------");

            // 再次取消 UnaryService 注册
            metadatas.ForEach(metadata => registery.Unregister(metadata, true), 1000);

            Console.Read();
        }

        static void Main(string[] args)
        {
            var weightCalculator = new ServiceMetadataWeightCalculator(30);
            var loadBalance = new RoundRobinLoadBalance();

            // 创建注册中心
            var registery = new ZooKeeperRegistery(ZooKeeperClient.Instance);

            // 启动并注册 UnaryService
            Enumerable.Range(UNARY_SERVICE_MIN_PORT, UNARY_SERVICE_MAX_COUNT)
                      .Select(port => HostUnaryService(port))
                      .ForEach(meta => registery.Register(meta, true), 1000);

            // 调用 UnaryService
            var unaryInvoker = new UnaryServiceInvoker(registery, UNARY_SERVICE_PROVIDER_PATH, loadBalance, weightCalculator);
            for (int i = -1; i >= -10; i--)
                Console.WriteLine("unaryInvoker.Negate({0}) = {1}", i, unaryInvoker.Negate(i));
            Console.WriteLine("------------------");


            // 启动并注册 CalcService
            Enumerable.Range(CALC_SERVICE_MIN_PORT, CALC_SERVICE_MAX_COUNT)
                      .Select(port => HostCalcService(port))
                      .ForEach(meta => registery.Register(meta, true), 1000);

            // 调用 CalcService
            var calcInvoker = new CalculateServiceInvoker(registery, CALC_SERVICE_PROVIDER_PATH, loadBalance, weightCalculator);
            for (int i = 1; i <= 10; i++)
            {
                calcInvoker.Multicast(i);
                Console.WriteLine("{0} + {1} = {2}", i, i + 1, calcInvoker.Add(i, i + 1));
                Console.WriteLine("-------------------");
                System.Threading.Thread.Sleep(1000);
            }

            Console.Read();

            // 断开与 ZooKeeper 服务器的连接
            ZooKeeperClient.Instance.Dispose();
        }

        static ServiceMetadata HostCalcService(int port)
        {
            var baseAddress = string.Format("http://192.168.81.102:{0}/calc", port);
            var address = new EndpointAddress(baseAddress);
            var binding = new BasicHttpBinding();
            var contract = ContractDescription.GetContract(typeof(ICalculate));
            var endpoint = new ServiceEndpoint(contract, binding, address);

            var host = new ServiceHost(typeof(Calculate), new Uri(baseAddress));
            host.AddServiceEndpoint(endpoint);
            host.Open();

            Console.WriteLine("{0} service {1} hosted.", DateTime.Now.ToString("HH:mm:ss"), baseAddress);
            Console.WriteLine("----------------------------");

            var metadata = new ServiceMetadata(typeof(ICalculate), "/dotdubbo/calc/providers", address, binding);
            metadata.Weight = port;
            return metadata;
        }

        static ServiceMetadata HostUnaryService(int port)
        {
            var baseAddress = string.Format("http://192.168.81.102:{0}/unary", port);
            var address = new EndpointAddress(baseAddress);
            var binding = new BasicHttpBinding();
            var contract = ContractDescription.GetContract(typeof(IUnary));
            var endpoint = new ServiceEndpoint(contract, binding, address);

            var host = new ServiceHost(typeof(Unary), new Uri(baseAddress));
            host.AddServiceEndpoint(endpoint);
            host.Open();

            Console.WriteLine("{0} service {1} hosted.", DateTime.Now.ToString("HH:mm:ss"), baseAddress);
            Console.WriteLine("----------------------------");

            var metadata = new ServiceMetadata(typeof(IUnary), "/dotdubbo/unary/providers", address, binding);
            metadata.Weight = port;
            return metadata;
        }
    }
}