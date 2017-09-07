using System.Collections.Generic;
using Debonair.Data.Orm.QueryBuilder.ExpressionTree;

namespace Debonair.Data.Context
{
    public interface ISqlBuilder
    {
        string GenerateSql(Node result, out Dictionary<string, object> selectParameters);
    }
}
