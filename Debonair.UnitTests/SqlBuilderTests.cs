using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;

using Debonair.Data.Orm.QueryBuilder;
using Debonair.Data.Orm.QueryBuilder.ExpressionTree;
using Debonair.Entities;

namespace Debonair.UnitTests
{
    [TestFixture]
    public class SqlBuilderTests
    {
        public class OperationNodeCase
        {
            [Test]
            public void GenerateSql_ThrowsRuntimeBuilderExceptionOperationNodeHasNullFields()
            {
                var sut = new SqlBuilder();
                Dictionary<string, object> parameters;
                var node = new OperationNode();
                Assert.Throws<Microsoft.CSharp.RuntimeBinder.RuntimeBinderException>(() => sut.GenerateSql(node, out parameters));
            }
            [Test]

            public void GenerateSql_throwsNullRefereceExceptionWhenPassedNull()
            {
                var sut = new SqlBuilder();
                Dictionary<string, object> parameters;
                Assert.Throws<NullReferenceException>(() => sut.GenerateSql(null, out parameters));
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
                Assert.That(parameters["field1"], Is.EqualTo("1"));
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
                Assert.That(parameters["field1"], Is.EqualTo("'irrelevant'"));
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
                Assert.That(parameters["field1"], Is.EqualTo("1.5"));
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
                Assert.That(parameters["field1"], Is.EqualTo("1"));
            }
        }

        public class MemberNodeCase
        {
            [Test]
            public void Test1()
            {
                var sut = new SqlBuilder();
                Dictionary<string, object> parameters;
                Assert.Throws<ArgumentNullException>(() => sut.GenerateSql(new MemberNode(), out parameters));
            }
            [Test]
            public void Test2()
            {
                var sut = new SqlBuilder();
                Dictionary<string, object> parameters;
                var member = new MemberNode()
                {
                    TableName = "",
                    FieldName = ""
                };
                var ex = Assert.Throws<ArgumentException>(() => sut.GenerateSql(member, out parameters));
                Assert.That(ex.Message, Is.EqualTo("Marformed Query = ' WHERE [].[] = @'"));
            }
            [Test]
            public void Test3()
            {
                var sut = new SqlBuilder();
                Dictionary<string, object> parameters;
                var member = new MemberNode
                {
                    TableName = "table1",
                    FieldName = ""
                };
                Assert.That(sut.GenerateSql(member, out parameters), Is.EqualTo(""));
                var ex = Assert.Throws<ArgumentException>(() => sut.GenerateSql(member, out parameters));
                Assert.That(ex.Message, Is.EqualTo("Marformed Query = ' WHERE [table1].[] = @'"));
            }
            [Test]
            public void Test4()
            {
                var sut = new SqlBuilder();
                Dictionary<string, object> parameters;
                var member = new MemberNode
                {
                    TableName = "table1",
                    FieldName = "field1"
                };
                Assert.That(sut.GenerateSql(member, out parameters), Is.EqualTo(" WHERE [table1].[field1] = @field1"));
                Assert.That(parameters.Count, Is.EqualTo(1));
                Assert.That(parameters["field1"], Is.EqualTo("1"));
            }
        }

        public class SingleOperationNodeCase
        {
            [Test]
            public void Test1()
            {
                var sut = new SqlBuilder();
                Dictionary<string, object> parameters;
                Assert.Throws<NullReferenceException>(() => sut.GenerateSql(new SingleOperationNode(), out parameters));
            }
            [Test]
            public void Test2()
            {
                var sut = new SqlBuilder();
                Dictionary<string, object> parameters;
                var node = new SingleOperationNode()
                {
                    Child = new MemberNode()
                };
                Assert.Throws<ArgumentNullException>(() => sut.GenerateSql(node, out parameters));
            }

