using System;
using System.Linq;
using Debonair.Data.Orm;
using Debonair.Provider.MsSql.Data.Orm;
using Debonair.Tests.MockObjects;
using Xunit;

namespace Debonair.Tests.Data.Orm
{
    public class MsSqlCrudGeneratorTests
    {
        private readonly ICrudGenerator<DeleteableTestObject> _sqlGenerator = new MsSqlCrudGenerator<DeleteableTestObject>();

        [Fact]
        public void SelectAll()
        {
            var sql = _sqlGenerator.Select();
            Assert.False(_sqlGenerator.SelectParameters.Any());
            Assert.Equal(sql,
                "SELECT [DeletetableTestTable].[ClientName] AS [CustomerName], [DeletetableTestTable].[CreatedDate], [DeletetableTestTable].[IsActive], [DeletetableTestTable].[IsDeleted] FROM [DeletetableTestSchema].[DeletetableTestTable] WITH (NOLOCK)");
        }

        [Fact]
        public void SelectByInt()
        {
            var sql = _sqlGenerator.Select(x => x.Id == 1);

            Assert.True(_sqlGenerator.SelectParameters.Any());
            Assert.Equal(1,_sqlGenerator.SelectParameters.Count());
            Assert.True(_sqlGenerator.SelectParameters.First().Key == "Id");
            Assert.True(_sqlGenerator.SelectParameters.First().Value.ToString() == "1");
            Assert.Equal(sql,
                "SELECT [DeletetableTestTable].[ClientName] AS [CustomerName], [DeletetableTestTable].[CreatedDate], [DeletetableTestTable].[IsActive], [DeletetableTestTable].[IsDeleted] FROM [DeletetableTestSchema].[DeletetableTestTable] WITH (NOLOCK) WHERE [DeletetableTestTable].[Id] = @Id");
        }

        [Fact]
        public void SelectByString()
        {
            var sql = _sqlGenerator.Select(x => x.CustomerName == "'; DROP TABLE Users;--");
            Assert.True(_sqlGenerator.SelectParameters.Any());
            Assert.Equal(1, _sqlGenerator.SelectParameters.Count());
            Assert.True(_sqlGenerator.SelectParameters.First().Key == "CustomerName");
            Assert.True(_sqlGenerator.SelectParameters.First().Value.ToString() == "''; DROP TABLE Users;--'");
            Assert.Equal(sql,
                "SELECT [DeletetableTestTable].[ClientName] AS [CustomerName], [DeletetableTestTable].[CreatedDate], [DeletetableTestTable].[IsActive], [DeletetableTestTable].[IsDeleted] FROM [DeletetableTestSchema].[DeletetableTestTable] WITH (NOLOCK) WHERE [DeletetableTestTable].[CustomerName] = @CustomerName");
        }

        [Fact]
        public void SelectByBoolTrue()
        {
            var sql = _sqlGenerator.Select(x => x.IsActive);
            Assert.True(_sqlGenerator.SelectParameters.Any());
            Assert.Equal(1, _sqlGenerator.SelectParameters.Count());
            Assert.True(_sqlGenerator.SelectParameters.First().Key == "IsActive");
            Assert.True(_sqlGenerator.SelectParameters.First().Value.ToString() == "1");
            Assert.Equal(sql,
                "SELECT [DeletetableTestTable].[ClientName] AS [CustomerName], [DeletetableTestTable].[CreatedDate], [DeletetableTestTable].[IsActive], [DeletetableTestTable].[IsDeleted] FROM [DeletetableTestSchema].[DeletetableTestTable] WITH (NOLOCK) WHERE [DeletetableTestTable].[IsActive] = @IsActive");
        }

        [Fact]
        public void SelectByBoolFalse()
        {
            var sql = _sqlGenerator.Select(x => !x.IsActive);
            Assert.True(_sqlGenerator.SelectParameters.Any());
            Assert.Equal(1, _sqlGenerator.SelectParameters.Count());
            Assert.True(_sqlGenerator.SelectParameters.First().Key == "IsActive");
            Assert.True(_sqlGenerator.SelectParameters.First().Value.ToString() == "1");
            Assert.Equal(sql,
                "SELECT [DeletetableTestTable].[ClientName] AS [CustomerName], [DeletetableTestTable].[CreatedDate], [DeletetableTestTable].[IsActive], [DeletetableTestTable].[IsDeleted] FROM [DeletetableTestSchema].[DeletetableTestTable] WITH (NOLOCK) WHERE  NOT [DeletetableTestTable].[IsActive] = @IsActive");
        }

