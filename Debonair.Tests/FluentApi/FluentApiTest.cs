using System.Linq;
using Xunit;

namespace Debonair.Tests.FluentApi
{
    public class FluentApiTest
    {
        [Fact]
        public void MappingTest()
        {
            var mapping = new TestObjectMapping();

            Assert.True(mapping.PropertyMappings.Any());
            Assert.True(mapping.PropertyMappings.Count == 6);
        }
    }
}
