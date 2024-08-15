using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Filtering.ValueNodes;

public abstract class ExpressionNode : IPredicate
{
    public abstract bool Apply(IPredicateContext context);

    public abstract string ToUnenclosedString();
    public static ExpressionNode CreateExpressionNode(ExpressionNode right, LogicalOperator @operator,
        ExpressionNode left)
    {
        if (@operator == LogicalOperator.And)
        {
            if (right is LogicalExpressionNode && ((LogicalExpressionNode)right).Operator == LogicalOperator.And)
            {
                var len = (LogicalExpressionNode)right;
                return len.Append(left);
            }

            return LogicalExpressionNode.CreateLogicalAnd(left, right);
        }

        if (right is LogicalExpressionNode && ((LogicalExpressionNode)right).Operator == LogicalOperator.Or)
        {
            var len = (LogicalExpressionNode)right;
            return len.Append(left);
        }

        return LogicalExpressionNode.CreateLogicalOr(left, right);
    }
}