        [Fact]
        public void SelectWhereEnum()
        {
            var sql = _sqlGenerator.Select(x => x.Status == TestStatus.Dead);
            Assert.True(_sqlGenerator.SelectParameters.Any());
            Assert.Equal(1, _sqlGenerator.SelectParameters.Count());
            Assert.True(_sqlGenerator.SelectParameters.First().Key == "Status");
            Assert.True(_sqlGenerator.SelectParameters.First().Value.ToString() == "200");
            Assert.Equal(sql,
                "SELECT [DeletetableTestTable].[ClientName] AS [CustomerName], [DeletetableTestTable].[CreatedDate], [DeletetableTestTable].[IsActive], [DeletetableTestTable].[IsDeleted] FROM [DeletetableTestSchema].[DeletetableTestTable] WITH (NOLOCK) WHERE [DeletetableTestTable].[Status] = @Status");
        }

        [Fact]
        public void SelectWhereNotEnum()
        {
            var sql = _sqlGenerator.Select(x => x.Status != TestStatus.Dead);
            Assert.True(_sqlGenerator.SelectParameters.Any());
            Assert.Equal(1, _sqlGenerator.SelectParameters.Count());
            Assert.True(_sqlGenerator.SelectParameters.First().Key == "Status");
            Assert.True(_sqlGenerator.SelectParameters.First().Value.ToString() == "200");
            Assert.Equal(sql,
                "SELECT [DeletetableTestTable].[ClientName] AS [CustomerName], [DeletetableTestTable].[CreatedDate], [DeletetableTestTable].[IsActive], [DeletetableTestTable].[IsDeleted] FROM [DeletetableTestSchema].[DeletetableTestTable] WITH (NOLOCK) WHERE [DeletetableTestTable].[Status] != @Status");
        }

        [Fact]
        public void SelectWhereLike()
        {
            var sql = _sqlGenerator.Select(x => x.CustomerName.Contains("Smith"));
            Assert.True(_sqlGenerator.SelectParameters.Any());
            Assert.Equal(1, _sqlGenerator.SelectParameters.Count());
            Assert.True(_sqlGenerator.SelectParameters.First().Key == "CustomerName");
            Assert.True(_sqlGenerator.SelectParameters.First().Value.ToString() == "'%Smith%'");
            Assert.Equal(sql,
                "SELECT [DeletetableTestTable].[ClientName] AS [CustomerName], [DeletetableTestTable].[CreatedDate], [DeletetableTestTable].[IsActive], [DeletetableTestTable].[IsDeleted] FROM [DeletetableTestSchema].[DeletetableTestTable] WITH (NOLOCK) WHERE [DeletetableTestTable].[CustomerName] LIKE @CustomerName");
        }

        [Fact]
        public void SelectWhereNotLike()
        {
            var sql = _sqlGenerator.Select(x => !x.CustomerName.Contains("Bloggs"));
            Assert.True(_sqlGenerator.SelectParameters.Any());
            Assert.Equal(1, _sqlGenerator.SelectParameters.Count());
            Assert.True(_sqlGenerator.SelectParameters.First().Key == "CustomerName");
            Assert.True(_sqlGenerator.SelectParameters.First().Value.ToString() == "'%Bloggs%'");
            Assert.Equal(sql,
                "SELECT [DeletetableTestTable].[ClientName] AS [CustomerName], [DeletetableTestTable].[CreatedDate], [DeletetableTestTable].[IsActive], [DeletetableTestTable].[IsDeleted] FROM [DeletetableTestSchema].[DeletetableTestTable] WITH (NOLOCK) WHERE  NOT [DeletetableTestTable].[CustomerName] LIKE @CustomerName");
        }

        [Fact]
        public void SelectWhereStartsWith()
        {
            var sql = _sqlGenerator.Select(x => x.CustomerName.StartsWith("Joe"));
            Assert.True(_sqlGenerator.SelectParameters.Any());
            Assert.Equal(1, _sqlGenerator.SelectParameters.Count());
            Assert.True(_sqlGenerator.SelectParameters.First().Key == "CustomerName");
            Assert.True(_sqlGenerator.SelectParameters.First().Value.ToString() == "'Joe%'");
            Assert.Equal(sql,
                "SELECT [DeletetableTestTable].[ClientName] AS [CustomerName], [DeletetableTestTable].[CreatedDate], [DeletetableTestTable].[IsActive], [DeletetableTestTable].[IsDeleted] FROM [DeletetableTestSchema].[DeletetableTestTable] WITH (NOLOCK) WHERE [DeletetableTestTable].[CustomerName] LIKE @CustomerName");
        }

