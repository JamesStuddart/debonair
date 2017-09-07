using System.Data;
using Debonair.Data;
using Debonair.Data.Context;
using Debonair.Data.Orm;
using Debonair.Provider.MsSql.Data.Orm;

namespace Debonair.Provider.MsSql.Data.Context
{
    public class MsSqlProvider<TEntity> : IDataProvider<TEntity> where TEntity : class, new()
    {
        public IDbConnection DbConnection { get; }
        public IContext DataContext { get; }
        public ICrudGenerator<TEntity> CrudGenerator { get; }

        public MsSqlProvider(IDbConnection connection)
        {
            DbConnection = connection;
            CrudGenerator = new MsSqlCrudGenerator<TEntity>(false);
            DataContext = new SqlContext(DbConnection);
        }

    }
}
