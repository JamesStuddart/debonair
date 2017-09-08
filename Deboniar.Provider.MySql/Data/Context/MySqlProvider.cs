using System.Data;
using Debonair.Data;
using Debonair.Data.Context;
using Debonair.Data.Orm;
using Debonair.Provider.MySql.Data.Orm;

namespace Debonair.Provider.MySql.Data.Context
{
    public class MySqlProvider<TEntity> : IDataProvider<TEntity> where TEntity : class, new()
    {
        public IDbConnection DbConnection { get; }
        public IContext DataContext { get; }
        public ICrudGenerator<TEntity> CrudGenerator { get; }

        public MySqlProvider(IDbConnection connection)
        {
            DbConnection = connection;
            CrudGenerator = new MySqlCrudGenerator<TEntity>(false);
            DataContext = new SqlContext(DbConnection);
        }

    }
}
