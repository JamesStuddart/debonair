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
        public void Test()
        {
            var repo = new DataRepository<TestObject>(new SqlConnection());
            var results = repo.Select(x => x.CustomerName == "Joe Bloggs");
        }
    }
}
