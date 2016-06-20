using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dot.LoadBalance;
using Dot.Sample.Support;

namespace Dot.Sample
{
    partial class Program
    {
        static void RunRandomLoadBalance()
        {
            var random = new RandomLoadBalance(); // 负载均衡器
            var people = Enumerable.Range(10, 4).Select(num => new Person { Id = num, Weight = num }).ToList(); // 待负载均衡的元素
            var totalWeight = people.Sum(person => person.Weight); // 待负载均衡的元素的权重和
            var calculator = new PersonWeightCalculator(); // 权重计算器
            var limitedWeight = 10;
            var limitCalculator = new PersonLimitedWeightCalculator(limitedWeight); // 限制最大权重和的权重计算器（当遇到超大的权重和时，对于 RandomLoadBalance 该算法可以提升加权负载的性能）

            var monitor = people.ToDictionary(person => person, person => 0); // 命中记数器
            Console.WriteLine("--- Run random load balance with weight[{0}] calculator ---", totalWeight);
            for (int i = 1; i <= 20; i++)
            {
                var person = random.Select(calculator, people);
                monitor[person]++;
            }
            DisplayLoadBalanceMonitor(monitor);

            monitor = people.ToDictionary(person => person, person => 0);
            Console.WriteLine("--- Run random load balance with limited weight[{0}] calculator ---", limitedWeight);
            for (int i = 1; i <= 20; i++)
            {
                var person = random.Select(limitCalculator, people);
                monitor[person]++;
            }
            DisplayLoadBalanceMonitor(monitor);
        }

        static void RunRoundRobinLoadBalance()
        {
            var roundRobin = new RoundRobinLoadBalance();
            var people = new List<int> { 210, 350, 440 }.Select(num => new Person { Id = num, Weight = num }).ToList();
            var totalWeight = people.Sum(person => person.Weight);
            var calculator = new PersonWeightCalculator();
            var limitedWeight = 10;
            var limitCalculator = new PersonLimitedWeightCalculator(limitedWeight); // 限制最大权重和的权重计算器（当遇到超大的权重和时，对于 RoundRobinLoadBalance 该算法可以提升加权负载的性能，减少内存占用，但命中率可能会产生小幅度的偏差）

            var monitor = people.ToDictionary(person => person, person => 0);
            Console.WriteLine("--- Run round robin load balance with weight[{0}] calculator ---", totalWeight);
            for (int i = 1; i <= 1000; i++)
            {
                var person = roundRobin.Select(calculator, people, "normal");
                monitor[person]++;
            }
            DisplayLoadBalanceMonitor(monitor);

            monitor = people.ToDictionary(person => person, person => 0);
            Console.WriteLine("--- Run round robin load balance with limited weight[{0}] calculator ---", limitedWeight);
            for (int i = 1; i <= 1000; i++)
            {
                var person = roundRobin.Select(limitCalculator, people, "limited");
                monitor[person]++;
            }
            DisplayLoadBalanceMonitor(monitor);
        }

        static void RunConsistenHashLoadBalance()
        {
            var consistentHash = new ConsistentHashLoadBalance();
            var people = Enumerable.Range(10, 4).Select(num => new Person { Id = num, Weight = num }).ToList();
            var totalWeight = people.Sum(person => person.Weight);
            var calculator = new PersonWeightCalculator();
            var limitedWeight = 10;
            var limitCalculator = new PersonLimitedWeightCalculator(limitedWeight); // 限制最大权重和的权重计算器（当遇到超大的权重和时，对于 ConsistentHashLoadBalance 该算法可以提升加权负载的性能，减少内存占用）

            var monitor = people.ToDictionary(person => person, person => 0);
            Console.WriteLine("--- Run consistent hash load balance with weight[{0}] calculator ---", totalWeight);
            for (int i = 1; i <= 24; i++)
            {
                var person = consistentHash.Select(calculator, people, "normal");
                monitor[person]++;
            }
            DisplayLoadBalanceMonitor(monitor);

            monitor = people.ToDictionary(person => person, person => 0);
            Console.WriteLine("--- Run consistent hash load balance with limited weight[{0}] calculator ---", limitedWeight);
            for (int i = 1; i <= 24; i++)
            {
                var person = consistentHash.Select(limitCalculator, people, "limited");
                monitor[person]++;
            }
            DisplayLoadBalanceMonitor(monitor);
        }

        static void DisplayLoadBalanceMonitor<T>(Dictionary<T, int> monitor)
        {
            var totalHit = (double)monitor.Values.Sum();
            var hitMessages = monitor.Select(kv => string.Format("{0} hit {1} times, hit precent {2}%", kv.Key, kv.Value, kv.Value * 100 / totalHit));
            Console.WriteLine(string.Join(Environment.NewLine, hitMessages));
        }
    }
}