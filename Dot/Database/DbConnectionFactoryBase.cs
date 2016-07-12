using System;
using System.Collections.Concurrent;
using System.Data;
using Dot.Extension;

namespace Dot.Database
{
    /// <summary>
    /// 父类已做好了异常处理，子类只需重写 DoGetConnectionString、DoConnect 方法，不需要再异常处理。
    /// </summary>
    public abstract class DbConnectionFactoryBase : IDbConnectionFactory
    {
        protected ConcurrentDictionary<string, string> _connectionStrings = new ConcurrentDictionary<string, string>();

        public IDbConnection Create(string name)
        {
            var connectionString = this.GetConnectionString(name);
            return this.Connect(connectionString);
        }

        protected string GetConnectionString(string name)
        {
            try
            {
                string connectionString;
                if (!_connectionStrings.TryGetValue(name, out connectionString))
                {
                    connectionString = this.DoGetConnectionString(name);
                    _connectionStrings.TryAdd(name, connectionString);
                }

                if (string.IsNullOrEmpty(connectionString))
                    throw new Exception("the connection string is null or empty");

                return connectionString;
            }
            catch (Exception ex)
            {
                throw new Exception("Get connection string by name = [{0}] fail. because {1}.".FormatWith(name, ex.Message));
            }
        }

        protected abstract string DoGetConnectionString(string name);

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