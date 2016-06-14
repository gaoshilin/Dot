using System;
using System.Collections.Generic;
using Dot.Dubbo.Registery;
using Dot.ServiceModel;

namespace Dot.Dubbo.Demo.Support.Listener
{
    class DebugNotifyListener : INotifyListener
    {
        public void Notify(List<ServiceMetadata> metadatas)
        {
            metadatas.ForEach(m => Console.WriteLine(m.FullPath));
            Console.WriteLine("-------------------------");
        }
    }
}