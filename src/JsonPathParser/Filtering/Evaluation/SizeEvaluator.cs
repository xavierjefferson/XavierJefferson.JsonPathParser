using XavierJefferson.JsonPathParser.Filtering.ValueNodes;
using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Filtering.Evaluation;

public class SizeEvaluator : IEvaluator
{
    public bool Evaluate(ValueNode left, ValueNode right, IPredicateContext context)
    {
        if (right is NumberNode numberNode)
        {
            var expectedSize = Convert.ToInt32(numberNode.Value);

            if (left is StringNode stringNode)
                return stringNode.Length() == expectedSize;
            if (left is JsonNode jsonNode) return jsonNode.Length(context) == expectedSize;
            return false;
        }

        return false;
    }
}