using XavierJefferson.JsonPathParser.Filtering.ValueNodes;

namespace XavierJefferson.JsonPathParser.Interfaces;

public interface IEvaluator
{
    bool Evaluate(ValueNode left, ValueNode right, IPredicateContext context);
}