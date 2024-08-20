using XavierJefferson.JsonPathParser.Filtering.ValueNodes;
using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Filtering.Evaluation;

public class NoneOfEvaluator : IEvaluator
{
    public bool Evaluate(ValueNode left, ValueNode right, IPredicateContext context)
    {
        ValueListNode rightValueListNode;
        if (right is JsonNode jsonNode)
        {
            var vn = jsonNode.AsValueListNode(context);
            if (vn is UndefinedNode)
                return false;
            rightValueListNode = vn.AsValueListNode();
        }
        else
        {
            rightValueListNode = right.AsValueListNode();
        }

        ValueListNode leftValueListNode;
        if (left is JsonNode jsonNode1)
        {
            var vn = jsonNode1.AsValueListNode(context);
            if (vn is UndefinedNode)
                return false;
            leftValueListNode = vn.AsValueListNode();
        }
        else
        {
            leftValueListNode = left.AsValueListNode();
        }

        foreach (var leftValueNode in leftValueListNode)
        foreach (var rightValueNode in rightValueListNode)
            if (leftValueNode.Equals(rightValueNode))
                return false;
        return true;
    }
}