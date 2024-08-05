using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Function.Sequence;

/// <summary>
///     Take the index from first Parameter, then the item from collection of JSONArray by index
/// </summary>
public class Index : AbstractSequenceAggregation
{
    protected override int TargetIndex(IEvaluationContext ctx, SerializingList<Parameter>? parameters)
    {
        return GetIndexFromParameters(ctx, parameters);
    }
}