using XavierJefferson.JsonPathParser.Filtering.ValueNodes;
using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Filtering.Evaluation;

public class InEvaluator : IEvaluator
{
    public virtual bool Evaluate(ValueNode left, ValueNode right, IPredicateContext ctx)
    {
        ValueListNode valueListNode;
        if (right is JsonNode)
        {
            var vn = right.AsJsonNode().AsValueListNode(ctx);
            if (vn is UndefinedNode)
                return false;
            valueListNode = vn.AsValueListNode();
        }
        else
        {
            valueListNode = right.AsValueListNode();
        }

        return Enumerable.Contains(valueListNode, left);
    }
}