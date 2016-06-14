using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dot.LoadBalance;
using Dot.Sample.Support;

namespace Dot.Sample
{
    partial class Program
    {
        static void RunLoadBalanceSample_SingleThread()
        {
            Console.WriteLine("### RunLoadBalanceSample in single thread ###\r\n");

            var people = Enumerable.Range(1, 4).Select(id => new Person { Id = id, Weight = 1 }).ToList();
            var weightPeople = Enumerable.Range(1, 4).Select(weight => new Person { Id = weight, Weight = weight }).ToList();

            var calculator = new PersonWeightCalculator();
            var limitedCalculator = new PersonLimitedWeightCalculator(10);

            var random = new RandomLoadBalance<Person>(calculator);
            var roundRobin = new RoundRobinLoadBalance<Person>(calculator);
            var consistentHash = new ConsistentHashLoadBalance<Person>(calculator);

            Console.WriteLine("--- Random load balance for equal people ---");
            for (int i = 1; i <= 10; i++)
                Console.WriteLine(random.Select(people));

            Console.WriteLine("--- Random load balance for weight people ---");
            for (int i = 1; i <= 10; i++)
                Console.WriteLine(random.Select(weightPeople));

            Console.WriteLine("--- RoundRobin load balance for equal people ---");
            for (int i = 1; i <= 10; i++)
                Console.WriteLine(roundRobin.Select(people, "person"));

            Console.WriteLine("--- RoundRobin load balance for weight people ---");
            for (int i = 1; i <= 20; i++)
                Console.WriteLine(roundRobin.Select(weightPeople, "weightPerson"));

            Console.WriteLine("--- ConsistentHash load balance for equal people ---");
            for (int i = 1; i <= 10; i++)
                Console.WriteLine(consistentHash.Select(people, "person" + i % 2));

            Console.WriteLine("--- ConsistentHash load balance for weight people ---");
            for (int i = 1; i <= 10; i++)
                Console.WriteLine(consistentHash.Select(weightPeople, "weightPeople" + i % 2));

            Console.WriteLine("--- RoundRobin load balance with limited weight calculator for weight people ---");
            weightPeople = new List<int> { 3, 6, 24, 36 }.Select(weight => new Person { Id = weight, Weight = weight }).ToList();
            roundRobin = new RoundRobinLoadBalance<Person>(limitedCalculator);
            for (int i = 1; i <= 50; i++)
                Console.WriteLine(roundRobin.Select(weightPeople, "weightPerson"));
            Console.WriteLine("------------------");
        }
    }
}