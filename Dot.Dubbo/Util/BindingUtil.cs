using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Dot.Dubbo.Util
{
    internal static class BindingUtil
    {
        public static Binding CreateBinding(Type type)
        {
            return CreateBinding(type, BindingSetting.Default);
        }

        public static Binding CreateBinding(Type type, BindingSetting setting)
        {
            if (type == typeof(BasicHttpBinding))
            {
                var binding = new BasicHttpBinding();
                binding.MaxReceivedMessageSize = setting.BasicHttpBindingSetting.MaxReceivedMessageSize;
                binding.MaxBufferSize = setting.BasicHttpBindingSetting.MaxBufferSize;
                binding.MaxBufferPoolSize = setting.BasicHttpBindingSetting.MaxBufferPoolSize;
                binding.SendTimeout = setting.BasicHttpBindingSetting.SendTimeout;
                binding.OpenTimeout = setting.BasicHttpBindingSetting.OpenTimeout;
                return binding;
            }
            else if (type == typeof(WSHttpBinding))
            {
                var binding = new WSHttpBinding();
                binding.MaxReceivedMessageSize = setting.WsHttpBindingSetting.MaxReceivedMessageSize;
                binding.MaxBufferPoolSize = setting.WsHttpBindingSetting.MaxBufferPoolSize;
                binding.SendTimeout = setting.WsHttpBindingSetting.SendTimeout;
                binding.OpenTimeout = setting.WsHttpBindingSetting.OpenTimeout;
                return binding;
            }
            else
            {
                throw new Exception(string.Format("no binding type {0}", type.Name));
            }
        }
    }

    public class BindingSetting
    {
        public static readonly BindingSetting Default = new BindingSetting
        {
            BasicHttpBindingSetting = BasicHttpBindingSetting.Default,
            WsHttpBindingSetting = WsHttpBindingSetting.Default
        };

        public BasicHttpBindingSetting BasicHttpBindingSetting { get; set; }
        public WsHttpBindingSetting WsHttpBindingSetting { get; set; }
    }

    public class BindingSettingBase
    {
        public int MaxReceivedMessageSize { get; set; }
        public int MaxBufferPoolSize { get; set; }
        public TimeSpan SendTimeout { get; set; }
        public TimeSpan OpenTimeout { get; set; }
    }

    public class BasicHttpBindingSetting : BindingSettingBase
    {
        public int MaxBufferSize { get; set; }
        public static readonly BasicHttpBindingSetting Default = new BasicHttpBindingSetting
        {
            MaxReceivedMessageSize = int.MaxValue,
            MaxBufferPoolSize = int.MaxValue,
            MaxBufferSize = int.MaxValue,
            SendTimeout = TimeSpan.FromMilliseconds(60000),
            OpenTimeout = TimeSpan.FromMilliseconds(60000)
        };
    }

    public class WsHttpBindingSetting : BindingSettingBase
    {
        public static readonly WsHttpBindingSetting Default = new WsHttpBindingSetting
        {
            MaxReceivedMessageSize = int.MaxValue,
            MaxBufferPoolSize = int.MaxValue,
            SendTimeout = TimeSpan.FromMilliseconds(60000),
            OpenTimeout = TimeSpan.FromMilliseconds(60000)
        };
    }
}