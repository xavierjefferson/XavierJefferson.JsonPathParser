using XavierJefferson.JsonPathParser.Filtering.ValueNodes;
using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Filtering.Evaluation;

public class TypeSafeEqualsEvaluator : EqualsEvaluator
{
    public override bool Evaluate(ValueNode left, ValueNode right, IPredicateContext ctx)
    {
        return left.GetType().Equals(right.GetType()) && base.Evaluate(left, right, ctx);
    }
}