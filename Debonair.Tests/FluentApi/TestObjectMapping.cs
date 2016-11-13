using Debonair.FluentApi;
using Debonair.Tests.MockObjects;

namespace Debonair.Tests.FluentApi
{
    public class TestObjectMapping : EntityMapping<DeleteableTestObject>
    {
        public TestObjectMapping()
        {
            SetTableName("TestTable");
            SetSchemaName("TestSchema");
            SetPrimaryKey(x => x.Id);
            SetIgnore(x => x.Status);
            SetColumnName(x => x.CustomerName, "Name");
            SetIsDeletedProperty(x => x.IsDeleted);
        }
    }
}
