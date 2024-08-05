using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Function.Sequence;

/// <summary>
///     Take the first item from collection of JSONArray
/// </summary>
public class First : AbstractSequenceAggregation
{
    protected override int TargetIndex(IEvaluationContext ctx, SerializingList<Parameter>? parameters)
    {
        return 0;
    }
}