using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Dot.Extension;

namespace Dot.ServiceModel.Channels
{
    public class BindingConfig : IConfigurationSectionHandler
    {
        public static Dictionary<string, Type> BINDING_NAMES = new Dictionary<string, Type>();
        public static BindingConfig DEFAULT = new BindingConfig();
        public Dictionary<Type, Dictionary<string, BindingSetting>> Settings { get; private set; }

        static BindingConfig()
        {
            BINDING_NAMES.Add("basichttp", typeof(BasicHttpBinding));
            BINDING_NAMES.Add("basichttpbinding", typeof(BasicHttpBinding));
        }

        public BindingConfig()
        {
            this.Settings = new Dictionary<Type, Dictionary<string, BindingSetting>>();
            this.AddSetting(BindingSetting.DefaultBasicHttp);
        }

        public object Create(object parent, object configContext, XmlNode section)
        {
            var config = new BindingConfig();

            var bindingNodes = section.SelectNodes("binding");
            foreach (XmlNode bindingNode in bindingNodes)
            {
                var bindingTypeNick = bindingNode.GetAttributeValue("bindingType").ToLower();
                Type bindingType;
                if (!BINDING_NAMES.TryGetValue(bindingTypeNick, out bindingType))
                    continue;

                var maxReceivedMessageSize = Convert.ToInt64(bindingNode.GetAttributeValue("messageSize", int.MaxValue.ToString()));
                var maxBufferSize = Convert.ToInt32(bindingNode.GetAttributeValue("bufferSize", int.MaxValue.ToString()));
                var maxBufferPoolSize = Convert.ToInt64(bindingNode.GetAttributeValue("bufferPoolSize", int.MaxValue.ToString()));
                var sendTimeout = TimeSpan.FromMilliseconds(Convert.ToInt32(bindingNode.GetAttributeValue("sendTimeout", "6000")));
                var openTimeout = TimeSpan.FromMilliseconds(Convert.ToInt32(bindingNode.GetAttributeValue("openTimeout", "6000")));

                var names = bindingNode.GetAttributeValue("name").Split(',', true).Distinct();
                foreach (var name in names)
                {
                    var setting = new BindingSetting
                    {
                        BindingType = bindingType,
                        Name = name,
                        MaxReceivedMessageSize = maxReceivedMessageSize,
                        MaxBufferSize = maxBufferSize,
                        MaxBufferPoolSize = maxBufferPoolSize,
                        SendTimeout = sendTimeout,
                        OpenTimeout = openTimeout
                    };

                    config.AddSetting(setting);
                }
            }

            return config;
        }

        private void AddSetting(BindingSetting setting)
        {
            Dictionary<string, BindingSetting> keyValue;
            if (!this.Settings.TryGetValue(setting.BindingType, out keyValue))
            {
                keyValue = new Dictionary<string, BindingSetting>();
                this.Settings.Add(setting.BindingType, keyValue);
            }

            if (!keyValue.ContainsKey(setting.Name))
                keyValue.Add(setting.Name, setting);
        }
    }
}