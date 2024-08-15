using XavierJefferson.JsonPathParser.Filtering.ValueNodes;
using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Filtering.Evaluation;

internal class NotEqualsEvaluator : EqualsEvaluator
{
    public override bool Evaluate(ValueNode left, ValueNode right, IPredicateContext context)
    {
        return !base.Evaluate(left, right, context);
    }
}