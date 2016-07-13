using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Data;
using Dot.Extension;
using Dot.LoadBalance;
using Dot.LoadBalance.Weight;

namespace Dot.Database
{
    public abstract class ReadWriteDbConnectionFactory : IDbConnectionFactory
    {
        private ConcurrentDictionary<string, DatabaseSetting> _databases;
        private ILoadBalance _loadBalance;
        private IWeightCalculator<string> _weightCalculator = new EmptyWeightCalculator<string>();

        public ReadWriteDbConnectionFactory()
            : this(new RoundRobinLoadBalance())
        {
        }

        public ReadWriteDbConnectionFactory(ILoadBalance loadBalance)
        {
            var config = ConfigurationManager.GetSection("databases") as DatabaseConfig;
            _databases = new ConcurrentDictionary<string, DatabaseSetting>(config.DatabaseSettings);
            _loadBalance = loadBalance;
        }

        public IDbConnection Create(string name)
        {
            return this.CreateWrite(name);
        }

        public IDbConnection CreateWrite(string name)
        {
            DatabaseSetting database;
            if (!_databases.TryGetValue(name, out database))
                throw new Exception("Create db connection fail, because database not exists which keyed [{0}]".FormatWith(name));

            return this.Connect(database.Write);
        }

        public IDbConnection CreateRead(string name)
        {
            DatabaseSetting database;
            if (!_databases.TryGetValue(name, out database))
                throw new Exception("Create db connection fail, because database not exists which keyed [{0}]".FormatWith(name));

            var connectionString = _loadBalance.Select(_weightCalculator, database.Reads);
            return this.Connect(connectionString);
        }

        protected IDbConnection Connect(string connectionString)
        {
            try
            {
                return this.DoConnect(connectionString);
            }
            catch (Exception ex)
            {
                throw new Exception("Create database connection by connection string = [{0}] fail, because {1}.".FormatWith(connectionString, ex.Message));
            }
        }

        protected abstract IDbConnection DoConnect(string connectionString);
    }
}