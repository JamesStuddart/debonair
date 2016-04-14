using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Debonair.Entities;

namespace Debonair.Data.Orm
{
    public interface ICrudGenerator<TEntity> where TEntity : DebonairStandard, new()
    {

        #region Properties
        Dictionary<string,object> SelectParameters { get; set; }
        bool IsPrimaryKey { get; }

        PropertyMetadata IdentityProperty { get; }

        string TableName { get; }

        string Scheme { get; }

        IEnumerable<PropertyMetadata> KeyProperties { get; }

        IEnumerable<PropertyMetadata> BaseProperties { get; }

        PropertyMetadata IsDeletedProperty { get; }

        bool IsSoftDeletable { get; }

        #endregion

        #region Functions

        string Select(Expression<Func<TEntity, bool>> predicate = null, bool dirtyRead = true);

        string Insert();

        string Update();

        string Delete(bool forceDelete = false);

        #endregion
    }
}
