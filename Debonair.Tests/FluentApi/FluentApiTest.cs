using System.Linq;
using Debonair.Data.Orm;
using Debonair.Tests.MockObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Debonair.Tests.FluentApi
{
    [TestClass]
    public class FluentApiTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var mapping = new TestObjectMapping();

            Assert.IsTrue(mapping.PropertyMappings.Any());
            Assert.IsTrue(mapping.PropertyMappings.Count == 1);
        }

    }
}
