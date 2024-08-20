using XavierJefferson.JsonPathParser.Filtering.ValueNodes;
using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Filtering.Evaluation;

public class AllEvaluator : IEvaluator
{
    public bool Evaluate(ValueNode left, ValueNode right, IPredicateContext context)
    {
        var requiredValues = right.AsValueListNode();

        if (left is JsonNode jsonNode)
        {
            var valueNode =
                jsonNode.AsValueListNode(context); //returns UndefinedNode if conversion is not possible
            if (valueNode is ValueListNode valueListNode)
                foreach (var required in requiredValues)
                    if (!Enumerable.Contains(valueListNode, required))
                        return false;

            return true;
        }

        return false;
    }
}