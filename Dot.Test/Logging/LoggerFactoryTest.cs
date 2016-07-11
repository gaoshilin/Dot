using Autofac;
using Dot.Dependency.Engine;
using Dot.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dot.Test.Logging
{
    [TestClass]
    public class LoggerFactoryTest
    {
        [TestMethod]
        public void Logging_LoggerFactory_Test()
        {
            var loggerFactory = EngineContext.Current.Resolve<LoggerFactory>();
            Assert.IsNotNull(loggerFactory);

            var logger = loggerFactory.Create("foo");
            logger.Debug("foo");

            logger = loggerFactory.Create(this.GetType());
            logger.Error("bar");
        }
    }
}