using XavierJefferson.JsonPathParser.Filtering.ValueNodes;
using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Filtering.Evaluation;

public class EmptyEvaluator : IEvaluator
{
    public bool Evaluate(ValueNode left, ValueNode right, IPredicateContext context)
    {
        if (left is StringNode leftStringNode)
            return leftStringNode.IsEmpty() == right.AsBooleanNode().Value;
        if (left is JsonNode leftJsonNode) return leftJsonNode.IsEmpty(context) == right.AsBooleanNode().Value;
        return false;
    }
}