using Debonair.Entities;

namespace Debonair.Data.Orm.QueryBuilder.ExpressionTree
{
    public class LikeNode : Node
    {
        public LikeMethod Method { get; set; }
        public MemberNode MemberNode { get; set; }
        public string Value { get; set; }
    }
}
