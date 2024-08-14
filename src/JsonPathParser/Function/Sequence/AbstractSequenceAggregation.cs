using XavierJefferson.JsonPathParser.Exceptions;
using XavierJefferson.JsonPathParser.Extensions;
using XavierJefferson.JsonPathParser.Function.Numeric;
using XavierJefferson.JsonPathParser.Interfaces;
using XavierJefferson.JsonPathParser.PathRefs;

namespace XavierJefferson.JsonPathParser.Function.Sequence;

/// <summary>
///     Defines the pattern for taking item from collection of JSONArray by index
/// </summary>
public abstract class AbstractSequenceAggregation : IPathFunction, IInvocable
{
    public object? Invoke(string currentPath, PathRef parent, object? model, IEvaluationContext ctx,
        SerializingList<Parameter>? parameters)
    {
        if (ctx.Configuration.JsonProvider.IsArray(model))
        {
            var objects = ctx.Configuration.JsonProvider.AsEnumerable(model);
            var objectList = objects.Cast<object>().ToSerializingList();

            var targetIndex = TargetIndex(ctx, parameters);
            if (targetIndex >= 0) return objectList[targetIndex];

            var realIndex = objectList.Count() + targetIndex;
            if (realIndex > 0)
                return objectList[realIndex];
            throw new JsonPathException($"Target index:{targetIndex} larger than object count:{objectList.Count()}");
        }

        throw new JsonPathException(AbstractAggregation.EmptyArrayMessage);
    }

    protected abstract int TargetIndex(IEvaluationContext ctx, SerializingList<Parameter>? parameters);

    protected int GetIndexFromParameters(IEvaluationContext ctx, SerializingList<Parameter>? parameters)
    {
        var numbers = Parameter.ToList<double>(ctx, parameters);
        return Convert.ToInt32(numbers[0]);
    }
}