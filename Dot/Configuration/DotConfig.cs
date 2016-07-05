using System;
using System.Configuration;
using System.Xml;
using Dot.Extension;

namespace Dot.Configuration
{
    public class DotConfig : IConfigurationSectionHandler
    {
        public static DotConfig Default { get; private set; }
        public string EngineType { get; private set; }
        public string AssemblySkipPattern { get; private set; }
        public string AssemblyRestrictPattern { get; private set; }
        public bool IsWebApplication { get; private set; }

        static DotConfig()
        {
            Default = new DotConfig();
            Default.EngineType = "Dot.Engine.DefaultEngine, Dot";
            Default.AssemblySkipPattern = "^System|^mscorlib|^Microsoft|^AjaxControlToolkit|^Antlr3|^Autofac|^AutoMapper|^Castle|^ComponentArt|^CppCodeProvider|^DotNetOpenAuth|^EntityFramework|^EPPlus|^FluentValidation|^ImageResizer|^itextsharp|^log4net|^MaxMind|^MbUnit|^MiniProfiler|^Mono.Math|^MvcContrib|^Newtonsoft|^NHibernate|^nunit|^Org.Mentalis|^PerlRegex|^QuickGraph|^Recaptcha|^Remotion|^RestSharp|^Rhino|^Telerik|^Iesi|^TestDriven|^TestFu|^UserAgentStringLibrary|^VJSharpCodeProvider|^WebActivator|^WebDev|^WebGrease";
            Default.AssemblyRestrictPattern = "Dot.*";
            Default.IsWebApplication = true;
        }

        public object Create(object parent, object configContext, XmlNode section)
        {
            var config = new DotConfig();

            config.EngineType = section.GetNode("Engine").GetAttributeValue("Type");
            config.AssemblySkipPattern = section.GetNode("AssemblySkipPattern").GetAttributeValue("Value");
            config.AssemblyRestrictPattern = section.GetNode("AssemblyRestrictPattern").GetAttributeValue("Value");
            config.IsWebApplication = Convert.ToBoolean(section.GetNode("IsWebApplication").GetAttributeValue("Value"));

            return config;
        }
    }
}