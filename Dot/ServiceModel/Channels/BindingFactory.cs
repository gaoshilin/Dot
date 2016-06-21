using System;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Dot.Extension;

namespace Dot.ServiceModel.Channels
{
    public static class BindingFactory
    {
        private static readonly BindingConfig BINDING_CONFIG = ConfigurationManager.GetSection("bindings") as BindingConfig ?? BindingConfig.DEFAULT;

        public static Binding Create<T>(string name = "default")
            where T : Binding
        {
            var bindingType = typeof(T);

            if (!BINDING_CONFIG.Settings.ContainsKey(bindingType))
                throw new Exception("can not create binding type of {0}, because the type have not register.".FormatWith(bindingType.Name));

            if (!BINDING_CONFIG.Settings[bindingType].ContainsKey(name))
                throw new Exception("can not create binding type of {0} by name = {1}, because the name have not register.".FormatWith(bindingType.Name, name));

            if (bindingType == typeof(BasicHttpBinding))
                return CreateBasicHttpBinding(name);

            throw new Exception("can not create binding for type of {0}.".FormatWith(bindingType.Name));
        }

        private static BasicHttpBinding CreateBasicHttpBinding(string name = "")
        {
            var setting = BINDING_CONFIG.Settings[typeof(BasicHttpBinding)][name];
            var binding = new BasicHttpBinding();
            binding.MaxReceivedMessageSize = setting.MaxReceivedMessageSize;
            binding.MaxBufferSize = setting.MaxBufferSize;
            binding.MaxBufferPoolSize = setting.MaxBufferPoolSize;
            binding.SendTimeout = setting.SendTimeout;
            binding.OpenTimeout = setting.OpenTimeout;
            return binding;
        }
    }
}