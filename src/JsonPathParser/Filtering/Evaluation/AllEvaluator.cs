using XavierJefferson.JsonPathParser.Filtering.ValueNodes;
using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Filtering.Evaluation;

public class AllEvaluator : IEvaluator
{
    public bool Evaluate(ValueNode left, ValueNode right, IPredicateContext ctx)
    {
        var requiredValues = right.AsValueListNode();

        if (left is JsonNode)
        {
            var valueNode =
                left.AsJsonNode().AsValueListNode(ctx); //returns UndefinedNode if conversion is not possible
            if (valueNode is ValueListNode)
            {
                var shouldContainAll = valueNode.AsValueListNode();
                foreach (var required in requiredValues)
                    if (!Enumerable.Contains(shouldContainAll, required))
                        return false;
            }

            return true;
        }

        return false;
    }
}