        [Fact]
        public void SelectWhereNotStartsWith()
        {
            var sql = _sqlGenerator.Select(x => !x.CustomerName.StartsWith("Joe"));
            Assert.True(_sqlGenerator.SelectParameters.Any());
            Assert.Equal(1, _sqlGenerator.SelectParameters.Count());
            Assert.True(_sqlGenerator.SelectParameters.First().Key == "CustomerName");
            Assert.True(_sqlGenerator.SelectParameters.First().Value.ToString() == "'Joe%'");
            Assert.Equal(sql,
                "SELECT [DeletetableTestTable].[ClientName] AS [CustomerName], [DeletetableTestTable].[CreatedDate], [DeletetableTestTable].[IsActive], [DeletetableTestTable].[IsDeleted] FROM [DeletetableTestSchema].[DeletetableTestTable] WITH (NOLOCK) WHERE  NOT [DeletetableTestTable].[CustomerName] LIKE @CustomerName");
        }

        [Fact]
        public void SelectWhereEndsWith()
        {
            var sql = _sqlGenerator.Select(x => x.CustomerName.EndsWith("Bloggs"));
            Assert.True(_sqlGenerator.SelectParameters.Any());
            Assert.Equal(1, _sqlGenerator.SelectParameters.Count());
            Assert.True(_sqlGenerator.SelectParameters.First().Key == "CustomerName");
            Assert.True(_sqlGenerator.SelectParameters.First().Value.ToString() == "'%Bloggs'");
            Assert.Equal(sql,
                "SELECT [DeletetableTestTable].[ClientName] AS [CustomerName], [DeletetableTestTable].[CreatedDate], [DeletetableTestTable].[IsActive], [DeletetableTestTable].[IsDeleted] FROM [DeletetableTestSchema].[DeletetableTestTable] WITH (NOLOCK) WHERE [DeletetableTestTable].[CustomerName] LIKE @CustomerName");
        }

        [Fact]
        public void SelectWhereNotEndsWith()
        {
            var sql = _sqlGenerator.Select(x => !x.CustomerName.EndsWith("Bloggs"));
            Assert.True(_sqlGenerator.SelectParameters.Any());
            Assert.Equal(1, _sqlGenerator.SelectParameters.Count());
            Assert.True(_sqlGenerator.SelectParameters.First().Key == "CustomerName");
            Assert.True(_sqlGenerator.SelectParameters.First().Value.ToString() == "'%Bloggs'");
            Assert.Equal(sql,
                "SELECT [DeletetableTestTable].[ClientName] AS [CustomerName], [DeletetableTestTable].[CreatedDate], [DeletetableTestTable].[IsActive], [DeletetableTestTable].[IsDeleted] FROM [DeletetableTestSchema].[DeletetableTestTable] WITH (NOLOCK) WHERE  NOT [DeletetableTestTable].[CustomerName] LIKE @CustomerName");

        }

        [Fact]
        public void SelectWhereIsNull()
        {
            var sql = _sqlGenerator.Select(x => x.CustomerName == null);
            Assert.True(!_sqlGenerator.SelectParameters.Any());
            Assert.Equal(sql,
                "SELECT [DeletetableTestTable].[ClientName] AS [CustomerName], [DeletetableTestTable].[CreatedDate], [DeletetableTestTable].[IsActive], [DeletetableTestTable].[IsDeleted] FROM [DeletetableTestSchema].[DeletetableTestTable] WITH (NOLOCK) WHERE [DeletetableTestTable].[CustomerName] IS NULL");
        }

        [Fact]
        public void SelectWhereIsNotNull()
        {
            var sql = _sqlGenerator.Select(x => x.CustomerName != null);
            Assert.Equal(sql,
                "SELECT [DeletetableTestTable].[ClientName] AS [CustomerName], [DeletetableTestTable].[CreatedDate], [DeletetableTestTable].[IsActive], [DeletetableTestTable].[IsDeleted] FROM [DeletetableTestSchema].[DeletetableTestTable] WITH (NOLOCK) WHERE [DeletetableTestTable].[CustomerName] IS NOT NULL");
        }

        [Fact]
        public void SelectWhereDateTimeHasValue()
        {
            var sql = _sqlGenerator.Select(x => x.CreatedDate == null);
            Assert.True(!_sqlGenerator.SelectParameters.Any());
            Assert.Equal(sql,
                "SELECT [DeletetableTestTable].[ClientName] AS [CustomerName], [DeletetableTestTable].[CreatedDate], [DeletetableTestTable].[IsActive], [DeletetableTestTable].[IsDeleted] FROM [DeletetableTestSchema].[DeletetableTestTable] WITH (NOLOCK) WHERE [DeletetableTestTable].[CreatedDate] IS NULL");
        }

        [Fact]
        public void SelectWhereDateTimeDoesNotHasValue()
        {
            var sql = _sqlGenerator.Select(x => x.CreatedDate != null);
            Assert.True(!_sqlGenerator.SelectParameters.Any());
            Assert.Equal(sql,
                "SELECT [DeletetableTestTable].[ClientName] AS [CustomerName], [DeletetableTestTable].[CreatedDate], [DeletetableTestTable].[IsActive], [DeletetableTestTable].[IsDeleted] FROM [DeletetableTestSchema].[DeletetableTestTable] WITH (NOLOCK) WHERE [DeletetableTestTable].[CreatedDate] IS NOT NULL");
        }

