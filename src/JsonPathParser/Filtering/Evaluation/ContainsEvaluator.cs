using XavierJefferson.JsonPathParser.Filtering.ValueNodes;
using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Filtering.Evaluation;

public class ContainsEvaluator : IEvaluator
{
    public bool Evaluate(ValueNode left, ValueNode right, IPredicateContext context)
    {
        if (left is StringNode leftStringNode && right is StringNode rightStringNode)
            return leftStringNode.Contains(rightStringNode.Value);

        if (left is JsonNode leftJsonNode)
        {
            var valueNode = leftJsonNode.AsValueListNode(context);
            if (valueNode is UndefinedNode) return false;

            var res = Enumerable.Contains(valueNode.AsValueListNode(), right);
            return res;
        }

        return false;
    }
}