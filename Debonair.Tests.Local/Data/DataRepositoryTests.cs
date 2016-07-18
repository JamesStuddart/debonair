using System.Configuration;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using Debonair.Data;
using Debonair.Tests.Local.MockObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Debonair.Tests.Local.Data
{
    [TestClass]
    public class DataRepositoryTests
    {
       PersonRepository repo = new PersonRepository(new SqlConnection(ConfigurationManager.ConnectionStrings["AdventureWorks2014"].ConnectionString));
        [TestMethod]
        public void SelectTest()
        {
          var results = repo.Select(x=>x.Title != null);
         }

        [TestMethod]
        public void StoredProcedureTest()
        {
            var results = repo.ExecuteStoredProcedure("dbo.spName", new {Id = 1, CustomerName = "Joe Bloggs"});
        }
    }
}
