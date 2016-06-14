using System;
using System.Configuration;
using System.Xml;
using Dot.Extension;

namespace Dot.ZooKeeper
{
    public class ZooKeeperConfig : IConfigurationSectionHandler
    {
        public static ZooKeeperConfig Default { get; private set; }
        public string ConnectionString { get; private set; }
        public int ConnectTimeoutMs { get; private set; }
        public int SessionTimeoutMs { get; private set; }

        static ZooKeeperConfig()
        {
            Default = new ZooKeeperConfig();
            Default.ConnectionString = "192.168.81.102:2181,192.168.81.102:2182,192.168.81.102:2183,192.168.81.102:2184,192.168.81.102:2185";
            Default.ConnectTimeoutMs = int.MaxValue;
            Default.SessionTimeoutMs = 20000;
        }

        public object Create(object parent, object configContext, XmlNode section)
        {
            var config = new ZooKeeperConfig();

            config.ConnectionString = section.GetNode("connectionString").GetAttributeValue("value");
            config.ConnectTimeoutMs = Convert.ToInt32(section.GetNode("connectTimeoutMs").GetAttributeValue("value"));
            config.SessionTimeoutMs = Convert.ToInt32(section.GetNode("sessionTimeoutMs").GetAttributeValue("value"));

            return config;
        }
    }
}