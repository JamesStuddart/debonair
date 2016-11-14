using Debonair.FluentApi;
using Debonair.Tests.MockObjects;

namespace Debonair.Tests.FluentApi
{
    public class TestObjectMapping : EntityMapping<TestObjectMapping>
    {
        public TestObjectMapping()
        {
            SetTableName("TestTable");
            SetSchemaName("TestSchema");
        }
    }

    public class DeleteableTestObjectMapping : EntityMapping<DeleteableTestObject>
    {
        public DeleteableTestObjectMapping()
        {
            SetTableName("DeletetableTestTable");
            SetSchemaName("DeletetableTestSchema");
            SetPrimaryKey(x => x.Id);
            SetIgnore(x => x.Status);
            SetColumnName(x => x.CustomerName, "ClientName");
            SetIsDeletedProperty(x => x.IsDeleted);
        }
    }
}
