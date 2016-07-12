using System.Data;

namespace Dot.Database
{
    public interface IDbConnectionFactory
    {
        IDbConnection Create(string name);
    }
}