            [Test]
            public void Test3()
            {
                var sut = new SqlBuilder();
                Dictionary<string, object> parameters;
                var node = new SingleOperationNode
                {
                    Child = new MemberNode
                    {
                        TableName = "table1",
                        FieldName = "field1"
                    }
                };
                var sql = sut.GenerateSql(node, out parameters);
                Assert.That(sql, Is.EqualTo(" WHERE [table1].[field1] = @field1"));
    Assert.That(parameters.Keys.First(), Is.EqualTo("field1"));
                Assert.That(parameters.Values.First(), Is.EqualTo("1"));
                    }
            
            [Test]
            public void Test4()
            {
                var sut = new SqlBuilder();
                Dictionary<string, object> parameters;
                var node = new SingleOperationNode()
                {
                    Operator = ExpressionType.Not,
                    Child = new MemberNode()
                    {
                        TableName = "table1",
                        FieldName = "field1"
                    }
                };
                var sql = sut.GenerateSql(node, out parameters);
                Assert.That(sql, Is.EqualTo(" WHERE  NOT [table1].[field1] = @field1"));
             Assert.That(parameters.Keys.First(), Is.EqualTo("field1"));
                Assert.That(parameters.Values.First(), Is.EqualTo("1"));
           }
            [Test]
            public void Test5()
            {
                var sut = new SqlBuilder();
                Dictionary<string, object> parameters;
                var node = new SingleOperationNode
                {
                    Child = new LikeNode()
                    {
                        MemberNode = new MemberNode()
                        {
                            TableName = "table1",
                            FieldName = "field1"
                        },
                        Value = "value"
,
                        Method = LikeMethod.Like
                    }
                };
                var sql = sut.GenerateSql(node, out parameters);
                Assert.That(sql, Is.EqualTo(" WHERE [table1].[field1] LIKE @field1"));
             Assert.That(parameters.Keys.First(), Is.EqualTo("field1"));
                Assert.That(parameters.Values.First(), Is.EqualTo("'%value%'"));
           }
            [Test]
            public void Test6()
            {
                var sut = new SqlBuilder();
                Dictionary<string, object> parameters;
                var node = new SingleOperationNode
                {
                    Child = new LikeNode()
                    {
                        MemberNode = new MemberNode()
                        {
                            TableName = "table1",
                            FieldName = "field1"
                        },
                        Value = "value"
,
                        Method = LikeMethod.Contains
                    }
                };
                var sql = sut.GenerateSql(node, out parameters);
                Assert.That(sql, Is.EqualTo(" WHERE [table1].[field1] LIKE @field1"));
            Assert.That(parameters.Keys.First(), Is.EqualTo("field1"));
                Assert.That(parameters.Values.First(), Is.EqualTo("'%value%'"));
            }
            [Test]
            public void Test7()
            {
                var sut = new SqlBuilder();
                Dictionary<string, object> parameters;
                var node = new SingleOperationNode
                {
                    Child = new LikeNode()
                    {
                        MemberNode = new MemberNode()
                        {
                            TableName = "table1",
                            FieldName = "field1"
                        },
                        Value = "value"
,
                        Method = LikeMethod.EndsWith
                    }
                };
                var sql = sut.GenerateSql(node, out parameters);
                Assert.That(sql, Is.EqualTo(" WHERE [table1].[field1] LIKE @field1"));
                Assert.That(parameters.Keys.First(), Is.EqualTo("field1"));
                Assert.That(parameters.Values.First(), Is.EqualTo("'%value'"));
            }
            [Test]
            public void Test8()
            {
                var sut = new SqlBuilder();
                Dictionary<string, object> parameters;
                var node = new SingleOperationNode
                {
                    Child = new LikeNode()
                    {
                        MemberNode = new MemberNode()
                        {
                            TableName = "table1",
                            FieldName = "field1"
                        },
                        Value = "value"
,
                        Method = LikeMethod.StartsWith
                    }
                };
                var sql = sut.GenerateSql(node, out parameters);
                Assert.That(sql, Is.EqualTo(" WHERE [table1].[field1] LIKE @field1"));
                Assert.That(parameters.Keys.First(), Is.EqualTo("field1"));
                Assert.That(parameters.Values.First(), Is.EqualTo("'value%'"));
            }
            [Test]
            public void Test9()
            {
                var sut = new SqlBuilder();
                Dictionary<string, object> parameters;
                var node = new SingleOperationNode
                {
                    Child = new LikeNode()
                    {
                        MemberNode = new MemberNode()
                        {
                            TableName = "table1",
                            FieldName = "field1"
                        },
                        Value = "value"
,
                        Method = LikeMethod.Equals
                    }
                };
                var sql = sut.GenerateSql(node, out parameters);
                Assert.That(sql, Is.EqualTo(" WHERE [table1].[field1] LIKE @field1"));
                  }
        }
        [Test]
        public void Test9()
        {
            var sut = new SqlBuilder();
            Dictionary<string, object> parameters;
            var node = new OperationNode
            {
                Left = new OperationNode
                {
                    Left = new LikeNode
                    {
                        Method = LikeMethod.Contains,
                        MemberNode = new MemberNode
                        {
                            TableName = "t1",
                            FieldName = "f1"
                        },
                        Value = "v1"
                    },
                    Right = new ValueNode
                    {
                        Value = "v2"
                    },
                    Operator = ExpressionType.And
                },
                Right = new MemberNode
                {
                    TableName = "t2",
                    FieldName = "f2"
                },Operator = ExpressionType.And

            };
            var sql = sut.GenerateSql(node, out parameters);
            Assert.That(sql, Is.EqualTo(" WHERE (([t1].[f1] LIKE @f1 AND ) AND [t2].[f2] = @f2)"));
        }
        [Test]
        public void Test10()
        {
            var sut = new SqlBuilder();
            Dictionary<string, object> parameters;
            var node = new OperationNode
            {
                Left = new OperationNode
                {
                    Left = new LikeNode
                    {
                        Method = LikeMethod.Contains,
                        MemberNode = new MemberNode
                        {
                            TableName = "t1",
                            FieldName = "f1"
                        },
                        Value = "v1"
                    },
                    Right = new ValueNode
                    {
                        Value = "v2"
                    },
                    Operator = ExpressionType.Equal
                },
                Right = new MemberNode
                {
                    TableName = "t2",
                    FieldName = "f2"
                },
                Operator = ExpressionType.And

            };
            var sql = sut.GenerateSql(node, out parameters);
            //problably wrong query
            Assert.That(sql, Is.EqualTo(string.Empty));
        }
        [Test]
        public void Test11()
        {
            var sut = new SqlBuilder();
            Dictionary<string, object> parameters;
            var node = new OperationNode
            {
                Left = new OperationNode
                {
                    Left = new LikeNode
                    {
                        Method = LikeMethod.Contains,
                        MemberNode = new MemberNode
                        {
                            TableName = "t1",
                            FieldName = "f1"
                        },
                        Value = "v1"
                    },
                    Right = new ValueNode
                    {
                        Value = "v2"
                    },
                    Operator = ExpressionType.Equal
                },
                Right = new MemberNode
                {
                    TableName = "t2",
                    FieldName = "f2"
                },
                Operator = ExpressionType.Equal

            };
            var sql = sut.GenerateSql(node, out parameters);
            //problably wrong query
            Assert.That(sql, Is.EqualTo(string.Empty));
        }
        [Test]
        public void Test12()
        {
            var sut = new SqlBuilder();
            Dictionary<string, object> parameters;
            var node = new OperationNode
            {
                Left = new MemberNode()
                {
                    TableName = "t1",
                    FieldName = "f1"
                },
                Right = new ValueNode()
                {
                    Value = "12"
                },
                Operator = ExpressionType.Equal

            };
            var sql = sut.GenerateSql(node, out parameters);
            //problably wrong query
            Assert.That(sql, Is.EqualTo(" WHERE [t1].[f1] = @f1"));
            Assert.That(parameters.Keys.First(),Is.EqualTo("f1"));
            Assert.That(parameters.Values.First(),Is.EqualTo("'12'"));
        }
    }
}
