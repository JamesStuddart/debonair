using System.Linq.Expressions;

namespace Debonair.Data.Orm.QueryBuilder.ExpressionTree
{
    public class OperationNode : Node
    {
        public ExpressionType Operator { get; set; }
        public Node Left { get; set; }
        public Node Right { get; set; }
    }
}
