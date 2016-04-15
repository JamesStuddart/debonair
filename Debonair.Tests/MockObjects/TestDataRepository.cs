using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Debonair.Data;
using Debonair.Data.Context;
using Debonair.Data.Orm;

namespace Debonair.Tests.MockObjects
{
    public class TestDataRepository: DataRepository<TestObject>
    {
        public TestDataRepository(SqlConnection sqlConnection, ICrudGenerator<TestObject> generator = null, IContext context = null) : base(sqlConnection, generator, context)
        {
        }
    }
}
