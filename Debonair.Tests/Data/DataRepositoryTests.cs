using System.Data.SqlClient;
using Debonair.Data;
using Debonair.Tests.MockObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Debonair.Tests.Data
{
    [TestClass]
    public class DataRepositoryTests
    {

        [TestMethod]
        public void SelectTest()
        {
            var repo = new DataRepository<TestObject>(new SqlConnection());
            var results = repo.Select(x => x.CustomerName == "Joe Bloggs");
        }
        [TestMethod]
        public void StoredProcedureTest()
        {
            var repo = new DataRepository<TestObject>(new SqlConnection());
            var results = repo.ExecuteStoredProcedure<TestObject>("dbo.spName", new {Id = 1, CustomerName = "Joe Bloggs"});
        }
    }
}
