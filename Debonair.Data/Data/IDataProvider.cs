
using System.Data;
using Debonair.Data.Context;
using Debonair.Data.Orm;

namespace Debonair.Data
{
    public interface IDataProvider<TEntity> where TEntity : class, new()
    {
        IContext DataContext { get; }
        ICrudGenerator<TEntity> CrudGenerator { get;  }
        IDbConnection DbConnection { get; }
    }
}
