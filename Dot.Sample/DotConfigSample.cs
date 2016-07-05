using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dot.Configuration;

namespace Dot.Sample
{
    partial class Program
    {
        static void RunDotConfigSample()
        {
            var config = ConfigurationManager.GetSection("dotConfig") as DotConfig;
            Console.WriteLine("EngintType = {0}", config.EngineType);
            Console.WriteLine("AssemblySkipPattern = {0}", config.AssemblySkipPattern);
            Console.WriteLine("AssemblyRestrictPattern = {0}", config.AssemblyRestrictPattern);
            Console.WriteLine("IsWebApplication = {0}", config.IsWebApplication);
        }
    }
}