using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Debonair.Entities;

namespace Debonair.Data
{
    public interface IDataRepository<TEntity> where TEntity : DebonairStandard, new()
    {
        IEnumerable<TEntity> Select(Expression<Func<TEntity, bool>> predicate = null, bool dirtyRead = true);

        bool Insert(TEntity entity);

        bool Update(TEntity entity);

        bool Delete(TEntity entity, bool forceDelete = false);

        IEnumerable<TEntity> ExecuteStoredProcedure<TEntity>(string spName, object parameters);
        IEnumerable<TEntity> ExecuteStoredProcedure<TEntity>(string spName);

        void ExecuteStoredProcedure(string spName, object parameters);
        void ExecuteStoredProcedure(string spName);
    }
}
