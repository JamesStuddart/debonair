using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Debonair.Data.Orm
{
    public class PropertyMetadata
    {

        public string TableName { get; }

        public string ColumnName { get; }

        public string Name { get; }

        public bool IsSoftDeletable { get; }
        public PropertyMetadata(MemberInfo memberInfo)
        {
            Name = memberInfo.Name;
            var columnName = memberInfo.GetCustomAttribute<Column>();
            ColumnName = columnName != null ? columnName.Value : Name;

            var tableName = memberInfo.GetCustomAttribute<Table>();
            TableName = tableName != null ? tableName.Value : string.Empty;
        }

        public PropertyMetadata(Expression expression, Type entityType)
        {
            var tableAttribute = entityType.GetCustomAttribute<Table>();

            TableName = tableAttribute != null ? tableAttribute.Value : entityType.Name;

            var columnProperty = entityType.GetProperty(GetMemberExpression(expression).Member.Name);
            var columnAttribute = columnProperty.GetCustomAttribute<Column>();

            ColumnName = columnAttribute != null ? columnAttribute.Value : columnProperty.Name;

            var isDeletedAttribute = columnProperty.GetCustomAttribute<IsDeletedProperty>();
            IsSoftDeletable = isDeletedAttribute != null;
        }


        private static MemberExpression GetMemberExpression(Expression expression)
        {
            MemberExpression returnExpression;

            switch (expression.NodeType)
            {
                case ExpressionType.MemberAccess:
                    returnExpression = expression as MemberExpression;
                    break;
                case ExpressionType.Convert:
                    var unaryExpression = expression as UnaryExpression;
                    returnExpression = unaryExpression != null ? GetMemberExpression(unaryExpression.Operand) : null;
                    break;
                default:
                    throw new NotSupportedException($"{expression.NodeType} is an unsupported expression");
            }

            return returnExpression;
        }
    }
}
