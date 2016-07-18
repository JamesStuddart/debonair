using System.Data.SqlClient;
using Debonair.Data;
using Debonair.Data.Context;
using Debonair.Data.Orm;

namespace Debonair.Tests.Local.MockObjects
{
    public class PersonRepository : DataRepository<Person>
    {
        public PersonRepository(SqlConnection sqlConnection, ICrudGenerator<Person> generator = null, IContext context = null) : base(sqlConnection, generator, context)
        {
        }
    }
}
