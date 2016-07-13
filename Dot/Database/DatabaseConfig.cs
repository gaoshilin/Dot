using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Dot.Database
{
    public class DatabaseConfig : IConfigurationSectionHandler
    {
        public Dictionary<string, DatabaseSetting> DatabaseSettings { get; private set; }

        public DatabaseConfig()
        {
            this.DatabaseSettings = new Dictionary<string, DatabaseSetting>();
        }

        public object Create(object parent, object configContext, XmlNode section)
        {
            var config = new DatabaseConfig();
            using (var inputStream = new MemoryStream(Encoding.UTF8.GetBytes(section.OuterXml)))
            {
                var settings = XElement.Load(inputStream);
                foreach (var setting in settings.Elements("database"))
                {
                    var name = setting.Attribute("name").Value;
                    var write = setting.Element("write").Value;
                    var reads = setting.Element("reads").Elements("read").Select(t => t.Value).ToList();

                    if (!config.DatabaseSettings.ContainsKey(name))
                    {
                        config.DatabaseSettings.Add(name, new DatabaseSetting
                        {
                            Name = name,
                            Write = write,
                            Reads = reads
                        });
                    }
                }
            }

            return config;
        }
    }
}