using System.Linq.Expressions;

namespace Debonair.Data.Orm.QueryBuilder.ExpressionTree
{
    class SingleOperationNode : Node
    {
        public ExpressionType Operator { get; set; }
        public Node Child { get; set; }
    }
}
