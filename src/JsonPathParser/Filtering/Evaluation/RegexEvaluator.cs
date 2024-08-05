using XavierJefferson.JsonPathParser.Filtering.ValueNodes;
using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Filtering.Evaluation;

public class RegexEvaluator : IEvaluator
{
    public bool Evaluate(ValueNode left, ValueNode right, IPredicateContext ctx)
    {
        if (!(left is PatternNode ^ right is PatternNode)) return false;

        if (left is PatternNode)
        {
            if (right is ValueListNode || (right is JsonNode && right.AsJsonNode().IsArray(ctx)))
                return MatchesAny(left.AsPatternNode(), right.AsJsonNode().AsValueListNode(ctx));
            return Matches(left.AsPatternNode(), GetInput(right));
        }

        if (left is ValueListNode || (left is JsonNode && left.AsJsonNode().IsArray(ctx)))
            return MatchesAny(right.AsPatternNode(), left.AsJsonNode().AsValueListNode(ctx));
        return Matches(right.AsPatternNode(), GetInput(left));
    }

    private bool Matches(PatternNode patternNode, string inputToMatch)
    {
        return patternNode.Value.IsMatch(inputToMatch);
    }

    private bool MatchesAny(PatternNode patternNode, ValueNode valueNode)
    {
        if (valueNode is ValueListNode)
        {
            var listNode = valueNode.AsValueListNode();
            var pattern = patternNode.Value;

            foreach (var it in listNode)
            {
                var input = GetInput(it);
                if (pattern.IsMatch(input)) return true;
            }

            return false;
        }

        return false;
    }

    private string GetInput(ValueNode valueNode)
    {
        var input = "";

        if (valueNode is StringNode || valueNode is NumberNode)
            input = valueNode.AsStringNode().Value;
        else if (valueNode is BooleanNode) input = valueNode.AsBooleanNode().ToString();

        return input;
    }
}