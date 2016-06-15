using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dot.Extension;

namespace Dot.Sample
{
    partial class Program
    {
        static void Main(string[] args)
        {
            RunRandomLoadBalance();
            RunRoundRobinLoadBalance();
            RunConsistenHashLoadBalance();

            Console.Read();
        }
    }
}