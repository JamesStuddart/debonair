using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Debonair.Data.Orm.QueryBuilder.ExpressionTree;
using Debonair.Entities;
using Debonair.FluentApi;

namespace Debonair.Data.Orm.QueryBuilder
{
    internal class LambdaToSql<TEntity> where TEntity : class, new()
    {
        private IEntityMapping<TEntity> EntityMapping { get; }

        //public bool ContainsIsDeleteable { get; private set; }
        public Dictionary<string, object> SelectParameters = new Dictionary<string, object>();

        public LambdaToSql()
        {
            EntityMapping = EntityMappingEngine.GetMappingForEntity<TEntity>();
        }

        public string GenerateWhere(Expression<Func<TEntity, bool>> expression)
        {
            var result = ResolveQuery((dynamic)expression.Body);
            var sql = new SqlBuilder().GenerateSql(result, out SelectParameters);

            return sql;
        }


        #region resolver

        private static void ResolveQuery(Expression expression)
        {
            throw new NotSupportedException($"{expression.NodeType} is an unsupported expression");
        }

        private static Node ResolveQuery(ConstantExpression constantExpression)
        {
            return new ValueNode() { Value = constantExpression.Value };
        }

        private Node ResolveQuery(UnaryExpression unaryExpression)
        {
            return new SingleOperationNode()
            {
                Operator = unaryExpression.NodeType,
                Child = ResolveQuery((dynamic)unaryExpression.Operand)
            };
        }

        private Node ResolveQuery(BinaryExpression binaryExpression)
        {
            return new OperationNode
            {
                Left = ResolveQuery((dynamic)binaryExpression.Left),
                Operator = binaryExpression.NodeType,
                Right = ResolveQuery((dynamic)binaryExpression.Right)
            };
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

        private Node ResolveQuery(MethodCallExpression callExpression)
        {
            LikeMethod callFunction;
            if (Enum.TryParse(callExpression.Method.Name, true, out callFunction))
            {
                var fieldValue = (string)GetExpressionValue(callExpression.Arguments.First());

                var property = typeof(TEntity).GetProperty(GetMemberExpression(callExpression.Object).Member.Name);
                var column = EntityMapping.PropertyMappings.FirstOrDefault(x => x.GetType() == property.GetType());

                return new LikeNode()
                {
                    MemberNode = new MemberNode()
                    {
                        TableName = EntityMapping.TableName,
                        FieldName = column != null ? column.ColumnName : property.Name
                    },
                    Method = callFunction,
                    Value = fieldValue
                };
            }
            else
            {
                var value = ResolveMethodCall(callExpression);
                return new ValueNode() { Value = value };
            }
        }

        private Node ResolveQuery(MemberExpression memberExpression, MemberExpression rootExpression = null)
        {
            rootExpression = rootExpression ?? memberExpression;
            
            if (memberExpression.Expression == null)
            {
                return new ValueNode {Value =  Expression.Lambda(memberExpression).Compile().DynamicInvoke() };
            }

            switch (memberExpression.Expression.NodeType)
            {
                case ExpressionType.Parameter:

                    var property = typeof(TEntity).GetProperty(GetMemberExpression(rootExpression).Member.Name);
                    var column = EntityMapping.PropertyMappings.FirstOrDefault(x => x.GetType() == property.GetType());

                    return new MemberNode()
                    {
                        TableName = EntityMapping.TableName,
                        FieldName = column != null ? column.ColumnName : property.Name
                    };
                case ExpressionType.MemberAccess:
                    return ResolveQuery(memberExpression.Expression as MemberExpression, rootExpression);
                case ExpressionType.Call:
                case ExpressionType.Constant:
                    return new ValueNode() { Value = GetExpressionValue(rootExpression) };
                default:
                    throw new NotSupportedException($"{memberExpression.Expression.NodeType} is an unsupported expression");
            }
        }

        #region Helpers

        private object GetExpressionValue(Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Constant:
                    var constantExpression = expression as ConstantExpression;
                    return constantExpression?.Value;
                case ExpressionType.Call:
                    return ResolveMethodCall(expression as MethodCallExpression);
                case ExpressionType.MemberAccess:
                    var memberExpr = (expression as MemberExpression);
                    return memberExpr != null ? ResolveValue((dynamic)memberExpr.Member, GetExpressionValue(memberExpr.Expression)) : null;
                default:
                    throw new NotSupportedException($"{expression.NodeType} is an unsupported expression");
            }
        }

        private object ResolveMethodCall(MethodCallExpression callExpression)
        {
            var arguments = callExpression.Arguments.Select(GetExpressionValue).ToArray();
            var obj = callExpression.Object != null ? GetExpressionValue(callExpression.Object) : arguments.First();

            return callExpression.Method.Invoke(obj, arguments);
        }

        private static object ResolveValue(PropertyInfo property, object obj)
        {
            return property.GetValue(obj, null);
        }

        private static object ResolveValue(FieldInfo field, object obj)
        {
            return field.GetValue(obj);
        }






        #endregion
        #endregion resolver
    }
}