        [Fact]
        public void SelectWhereDateTimeEquals()
        {
            var sql = _sqlGenerator.Select(x => x.CreatedDate == DateTime.Parse("01/01/2016"));
            Assert.True(_sqlGenerator.SelectParameters.Any());
            Assert.Equal(1, _sqlGenerator.SelectParameters.Count());
            Assert.True(_sqlGenerator.SelectParameters.First().Key == "CreatedDate");
            Assert.True(_sqlGenerator.SelectParameters.First().Value.ToString() == "2016-01-01 00:00:00");
            Assert.Equal(sql,
                "SELECT [DeletetableTestTable].[ClientName] AS [CustomerName], [DeletetableTestTable].[CreatedDate], [DeletetableTestTable].[IsActive], [DeletetableTestTable].[IsDeleted] FROM [DeletetableTestSchema].[DeletetableTestTable] WITH (NOLOCK) WHERE [DeletetableTestTable].[CreatedDate] = @CreatedDate");
        }

        [Fact]
        public void Update()
        {
            var sql = _sqlGenerator.Update();
            Assert.Equal(sql,
                "UPDATE [DeletetableTestSchema].[DeletetableTestTable] SET [DeletetableTestTable].[ClientName] = @CustomerName, [DeletetableTestTable].[CreatedDate] = @CreatedDate, [DeletetableTestTable].[IsActive] = @IsActive, [DeletetableTestTable].[IsDeleted] = @IsDeleted WHERE [DeletetableTestTable].[] = @Id");
        }

        [Fact]
        public void Insert()
        {
            var sql = _sqlGenerator.Insert();
            Assert.Equal(sql.Replace(Environment.NewLine,string.Empty).Trim(),
                "INSERT INTO [DeletetableTestSchema].[DeletetableTestTable] ([DeletetableTestTable].[ClientName], [DeletetableTestTable].[CreatedDate], [DeletetableTestTable].[IsActive], [DeletetableTestTable].[IsDeleted])  VALUES (@CustomerName, @CreatedDate, @IsActive, @IsDeleted) DECLARE @NEWID intSET @NEWID = SCOPE_IDENTITY()SELECT @NEWID");
        }

        [Fact]
        public void Delete()
        {
            var sql = _sqlGenerator.Delete();
            Assert.Equal(sql,
                "DELETE FROM [DeletetableTestSchema].[DeletetableTestTable] WHERE [DeletetableTestTable].[Id] = @Id");
        }

        [Fact]
        public void DeleteForced()
        {
            var sql = _sqlGenerator.Delete(true);
            Assert.Equal(sql,
                "DELETE FROM [DeletetableTestSchema].[DeletetableTestTable] WHERE [DeletetableTestTable].[Id] = @Id");
        }

        [Fact]
        public void SelectWhereIsDeleted()
        {
            var sql = _sqlGenerator.Select(x => x.IsDeleted);
            Assert.True(_sqlGenerator.SelectParameters.Any());
            Assert.Equal(1, _sqlGenerator.SelectParameters.Count());
            Assert.True(_sqlGenerator.SelectParameters.First().Key == "IsDeleted");
            Assert.True(_sqlGenerator.SelectParameters.First().Value.ToString() == "1");
            Assert.Equal(sql,
                "SELECT [DeletetableTestTable].[ClientName] AS [CustomerName], [DeletetableTestTable].[CreatedDate], [DeletetableTestTable].[IsActive], [DeletetableTestTable].[IsDeleted] FROM [DeletetableTestSchema].[DeletetableTestTable] WITH (NOLOCK) WHERE [DeletetableTestTable].[IsDeleted] = @IsDeleted");
        }

        [Fact]
        public void SelectWhereIsNotDeleted()
        {
            var sql = _sqlGenerator.Select(x => !x.IsDeleted);
            Assert.True(_sqlGenerator.SelectParameters.Any());
            Assert.Equal(1, _sqlGenerator.SelectParameters.Count());
            Assert.True(_sqlGenerator.SelectParameters.First().Key == "IsDeleted");
            Assert.True(_sqlGenerator.SelectParameters.First().Value.ToString() == "1");
            Assert.Equal(sql,
                "SELECT [DeletetableTestTable].[ClientName] AS [CustomerName], [DeletetableTestTable].[CreatedDate], [DeletetableTestTable].[IsActive], [DeletetableTestTable].[IsDeleted] FROM [DeletetableTestSchema].[DeletetableTestTable] WITH (NOLOCK) WHERE  NOT [DeletetableTestTable].[IsDeleted] = @IsDeleted");
        }
    }
}
