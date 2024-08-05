using XavierJefferson.JsonPathParser.Filtering.ValueNodes;
using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Filtering.Evaluation;

public class NotInEvaluator : InEvaluator
{
    public override bool Evaluate(ValueNode left, ValueNode right, IPredicateContext ctx)
    {
        return !base.Evaluate(left, right, ctx);
    }
}