using XavierJefferson.JsonPathParser.Filtering.ValueNodes;
using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Filtering.Evaluation;

public class ContainsEvaluator : IEvaluator
{
    public bool Evaluate(ValueNode left, ValueNode right, IPredicateContext ctx)
    {
        if (left is StringNode && right is StringNode)
            return left.AsStringNode().Contains(right.AsStringNode().Value);

        if (left is JsonNode)
        {
            var valueNode = left.AsJsonNode().AsValueListNode(ctx);
            if (valueNode is UndefinedNode) return false;

            var res = Enumerable.Contains(valueNode.AsValueListNode(), right);
            return res;
        }

        return false;
    }
}