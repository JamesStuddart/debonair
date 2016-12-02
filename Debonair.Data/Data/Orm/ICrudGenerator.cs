using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Debonair.Entities;
using Debonair.FluentApi;

namespace Debonair.Data.Orm
{
    public interface ICrudGenerator<TEntity> where TEntity : class, new()
    {

        #region Properties
        Dictionary<string, object> SelectParameters { get; set; }
        IEntityMapping<TEntity> EntityMapping { get; }
        #endregion

        #region Functions

        string Select(Expression<Func<TEntity, bool>> predicate = null, bool dirtyRead = true);

        string Insert();

        string Update();

        string Delete(bool forceDelete = false);

        #endregion
    }
}
