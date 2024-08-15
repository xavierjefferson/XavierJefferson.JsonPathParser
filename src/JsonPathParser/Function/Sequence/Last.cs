using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Function.Sequence;

/// <summary>
///     Take the first item from collection of JSONArray
/// </summary>
public class Last : AbstractSequenceAggregation
{
    protected override int TargetIndex(IEvaluationContext context, SerializingList<Parameter>? parameters)
    {
        return -1;
    }
}