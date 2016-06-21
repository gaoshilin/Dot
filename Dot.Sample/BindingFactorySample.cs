using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Dot.ServiceModel.Channels;

namespace Dot.Sample
{
    partial class Program
    {
        public static void RunBindingFactorySample()
        {
            var binding = BindingFactory.Create<BasicHttpBinding>() as BasicHttpBinding;
            Console.WriteLine("MaxReceivedMessageSize = {0}", binding.MaxReceivedMessageSize);
            Console.WriteLine("MaxBufferSize          = {0}", binding.MaxBufferSize);
            Console.WriteLine("MaxBufferPoolSize      = {0}", binding.MaxBufferPoolSize);
            Console.WriteLine("OpenTimeout            = {0}", binding.OpenTimeout);
            Console.WriteLine("SendTimeout            = {0}", binding.SendTimeout);
            Console.WriteLine("----------------------------");

            var binding2 = BindingFactory.Create(typeof(BasicHttpBinding)) as BasicHttpBinding;
            Console.WriteLine("MaxReceivedMessageSize = {0}", binding2.MaxReceivedMessageSize);
            Console.WriteLine("MaxBufferSize          = {0}", binding2.MaxBufferSize);
            Console.WriteLine("MaxBufferPoolSize      = {0}", binding2.MaxBufferPoolSize);
            Console.WriteLine("OpenTimeout            = {0}", binding2.OpenTimeout);
            Console.WriteLine("SendTimeout            = {0}", binding2.SendTimeout);
            Console.WriteLine("----------------------------");

            var binding3 = BindingFactory.Create<BasicHttpBinding>("lucene") as BasicHttpBinding;
            Console.WriteLine("MaxReceivedMessageSize = {0}", binding3.MaxReceivedMessageSize);
            Console.WriteLine("MaxBufferSize          = {0}", binding3.MaxBufferSize);
            Console.WriteLine("MaxBufferPoolSize      = {0}", binding3.MaxBufferPoolSize);
            Console.WriteLine("OpenTimeout            = {0}", binding3.OpenTimeout);
            Console.WriteLine("SendTimeout            = {0}", binding3.SendTimeout);
            Console.WriteLine("----------------------------");

            var binding4 = BindingFactory.Create<BasicHttpBinding>("promotion") as BasicHttpBinding;
            Console.WriteLine("MaxReceivedMessageSize = {0}", binding4.MaxReceivedMessageSize);
            Console.WriteLine("MaxBufferSize          = {0}", binding4.MaxBufferSize);
            Console.WriteLine("MaxBufferPoolSize      = {0}", binding4.MaxBufferPoolSize);
            Console.WriteLine("OpenTimeout            = {0}", binding4.OpenTimeout);
            Console.WriteLine("SendTimeout            = {0}", binding4.SendTimeout);
            Console.WriteLine("----------------------------");
        }
    }
}