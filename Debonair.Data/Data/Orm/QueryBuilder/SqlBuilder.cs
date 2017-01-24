using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Debonair.Data.Orm.QueryBuilder.ExpressionTree;
using Debonair.Entities;

namespace Debonair.Data.Orm.QueryBuilder
{
    internal class SqlBuilder
    {
        private readonly StringBuilder conditions = new StringBuilder();
        private readonly Dictionary<string, object> sqlParameters = new Dictionary<string, object>();


        private readonly Dictionary<ExpressionType, string> operationDictionary = new Dictionary<ExpressionType, string>
        {
            {ExpressionType.Equal, "="},
            {ExpressionType.NotEqual, "!="},
            {ExpressionType.GreaterThan, ">"},
            {ExpressionType.LessThan, "<"},
            {ExpressionType.GreaterThanOrEqual, ">="},
            {ExpressionType.LessThanOrEqual, "<="}
        };

        public string GenerateSql(Node node, out Dictionary<string, object> parameters)
        {
            BuildSql((dynamic)node);
            AddWhere();
            parameters = sqlParameters;
            return conditions.ToString();
        }

        private void BuildSql(LikeNode node)
        {
            var value = string.Empty;

            switch (node.Method)
            {
                case LikeMethod.StartsWith:
                    value = node.Value + "%";
                    break;
                case LikeMethod.EndsWith:
                    value = "%" + node.Value;
                    break;
                case LikeMethod.Like:
                case LikeMethod.Contains:
                    value = "%" + node.Value + "%";
                    break;
                case LikeMethod.Equals:
                    QueryByField(node.MemberNode.TableName, node.MemberNode.FieldName, operationDictionary[ExpressionType.Equal], node.Value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            QueryByFieldLike(node.MemberNode.TableName, node.MemberNode.FieldName, value);
        }

        private void BuildSql(dynamic node)
        {
            var nodeType = node.GetType();

            if (nodeType == typeof(OperationNode))
            {
                BuildSql(node.Left, node.Right, node.Operator);
                return;
            }


            if (nodeType == typeof(MemberNode))
            {
                QueryByField(node.TableName, node.FieldName, operationDictionary[ExpressionType.Equal], true);

                return;
            }


            if (nodeType == typeof(SingleOperationNode))
            {
                if (node.Operator == ExpressionType.Not)
                    AddNot();
                BuildSql(node.Child);
            }
        }

        private void BuildSql(MemberNode memberNode, ValueNode valueNode, ExpressionType operatorType)
        {
            if (valueNode.Value == null)
            {
                ResolveNullValue(memberNode, operatorType);
            }
            else
            {
                QueryByField(memberNode.TableName, memberNode.FieldName, operationDictionary[operatorType],
                    valueNode.Value);
            }
        }

        private void BuildSql(ValueNode valueNode, MemberNode memberNode, ExpressionType operatorType)
        {
            BuildSql(memberNode, valueNode, operatorType);
        }

        private void BuildSql(MemberNode leftMember, MemberNode rightMember, ExpressionType operatorType)
        {
            QueryByFieldComparison(leftMember.TableName, leftMember.FieldName, operationDictionary[operatorType],
                rightMember.TableName, rightMember.FieldName);
        }

        private void BuildSql(SingleOperationNode leftMember, Node rightMember, ExpressionType operatorType)
        {
            if (leftMember.Operator == ExpressionType.Not)
                BuildSql(leftMember as Node, rightMember, operatorType);
            else
                BuildSql((dynamic)leftMember.Child, (dynamic)rightMember, operatorType);
        }

        private void BuildSql(Node leftMember, SingleOperationNode rightMember, ExpressionType operatorType)
        {
            BuildSql(rightMember, leftMember, operatorType);
        }

        private void BuildSql(Node leftNode, Node rightNode, ExpressionType operatorType)
        {
            BeginExpression();
            BuildSql((dynamic)leftNode);
            ResolveOperation(operatorType);
            BuildSql((dynamic)rightNode);
            EndExpression();
        }

        private void ResolveNullValue(MemberNode memberNode, ExpressionType op)
        {
            switch (op)
            {
                case ExpressionType.Equal:
                    QueryByFieldNull(memberNode.TableName, memberNode.FieldName);
                    break;
                case ExpressionType.NotEqual:
                    QueryByFieldNotNull(memberNode.TableName, memberNode.FieldName);
                    break;
            }
        }

        private void ResolveOperation(ExpressionType op)
        {
            switch (op)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    AddAnd();
                    break;
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    AddOr();
                    break;
                case ExpressionType.Equal:
                    AddEquals();
                    break;
                default:
                    throw new ArgumentException($"Unrecognized binary expression operation '{op}'");
            }
        }

        private void BeginExpression()
        {
            conditions.Append("(");
        }

        private void EndExpression()
        {
            conditions.Append(")");
        }

        private void AddAnd()
        {
            if (conditions.Length > 0)
                conditions.Append(" AND ");
        }

        private void AddOr()
        {
            if (conditions.Length > 0)
                conditions.Append(" OR ");
        }

        private void AddNot()
        {
            conditions.Append(" NOT ");
        }

        private void AddWhere()
        {
            conditions.Insert(0, " WHERE ");
        }

        private void AddEquals()
        {
            conditions.Insert(0, " = ");
        }

        private void QueryByField(string tableName, string fieldName, string op, object fieldValue)
        {
            sqlParameters.Add(fieldName, ParameterFormat(fieldValue));
            conditions.Append($"[{tableName}].[{fieldName}] {op} @{fieldName}");
        }


        private void QueryByFieldLike(string tableName, string fieldName, string fieldValue)
        {
            sqlParameters.Add(fieldName, ParameterFormat(fieldValue));
            conditions.Append($"[{tableName}].[{fieldName}] LIKE @{fieldName}");
        }

        private void QueryByFieldNull(string tableName, string fieldName)
        {
            conditions.Append($"[{tableName}].[{fieldName}] IS NULL");
        }

        private void QueryByFieldNotNull(string tableName, string fieldName)
        {
            conditions.Append($"[{tableName}].[{fieldName}] IS NOT NULL");
        }

        private void QueryByFieldComparison(string leftTableName, string leftFieldName, string op, string rightTableName,
            string rightFieldName)
        {

            conditions.Append($"[{leftTableName}].[{leftFieldName}] {op} {rightTableName}.{rightFieldName}");
        }

        private static string ParameterFormat(object parameter)
        {
            var returnValue = parameter.ToString();
            var paramType = parameter.GetType();

            switch (Type.GetTypeCode(paramType))
            {
                case TypeCode.String:
                    returnValue = $"'{parameter}'";
                    break;

                case TypeCode.Boolean:
                    returnValue = (((bool)parameter) ? 1 : 0).ToString();
                    break;

                case TypeCode.DateTime:
                    returnValue = ((DateTime)parameter).ToString("yyyy-MM-dd HH:mm:ss");
                    break;
            }

            return returnValue;
        }
    }
}