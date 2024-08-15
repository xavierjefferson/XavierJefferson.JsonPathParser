using XavierJefferson.JsonPathParser.Filtering.ValueNodes;
using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Filtering.Evaluation;

public class SubsetOfEvaluator : IEvaluator
{
    public bool Evaluate(ValueNode left, ValueNode right, IPredicateContext context)
    {
        ValueListNode rightValueListNode;
        if (right is JsonNode rightJsonNode)
        {
            var vn = rightJsonNode.AsValueListNode(context);
            if (vn is UndefinedNode)
                return false;
            rightValueListNode = vn.AsValueListNode();
        }
        else
        {
            rightValueListNode = right.AsValueListNode();
        }

        ValueListNode leftValueListNode;
        if (left is JsonNode leftJsonNode)
        {
            var vn = leftJsonNode.AsValueListNode(context);
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