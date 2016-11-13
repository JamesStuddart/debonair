using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Debonair.FluentApi
{
    public class EntityMapping<TEntity> : IEntityMapping<TEntity> where TEntity : class 
    {
        public IList<IPropertyMapping> PropertyMappings { get; }

        public IPropertyMapping PrimaryKey { get { return PropertyMappings.FirstOrDefault(x => x.IsPrimaryKey); } }
        public IPropertyMapping IsDeletedProperty { get { return PropertyMappings.FirstOrDefault(x => x.IsDeletedProperty); } }
        public IEnumerable<IPropertyMapping> Properties { get { return PropertyMappings.Where(x => x != PrimaryKey && !x.IsIgnored); } }


        protected internal EntityMapping(IList<IPropertyMapping> propertyMappers = null)
        {
            PropertyMappings = propertyMappers ?? DefineMappings();
        }

        public IPropertyMapping GetMappingForType(PropertyInfo info)
        {
            var mapping = PropertyMappings.FirstOrDefault(x => x.PropertyInfo.GetMethod == info.GetMethod);

            if (mapping == null)
            {
                mapping = new PropertyMapping(info);
                PropertyMappings.Add(mapping);
            }

            return mapping;
        }

        #region set methods

        public string SchemaName { get; private set; }
        public string TableName { get; private set; }

        public void SetPrimaryKey(Expression<Func<TEntity, object>> expression)
        {
            DefineMapping(expression).SetPrimaryKey();
        }

        public void SetIsDeletedProperty(Expression<Func<TEntity, object>> expression)
        {
            DefineMapping(expression).SetPrimaryKey();

        }

        public void SetIgnore(Expression<Func<TEntity, object>> expression)
        {
            DefineMapping(expression).Ignore();
        }

        public void SetSchemaName(string schemaName)
        {
            SchemaName = schemaName;
        }

        public void SetTableName(string tableName)
        {
            TableName = tableName;
        }

        public void SetColumnName(Expression<Func<TEntity, object>> expression, string columnName)
        {
            DefineMapping(expression).SetColumnName(columnName);

        }

        #endregion set methods


        public MemberInfo GetMemberInfo(LambdaExpression lambda)
        {
            Expression expr = lambda;
            while (true)
            {
                switch (expr.NodeType)
                {
                    case ExpressionType.Lambda:
                        expr = ((LambdaExpression)expr).Body;
                        break;

                    case ExpressionType.Convert:
                        expr = ((UnaryExpression)expr).Operand;
                        break;

                    case ExpressionType.MemberAccess:
                        var memberExpression = (MemberExpression)expr;
                        var baseMember = memberExpression.Member;

                        var paramType = lambda.Parameters[0].Type;
                        var memberInfo = paramType.GetMember(baseMember.Name)[0];
                        return memberInfo;

                    default:
                        throw new NotSupportedException($"{lambda.NodeType} is an unsupported expression");
                }
            }
        }


        private IPropertyMapping DefineMapping(LambdaExpression lambda)
        {
            var info = (PropertyInfo)GetMemberInfo(lambda);

            var mapping = PropertyMappings.FirstOrDefault(x => x.PropertyInfo.GetMethod == info.GetMethod);

            if (mapping == null)
            {
                mapping = new PropertyMapping(info);
                PropertyMappings.Add(mapping);
            }

            return mapping;
        }

        private IList<IPropertyMapping> DefineMappings()
        {
            var mappings = new List<IPropertyMapping>();

            foreach (var info in typeof(TEntity).GetProperties())
            {
                mappings.Add(new PropertyMapping(info));
            }

            return mappings;
        }


    }
}
