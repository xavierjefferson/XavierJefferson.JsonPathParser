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
            if (right is LogicalExpressionNode rightLogicalExpressionNode &&
                rightLogicalExpressionNode.Operator == LogicalOperator.And)
                return rightLogicalExpressionNode.Append(left);

            return LogicalExpressionNode.CreateLogicalAnd(left, right);
        }

        if (right is LogicalExpressionNode rln && rln.Operator == LogicalOperator.Or) return rln.Append(left);

        return LogicalExpressionNode.CreateLogicalOr(left, right);
    }
}