using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Debonair.FluentApi
{
    public interface IEntityMapping
    {
        IList<IPropertyMapping> PropertyMappings { get; }

        IPropertyMapping PrimaryKey { get; }
        IPropertyMapping IsDeletedProperty { get; }
        IEnumerable<IPropertyMapping> Properties { get; }

        string SchemaName { get; }
        string TableName { get; }
    }

    public interface IEntityMapping<TEntity> : IEntityMapping where TEntity : class
    {

        #region set methods

        IEntityMapping<TEntity> SetPrimaryKey(Expression<Func<TEntity, object>> expression);
        IEntityMapping<TEntity> SetIsDeletedProperty(Expression<Func<TEntity, object>> expression);
        IEntityMapping<TEntity> SetIgnore(Expression<Func<TEntity, object>> expression);
        IEntityMapping<TEntity> SetSchemaName(string schemaName);
        IEntityMapping<TEntity> SetTableName(string tableName);
        IEntityMapping<TEntity> SetColumnName(Expression<Func<TEntity, object>> expression, string columnName);

        #endregion set methods

        IPropertyMapping GetMappingForType(PropertyInfo info);
        MemberInfo GetMemberInfo(LambdaExpression lambda);
    }
}