using System.Data;
using Debonair.Data;
using Debonair.Data.Context;
using Debonair.Data.Orm;
using Debonair.Provider.SqlCe.Data.Orm;

namespace Debonair.Provider.SqlCe.Data.Context
{
    public class SqlCeProvider<TEntity> : IDataProvider<TEntity> where TEntity : class, new()
    {
        public IDbConnection DbConnection { get; }
        public IContext DataContext { get; }
        public ICrudGenerator<TEntity> CrudGenerator { get; }

        public SqlCeProvider(IDbConnection connection)
        {
            DbConnection = connection;
            CrudGenerator = new SqlCeCrudGenerator<TEntity>();
            DataContext = new SqlContext(DbConnection);
        }

    }
}
