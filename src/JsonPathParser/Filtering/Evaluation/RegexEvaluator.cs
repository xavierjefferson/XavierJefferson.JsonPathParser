using XavierJefferson.JsonPathParser.Filtering.ValueNodes;
using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Filtering.Evaluation;

public class RegexEvaluator : IEvaluator
{
    public bool Evaluate(ValueNode left, ValueNode right, IPredicateContext context)
    {
        if (!(left is PatternNode ^ right is PatternNode)) return false;

        if (left is PatternNode leftPatternNode)
        {
            if (right is ValueListNode rightValueListNode)
            {
                return MatchesAny(leftPatternNode, rightValueListNode);
            }

            if (right is JsonNode rightJsonNode && rightJsonNode.IsArray(context))
                return MatchesAny(leftPatternNode, rightJsonNode.AsValueListNode(context));
            return Matches(leftPatternNode, GetInput(right));
        }
        if (left is ValueListNode leftValueListNode)
        {
            return MatchesAny(right.AsPatternNode(), leftValueListNode);
        }

        if (left is JsonNode leftJsonNode && leftJsonNode.IsArray(context))
            return MatchesAny(right.AsPatternNode(), leftJsonNode.AsValueListNode(context));
        return Matches(right.AsPatternNode(), GetInput(left));
    }

    private bool Matches(PatternNode patternNode, string inputToMatch)
    {
        return patternNode.Value.IsMatch(inputToMatch);
    }

    private bool MatchesAny(PatternNode patternNode, ValueNode valueNode)
    {
        if (valueNode is ValueListNode valueListNode)
        {
            var pattern = patternNode.Value;

            foreach (var it in valueListNode)
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
        else if (valueNode is BooleanNode booleanNode) input = booleanNode.ToString();

        return input;
    }
}