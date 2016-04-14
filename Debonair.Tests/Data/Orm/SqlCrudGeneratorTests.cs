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
        }

        [TestMethod]
        public void SelectByInt()
        {
            var sql = sqlGenerator.Select(x => x.Id == 1);
        }

        [TestMethod]
        public void SelectByString()
        {
            var sql = sqlGenerator.Select(x => x.CustomerName == "'; DROP TABLE Users;--");
        }

        [TestMethod]
        public void SelectByBoolTrue()
        {
            var sql = sqlGenerator.Select(x => x.IsActive);
        }

        [TestMethod]
        public void SelectByBoolFalse()
        {
            var sql = sqlGenerator.Select(x => !x.IsActive);
        }

        [TestMethod]
        public void SelectWhereEnum()
        {
            var sql = sqlGenerator.Select(x => x.Status == TestStatus.dead);
        }

        [TestMethod]
        public void SelectWhereNotEnum()
        {
            var sql = sqlGenerator.Select(x => x.Status != TestStatus.dead);
        }

        [TestMethod]
        public void SelectWhereLike()
        {
            var sql = sqlGenerator.Select(x => x.CustomerName.Contains("Smith"));
        }

        [TestMethod]
        public void SelectWhereNotLike()
        {
            var sql = sqlGenerator.Select(x => !x.CustomerName.Contains("Bloggs"));
        }


        [TestMethod]
        public void SelectWhereStartsWith()
        {
            var sql = sqlGenerator.Select(x => x.CustomerName.StartsWith("Joe"));
        }

        [TestMethod]
        public void SelectWhereNotStartsWith()
        {
            var sql = sqlGenerator.Select(x => !x.CustomerName.StartsWith("Joe"));
        }

        [TestMethod]
        public void SelectWhereEndsWith()
        {
            var sql = sqlGenerator.Select(x => x.CustomerName.EndsWith("Bloggs"));
        }

        [TestMethod]
        public void SelectWhereNotEndsWith()
        {
            var sql = sqlGenerator.Select(x => !x.CustomerName.EndsWith("Bloggs"));
        }

        [TestMethod]
        public void SelectWhereIsNull()
        {
            var sql = sqlGenerator.Select(x => x.CustomerName == null); 
        }

        [TestMethod]
        public void SelectWhereIsNotNull()
        {
            var sql = sqlGenerator.Select(x => x.CustomerName != null); 
        }

        [TestMethod]
        public void Update()
        {
            var sql = sqlGenerator.Update(); 
        }

        [TestMethod]
        public void Insert()
        {
            var sql = sqlGenerator.Insert(); 
        }

        [TestMethod]
        public void Delete()
        {
            var sql = sqlGenerator.Delete(); 
        }

        [TestMethod]
        public void DeleteForced()
        {
            var sql = sqlGenerator.Delete(true); 
        }

        [TestMethod]
        public void SelectWhereIsDeleted()
        {
            var sql = sqlGenerator.Select(x => x.IsDeleted);
        }

        [TestMethod]
        public void SelectWhereIsNotDeleted()
        {
            var sql2 = sqlGenerator.Select(x => !x.IsDeleted);
        }
    }
}
