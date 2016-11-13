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

        void SetPrimaryKey(Expression<Func<TEntity, object>> expression);
        void SetIsDeletedProperty(Expression<Func<TEntity, object>> expression);
        void SetIgnore(Expression<Func<TEntity, object>> expression);
        void SetSchemaName(string schemaName);
        void SetTableName(string tableName);
        void SetColumnName(Expression<Func<TEntity, object>> expression, string columnName);

        #endregion set methods

        IPropertyMapping GetMappingForType(PropertyInfo info);
        MemberInfo GetMemberInfo(LambdaExpression lambda);
    }
}