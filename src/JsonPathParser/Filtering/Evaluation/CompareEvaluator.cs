using XavierJefferson.JsonPathParser.Filtering.ValueNodes;
using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Filtering.Evaluation;

public abstract class CompareEvaluator : IEvaluator
{
    protected abstract List<int> CompareValues { get; }

    public virtual bool Evaluate(ValueNode left, ValueNode right, IPredicateContext ctx)
    {
        if (left is JsonNode && right is JsonNode) return left.AsJsonNode().Equals(right.AsJsonNode(), ctx);

        if (left is NumberNode && right is NumberNode)
            return CompareValues.Contains(left.AsNumberNode().Value
                .CompareTo(right.AsNumberNode().Value));
        if (left is StringNode && right is StringNode)
            return CompareValues.Contains(left.AsStringNode().Value
                .CompareTo(right.AsStringNode().Value));
        if (left is DateTimeOffsetNode && right is DateTimeOffsetNode)
            //workaround for issue: https://github.com/json-path/JsonPath/issues/613
            return CompareValues.Contains(left.AsDateTimeOffsetNode().Value
                .CompareTo(right.AsDateTimeOffsetNode().Value));
        if (left is IComparable c && right is IComparable d) return CompareValues.Contains(c.CompareTo(d));
        if (CompareValues.Contains(0)) return left.Equals(right);
        return false;
        //return CompareValues.Contains( left.Com.Equals(right);
    }
}