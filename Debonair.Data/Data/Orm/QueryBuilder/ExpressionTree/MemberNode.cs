namespace Debonair.Data.Orm.QueryBuilder.ExpressionTree
{
    public class MemberNode : Node
    {
        public string TableName { get; set; }
        public string FieldName { get; set; }
    }
}
