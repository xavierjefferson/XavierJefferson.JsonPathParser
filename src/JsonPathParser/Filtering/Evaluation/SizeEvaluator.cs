using XavierJefferson.JsonPathParser.Filtering.ValueNodes;
using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Filtering.Evaluation;

public class SizeEvaluator : IEvaluator
{
    public bool Evaluate(ValueNode left, ValueNode right, IPredicateContext ctx)
    {
        if (right is NumberNode)
        {
            var expectedSize = Convert.ToInt32(right.AsNumberNode().Value);

            if (left is StringNode)
                return left.AsStringNode().Length() == expectedSize;
            if (left is JsonNode) return left.AsJsonNode().Length(ctx) == expectedSize;
            return false;
        }

        return false;
    }
}