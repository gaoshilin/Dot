using System;
using System.IO;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Layout;
using Dot.Dependency;

namespace Dot.Logging
{
    [Registration(LifeCycle = LifeCycle.Singelton, RegisterMode = RegisterMode.Self)]
    public class LoggerFactory
    {
        public LoggerFactory() 
            : this("log4net.config")
        {
        }

        public LoggerFactory(string configFile)
        {
            var file = File.Exists(configFile)
                ? new FileInfo(configFile)
                : new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFile));

            if (file.Exists)
                XmlConfigurator.ConfigureAndWatch(file);
            else
                BasicConfigurator.Configure(new ConsoleAppender { Layout = new PatternLayout() });
        }

        public ILog Create(string name = "")
        {
            return LogManager.GetLogger(name);
        }

        public ILog Create(Type type)
        {
            return LogManager.GetLogger(type);
        }
    }
}