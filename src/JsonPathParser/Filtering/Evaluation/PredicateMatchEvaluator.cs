using XavierJefferson.JsonPathParser.Filtering.ValueNodes;
using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Filtering.Evaluation;

public class PredicateMatchEvaluator : IEvaluator
{
    public bool Evaluate(ValueNode left, ValueNode right, IPredicateContext context)
    {
        return right.AsPredicateNode().Value.Apply(context);
    }
}