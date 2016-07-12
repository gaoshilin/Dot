using System.Collections.Concurrent;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Dot.Database.SQLServer
{
    public class SQLServerConnectionFactory : DbConnectionFactoryBase
    {
        protected override string DoGetConnectionString(string name)
        {
            return ConfigurationManager.AppSettings[name] ?? string.Empty;
        }

        protected override IDbConnection DoConnect(string connectionString)
        {
            var connection = new SqlConnection(connectionString);
            connection.Open();
            return connection;
        }
    }
}