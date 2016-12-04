using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using Debonair.Data;
using Debonair.Tests.MockObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Debonair.Tests.Data
{
    [TestClass]
    public class DataRepositoryTests
    {
        readonly IDataRepository<DeleteableTestObject> repo = new DataRepository<DeleteableTestObject>(new SqlConnection(ConfigurationManager.ConnectionStrings[0].ConnectionString));

        [TestMethod]
        public void SelectTest()
        {
          var results = repo.Select(x=>x.CreatedDate == DateTime.UtcNow);
         }

        [TestMethod]
        public void StoredProcedureTest()
        {
            var results = repo.ExecuteStoredProcedure("dbo.spName", new {Id = 1, CustomerName = "Joe Bloggs"});
        }
    }
}
