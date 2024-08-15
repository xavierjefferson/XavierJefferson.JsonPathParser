using XavierJefferson.JsonPathParser.Filtering.ValueNodes;
using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Filtering.Evaluation;

public abstract class CompareEvaluator : IEvaluator
{
    protected abstract List<int> CompareValues { get; }

    public virtual bool Evaluate(ValueNode left, ValueNode right, IPredicateContext context)
    {
        if (left is JsonNode leftJsonNode && right is JsonNode rightJsonNode) return leftJsonNode.Equals(rightJsonNode, context);

        if (left is NumberNode leftNumberNode && right is NumberNode rightNumberNode)
            return CompareValues.Contains(leftNumberNode.Value
                .CompareTo(rightNumberNode.Value));
        if (left is StringNode leftStringNode && right is StringNode rightStringNode)
            return CompareValues.Contains(leftStringNode.Value
                .CompareTo(rightStringNode.Value));
        if (left is DateTimeOffsetNode leftDateTimeOffsetNode && right is DateTimeOffsetNode rightDateTimeOffsetNode)
            //workaround for issue: https://github.com/json-path/JsonPath/issues/613
            return CompareValues.Contains(leftDateTimeOffsetNode.Value
                .CompareTo(rightDateTimeOffsetNode.Value));
        if (left is IComparable c && right is IComparable d) return CompareValues.Contains(c.CompareTo(d));
        if (CompareValues.Contains(0)) return left.Equals(right);
        return false;
    }
}