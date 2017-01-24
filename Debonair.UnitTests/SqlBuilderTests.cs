using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NUnit.Framework;

using Debonair.Data.Orm.QueryBuilder;
using Debonair.Data.Orm.QueryBuilder.ExpressionTree;

namespace Debonair.UnitTests
{
    [TestFixture]
    public class SqlBuilderTests
    {

        [Test]

        public void GenerateSql_throwsNullRefereceExceptionWhenPassedNull()
        {
            var sut = new SqlBuilder();
            Dictionary<string, object> parameters;
            Assert.Throws<NullReferenceException>(() => sut.GenerateSql(null, out parameters));
        }

        [Test]
        public void GenerateSql_ThrowsRuntimeBuilderExceptionOperationNodeHasNullFields()
        {
            var sut = new SqlBuilder();
            Dictionary<string, object> parameters;
            var node = new OperationNode();
            Assert.Throws<Microsoft.CSharp.RuntimeBinder.RuntimeBinderException> (() => sut.GenerateSql(node, out parameters));
        }

        [Test]
        public void GenerateSql_ThrowsMalformedQueryExceptionWhenMemberNodeAndValueNodeHaveNullProperties()
        {
            var sut = new SqlBuilder();
            Dictionary<string, object> parameters;
            var node = new OperationNode
            {
                Left = new MemberNode(),
                Right = new ValueNode(),
                Operator = ExpressionType.Equal
            };
            
            var ex = Assert.Throws<ArgumentException>(() => sut.GenerateSql(node, out parameters));
            Assert.That(ex.Message, Is.EqualTo("Marformed Query = ' WHERE[].[] IS NULL'"));
        }

        [Test]
        public void GenerateSql_ThrowsMalformedQueryExceptionWhenTableNameIsEmpty()
        {
            var sut = new SqlBuilder();
            Dictionary<string, object> parameters;
            var node = new OperationNode
            {
                Left = new MemberNode
                {
                    TableName = "",
                    FieldName = "field1"
                },
                Right = new ValueNode(),
                Operator = ExpressionType.Equal
            };
            
            var ex = Assert.Throws<ArgumentException>(() => sut.GenerateSql(node, out parameters));
            Assert.That(ex.Message, Is.EqualTo("Marformed Query = ' WHERE [].[field1] IS NULL'"));
        }

        [Test]
        public void GenerateSql_ReturnsWhereStatementForNullValue()
        {
            var sut = new SqlBuilder();
            Dictionary<string, object> parameters;
            var node = new OperationNode
            {
                Left = new MemberNode
                {
                    TableName = "table1",
                    FieldName = "field1"
                },
                Right = new ValueNode(),
                Operator = ExpressionType.Equal
            };

            var sql = sut.GenerateSql(node, out parameters);
            Assert.That(sql, Is.EqualTo(" WHERE [table1].[field1] IS NULL"));
        }

        [Test]
        public void GenerateSql_ReturnsWhereStatementForValueEqualTo1()
        {
            var sut = new SqlBuilder();
            Dictionary<string, object> parameters;
            var node = new OperationNode
            {
                Left = new MemberNode
                {
                    TableName = "table1",
                    FieldName = "field1"
                },
                Right = new ValueNode
                {
                    Value = 1
                },
                Operator = ExpressionType.Equal
            };

            var sql = sut.GenerateSql(node, out parameters);
            Assert.That(sql, Is.EqualTo(" WHERE [table1].[field1] = @field1"));
            Assert.That(parameters.Count, Is.EqualTo(1));
            Assert.That(parameters["field1"], Is.EqualTo(1));
        }

        [Test]
        public void GenerateSql_ReturnsWhereStatementForValueEqualToAString()
        {
            var sut = new SqlBuilder();
            Dictionary<string, object> parameters;
            var node = new OperationNode
            {
                Left = new MemberNode
                {
                    TableName = "table1",
                    FieldName = "field1"
                },
                Right = new ValueNode
                {
                    Value = "irrelevant"
                },
                Operator = ExpressionType.Equal
            };

            var sql = sut.GenerateSql(node, out parameters);
            Assert.That(sql, Is.EqualTo(" WHERE [table1].[field1] = @field1"));
            Assert.That(parameters.Count, Is.EqualTo(1));
            Assert.That(parameters["field1"], Is.EqualTo("irrelevant"));
        }

        [Test]
        public void GenerateSql_ReturnsWhereStatementForValueEqualToAFloat()
        {
            var sut = new SqlBuilder();
            Dictionary<string, object> parameters;
            var node = new OperationNode
            {
                Left = new MemberNode
                {
                    TableName = "table1",
                    FieldName = "field1"
                },
                Right = new ValueNode
                {
                    Value = 1.5
                },
                Operator = ExpressionType.Equal
            };

            var sql = sut.GenerateSql(node, out parameters);
            Assert.That(sql, Is.EqualTo(" WHERE [table1].[field1] = @field1"));
            Assert.That(parameters.Count, Is.EqualTo(1));
            Assert.That(parameters["field1"], Is.EqualTo(1.5));
        }

        [Test]
        public void GenerateSql_ReturnsWhereStatementForValueEqualToABoolean()
        {
            var sut = new SqlBuilder();
            Dictionary<string, object> parameters;
            var node = new OperationNode
            {
                Left = new MemberNode
                {
                    TableName = "table1",
                    FieldName = "field1"
                },
                Right = new ValueNode
                {
                    Value = true
                },
                Operator = ExpressionType.Equal
            };

            var sql = sut.GenerateSql(node, out parameters);
            Assert.That(sql, Is.EqualTo(" WHERE [table1].[field1] = @field1"));
            Assert.That(parameters.Count, Is.EqualTo(1));
            Assert.That(parameters["field1"], Is.EqualTo(true));
        }




















































    }
}
