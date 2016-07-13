using System.Configuration;
using Dot.Database;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dot.Test.Database
{
    [TestClass]
    public class DatabaseConfigTest
    {
        [TestMethod]
        public void Database_DatabaseConfig_Test()
        {
            var config = ConfigurationManager.GetSection("databases") as DatabaseConfig;
            Assert.IsNotNull(config);
            Assert.IsTrue(config.DatabaseSettings.ContainsKey("sjgo"));

            var database = config.DatabaseSettings["sjgo"];
            Assert.AreEqual<string>("server=192.168.1.1;user id=sa;password=sa;database=cqssWrite", database.Write);
            Assert.AreEqual<string>("server=192.168.1.2;user id=sa;password=sa;database=cqssRead", database.Reads[0]);
            Assert.AreEqual<string>("server=192.168.1.3;user id=sa;password=sa;database=cqssRead", database.Reads[1]);
        }
    }
}