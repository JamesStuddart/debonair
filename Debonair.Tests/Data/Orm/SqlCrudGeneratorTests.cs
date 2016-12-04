using System;
using Debonair.Data.Orm;
using Debonair.Tests.MockObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Debonair.Tests.Data.Orm
{

    [TestClass]
    public class SqlCrudGeneratorTests
    {
        private readonly ICrudGenerator<DeleteableTestObject> sqlGenerator = new SqlCrudGenerator<DeleteableTestObject>();

        [TestMethod]
        public void SelectAll()
        {
            var sql = sqlGenerator.Select();
            Assert.AreEqual(sql,
                "SELECT [DeletetableTestTable].[ClientName] AS [CustomerName], [DeletetableTestTable].[CreatedDate], [DeletetableTestTable].[IsActive], [DeletetableTestTable].[IsDeleted] FROM [DeletetableTestSchema].[DeletetableTestTable] WITH (NOLOCK)");
        }

        [TestMethod]
        public void SelectByInt()
        {
            var sql = sqlGenerator.Select(x => x.Id == 1);
            Assert.AreEqual(sql,
                "SELECT [DeletetableTestTable].[ClientName] AS [CustomerName], [DeletetableTestTable].[CreatedDate], [DeletetableTestTable].[IsActive], [DeletetableTestTable].[IsDeleted] FROM [DeletetableTestSchema].[DeletetableTestTable] WITH (NOLOCK) WHERE [DeletetableTestTable].[Id] = @Id");
        }

        [TestMethod]
        public void SelectByString()
        {
            var sql = sqlGenerator.Select(x => x.CustomerName == "'; DROP TABLE Users;--");
            Assert.AreEqual(sql,
                "SELECT [DeletetableTestTable].[ClientName] AS [CustomerName], [DeletetableTestTable].[CreatedDate], [DeletetableTestTable].[IsActive], [DeletetableTestTable].[IsDeleted] FROM [DeletetableTestSchema].[DeletetableTestTable] WITH (NOLOCK) WHERE [DeletetableTestTable].[CustomerName] = @CustomerName");
        }

        [TestMethod]
        public void SelectByBoolTrue()
        {
            var sql = sqlGenerator.Select(x => x.IsActive);
            Assert.AreEqual(sql,
                "SELECT [DeletetableTestTable].[ClientName] AS [CustomerName], [DeletetableTestTable].[CreatedDate], [DeletetableTestTable].[IsActive], [DeletetableTestTable].[IsDeleted] FROM [DeletetableTestSchema].[DeletetableTestTable] WITH (NOLOCK) WHERE [DeletetableTestTable].[IsActive] = @IsActive");
        }

        [TestMethod]
        public void SelectByBoolFalse()
        {
            var sql = sqlGenerator.Select(x => !x.IsActive);
            Assert.AreEqual(sql,
                "SELECT [DeletetableTestTable].[ClientName] AS [CustomerName], [DeletetableTestTable].[CreatedDate], [DeletetableTestTable].[IsActive], [DeletetableTestTable].[IsDeleted] FROM [DeletetableTestSchema].[DeletetableTestTable] WITH (NOLOCK) WHERE  NOT [DeletetableTestTable].[IsActive] = @IsActive");
        }

        [TestMethod]
        public void SelectWhereEnum()
        {
            var sql = sqlGenerator.Select(x => x.Status == TestStatus.dead);
            Assert.AreEqual(sql,
                "SELECT [DeletetableTestTable].[ClientName] AS [CustomerName], [DeletetableTestTable].[CreatedDate], [DeletetableTestTable].[IsActive], [DeletetableTestTable].[IsDeleted] FROM [DeletetableTestSchema].[DeletetableTestTable] WITH (NOLOCK) WHERE [DeletetableTestTable].[Status] = @Status");
        }

        [TestMethod]
        public void SelectWhereNotEnum()
        {
            var sql = sqlGenerator.Select(x => x.Status != TestStatus.dead);
            Assert.AreEqual(sql,
                "SELECT [DeletetableTestTable].[ClientName] AS [CustomerName], [DeletetableTestTable].[CreatedDate], [DeletetableTestTable].[IsActive], [DeletetableTestTable].[IsDeleted] FROM [DeletetableTestSchema].[DeletetableTestTable] WITH (NOLOCK) WHERE [DeletetableTestTable].[Status] != @Status");
        }

        [TestMethod]
        public void SelectWhereLike()
        {
            var sql = sqlGenerator.Select(x => x.CustomerName.Contains("Smith"));
            Assert.AreEqual(sql,
                "SELECT [DeletetableTestTable].[ClientName] AS [CustomerName], [DeletetableTestTable].[CreatedDate], [DeletetableTestTable].[IsActive], [DeletetableTestTable].[IsDeleted] FROM [DeletetableTestSchema].[DeletetableTestTable] WITH (NOLOCK) WHERE [DeletetableTestTable].[CustomerName] LIKE @CustomerName");
        }

        [TestMethod]
        public void SelectWhereNotLike()
        {
            var sql = sqlGenerator.Select(x => !x.CustomerName.Contains("Bloggs"));
            Assert.AreEqual(sql,
                "SELECT [DeletetableTestTable].[ClientName] AS [CustomerName], [DeletetableTestTable].[CreatedDate], [DeletetableTestTable].[IsActive], [DeletetableTestTable].[IsDeleted] FROM [DeletetableTestSchema].[DeletetableTestTable] WITH (NOLOCK) WHERE  NOT [DeletetableTestTable].[CustomerName] LIKE @CustomerName");
        }

        [TestMethod]
        public void SelectWhereStartsWith()
        {
            var sql = sqlGenerator.Select(x => x.CustomerName.StartsWith("Joe"));
            Assert.AreEqual(sql,
                "SELECT [DeletetableTestTable].[ClientName] AS [CustomerName], [DeletetableTestTable].[CreatedDate], [DeletetableTestTable].[IsActive], [DeletetableTestTable].[IsDeleted] FROM [DeletetableTestSchema].[DeletetableTestTable] WITH (NOLOCK) WHERE [DeletetableTestTable].[CustomerName] LIKE @CustomerName");
        }

        [TestMethod]
        public void SelectWhereNotStartsWith()
        {
            var sql = sqlGenerator.Select(x => !x.CustomerName.StartsWith("Joe"));
            Assert.AreEqual(sql,
                "SELECT [DeletetableTestTable].[ClientName] AS [CustomerName], [DeletetableTestTable].[CreatedDate], [DeletetableTestTable].[IsActive], [DeletetableTestTable].[IsDeleted] FROM [DeletetableTestSchema].[DeletetableTestTable] WITH (NOLOCK) WHERE  NOT [DeletetableTestTable].[CustomerName] LIKE @CustomerName");
        }

        [TestMethod]
        public void SelectWhereEndsWith()
        {
            var sql = sqlGenerator.Select(x => x.CustomerName.EndsWith("Bloggs"));
            Assert.AreEqual(sql,
                "SELECT [DeletetableTestTable].[ClientName] AS [CustomerName], [DeletetableTestTable].[CreatedDate], [DeletetableTestTable].[IsActive], [DeletetableTestTable].[IsDeleted] FROM [DeletetableTestSchema].[DeletetableTestTable] WITH (NOLOCK) WHERE [DeletetableTestTable].[CustomerName] LIKE @CustomerName");
        }

        [TestMethod]
        public void SelectWhereNotEndsWith()
        {
            var sql = sqlGenerator.Select(x => !x.CustomerName.EndsWith("Bloggs"));
            Assert.AreEqual(sql,
                "SELECT [DeletetableTestTable].[ClientName] AS [CustomerName], [DeletetableTestTable].[CreatedDate], [DeletetableTestTable].[IsActive], [DeletetableTestTable].[IsDeleted] FROM [DeletetableTestSchema].[DeletetableTestTable] WITH (NOLOCK) WHERE  NOT [DeletetableTestTable].[CustomerName] LIKE @CustomerName");

        }

        [TestMethod]
        public void SelectWhereIsNull()
        {
            var sql = sqlGenerator.Select(x => x.CustomerName == null);
            Assert.AreEqual(sql,
                "SELECT [DeletetableTestTable].[ClientName] AS [CustomerName], [DeletetableTestTable].[CreatedDate], [DeletetableTestTable].[IsActive], [DeletetableTestTable].[IsDeleted] FROM [DeletetableTestSchema].[DeletetableTestTable] WITH (NOLOCK) WHERE [DeletetableTestTable].[CustomerName] IS NULL");
        }

        [TestMethod]
        public void SelectWhereIsNotNull()
        {
            var sql = sqlGenerator.Select(x => x.CustomerName != null);
            Assert.AreEqual(sql,
                "SELECT [DeletetableTestTable].[ClientName] AS [CustomerName], [DeletetableTestTable].[CreatedDate], [DeletetableTestTable].[IsActive], [DeletetableTestTable].[IsDeleted] FROM [DeletetableTestSchema].[DeletetableTestTable] WITH (NOLOCK) WHERE [DeletetableTestTable].[CustomerName] IS NOT NULL");
        }

        [TestMethod]
        public void SelectWhereDateTimeHasValue()
        {
            var sql = sqlGenerator.Select(x => x.CreatedDate == null);

            Assert.AreEqual(sql,
                "SELECT [DeletetableTestTable].[ClientName] AS [CustomerName], [DeletetableTestTable].[CreatedDate], [DeletetableTestTable].[IsActive], [DeletetableTestTable].[IsDeleted] FROM [DeletetableTestSchema].[DeletetableTestTable] WITH (NOLOCK) WHERE [DeletetableTestTable].[CreatedDate] IS NULL");
        }

        [TestMethod]
        public void SelectWhereDateTimeDoesNotHasValue()
        {
            var sql = sqlGenerator.Select(x => x.CreatedDate != null);
            Assert.AreEqual(sql,
                "SELECT [DeletetableTestTable].[ClientName] AS [CustomerName], [DeletetableTestTable].[CreatedDate], [DeletetableTestTable].[IsActive], [DeletetableTestTable].[IsDeleted] FROM [DeletetableTestSchema].[DeletetableTestTable] WITH (NOLOCK) WHERE [DeletetableTestTable].[CreatedDate] IS NOT NULL");
        }

        [TestMethod]
        public void SelectWhereDateTimeEquals()
        {
            var sql = sqlGenerator.Select(x => x.CreatedDate == DateTime.UtcNow);
            Assert.AreEqual(sql,
                "SELECT [DeletetableTestTable].[ClientName] AS [CustomerName], [DeletetableTestTable].[CreatedDate], [DeletetableTestTable].[IsActive], [DeletetableTestTable].[IsDeleted] FROM [DeletetableTestSchema].[DeletetableTestTable] WITH (NOLOCK) WHERE [DeletetableTestTable].[CreatedDate] = @CreatedDate");
        }

        [TestMethod]
        public void Update()
        {
            var sql = sqlGenerator.Update();
            Assert.AreEqual(sql,
                "UPDATE [DeletetableTestSchema].[DeletetableTestTable] SET [DeletetableTestTable].[ClientName] = @CustomerName, [DeletetableTestTable].[] = @CreatedDate, [DeletetableTestTable].[] = @IsActive, [DeletetableTestTable].[] = @IsDeleted WHERE [DeletetableTestTable].[] = @Id");
        }

        [TestMethod]
        public void Insert()
        {
            var sql = sqlGenerator.Insert();
            Assert.AreEqual(sql.Replace(Environment.NewLine,string.Empty).Trim(),
                "INSERT INTO [DeletetableTestSchema].[DeletetableTestTable] ([DeletetableTestTable].[ClientName], [DeletetableTestTable].[], [DeletetableTestTable].[], [DeletetableTestTable].[])  VALUES (@CustomerName, @CreatedDate, @IsActive, @IsDeleted) DECLARE @NEWID intSET @NEWID = SCOPE_IDENTITY()SELECT @NEWID");
        }

        [TestMethod]
        public void Delete()
        {
            var sql = sqlGenerator.Delete();
            Assert.AreEqual(sql,
                "DELETE FROM [DeletetableTestSchema].[DeletetableTestTable] WHERE [DeletetableTestTable].[Id] = @Id");
        }

        [TestMethod]
        public void DeleteForced()
        {
            var sql = sqlGenerator.Delete(true);
            Assert.AreEqual(sql,
                "DELETE FROM [DeletetableTestSchema].[DeletetableTestTable] WHERE [DeletetableTestTable].[Id] = @Id");
        }

        [TestMethod]
        public void SelectWhereIsDeleted()
        {
            var sql = sqlGenerator.Select(x => x.IsDeleted);
            Assert.AreEqual(sql,
                "SELECT [DeletetableTestTable].[ClientName] AS [CustomerName], [DeletetableTestTable].[CreatedDate], [DeletetableTestTable].[IsActive], [DeletetableTestTable].[IsDeleted] FROM [DeletetableTestSchema].[DeletetableTestTable] WITH (NOLOCK) WHERE [DeletetableTestTable].[IsDeleted] = @IsDeleted");
        }

        [TestMethod]
        public void SelectWhereIsNotDeleted()
        {
            var sql = sqlGenerator.Select(x => !x.IsDeleted);
            Assert.AreEqual(sql,
                "SELECT [DeletetableTestTable].[ClientName] AS [CustomerName], [DeletetableTestTable].[CreatedDate], [DeletetableTestTable].[IsActive], [DeletetableTestTable].[IsDeleted] FROM [DeletetableTestSchema].[DeletetableTestTable] WITH (NOLOCK) WHERE  NOT [DeletetableTestTable].[IsDeleted] = @IsDeleted");
        }
    }
}
