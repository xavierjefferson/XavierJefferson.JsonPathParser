using XavierJefferson.JsonPathParser.Filtering.ValueNodes;
using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Filtering.Evaluation;

public class SubsetOfEvaluator : IEvaluator
{
    public bool Evaluate(ValueNode left, ValueNode right, IPredicateContext ctx)
    {
        ValueListNode rightValueListNode;
        if (right is JsonNode)
        {
            var vn = right.AsJsonNode().AsValueListNode(ctx);
            if (vn is UndefinedNode)
                return false;
            rightValueListNode = vn.AsValueListNode();
        }
        else
        {
            rightValueListNode = right.AsValueListNode();
        }

        ValueListNode leftValueListNode;
        if (left is JsonNode)
        {
            var vn = left.AsJsonNode().AsValueListNode(ctx);
            if (vn is UndefinedNode)
                return false;
            leftValueListNode = vn.AsValueListNode();
        }
        else
        {
            leftValueListNode = left.AsValueListNode();
        }

        return leftValueListNode.Subsetof(rightValueListNode);
    }
}