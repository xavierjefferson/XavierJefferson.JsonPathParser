using XavierJefferson.JsonPathParser.Filtering.ValueNodes;
using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Filtering.Evaluation;

public class EmptyEvaluator : IEvaluator
{
    public bool Evaluate(ValueNode left, ValueNode right, IPredicateContext ctx)
    {
        if (left is StringNode)
            return left.AsStringNode().IsEmpty() == right.AsBooleanNode().Value;
        if (left is JsonNode) return left.AsJsonNode().IsEmpty(ctx) == right.AsBooleanNode().Value;
        return false;
    }
}