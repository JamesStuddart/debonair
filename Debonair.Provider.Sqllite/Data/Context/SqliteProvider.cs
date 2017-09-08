using System.Data;
using Debonair.Data;
using Debonair.Data.Context;
using Debonair.Data.Orm;
using Debonair.Provider.Sqlite.Data.Orm;

namespace Debonair.Provider.Sqlite.Data.Context
{
    public class SqliteProvider<TEntity> : IDataProvider<TEntity> where TEntity : class, new()
    {
        public IDbConnection DbConnection { get; }
        public IContext DataContext { get; }
        public ICrudGenerator<TEntity> CrudGenerator { get; }

        public SqliteProvider(IDbConnection connection)
        {
            DbConnection = connection;
            CrudGenerator = new SqliteCrudGenerator<TEntity>();
            DataContext = new SqlContext(DbConnection);
        }

    }
}
