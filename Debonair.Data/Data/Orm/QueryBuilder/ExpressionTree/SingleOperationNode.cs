using System.Linq.Expressions;

namespace Debonair.Data.Orm.QueryBuilder.ExpressionTree
{
    public class SingleOperationNode : Node
    {
        public ExpressionType Operator { get; set; }
        public Node Child { get; set; }
    }
}
