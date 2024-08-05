using XavierJefferson.JsonPathParser.Exceptions;
using XavierJefferson.JsonPathParser.Filtering.ValueNodes;
using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Filtering.Evaluation;

internal class ExistsEvaluator : IEvaluator
{
    public bool Evaluate(ValueNode left, ValueNode right, IPredicateContext ctx)
    {
        if (left is BooleanNode || right is BooleanNode)
            return left.AsBooleanNode().Value == right.AsBooleanNode().Value;
        throw new JsonPathException("Failed to evaluate exists expression");
    }
}