using System;
using System.ServiceModel;

namespace Dot.ServiceModel.Channels
{
    public class BindingSetting
    {
        public Type BindingType { get; set; }
        public string Name { get; set; }
        public long MaxReceivedMessageSize { get; set; }
        public int MaxBufferSize { get; set; }
        public long MaxBufferPoolSize { get; set; }
        public TimeSpan SendTimeout { get; set; }
        public TimeSpan OpenTimeout { get; set; }


        public static BindingSetting DefaultBasicHttp = new BindingSetting
        {
            BindingType = typeof(BasicHttpBinding),
            Name = "default",
            MaxReceivedMessageSize = int.MaxValue,
            MaxBufferSize = int.MaxValue,
            MaxBufferPoolSize = int.MaxValue,
            SendTimeout = TimeSpan.FromMilliseconds(6000),
            OpenTimeout = TimeSpan.FromMilliseconds(6000)
        };
    }
}