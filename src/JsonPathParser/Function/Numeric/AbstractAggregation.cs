using XavierJefferson.JsonPathParser.Exceptions;
using XavierJefferson.JsonPathParser.Extensions;
using XavierJefferson.JsonPathParser.Interfaces;
using XavierJefferson.JsonPathParser.PathRefs;

namespace XavierJefferson.JsonPathParser.Function.Numeric;

/// <summary>
///     Defines the pattern for processing numerical values via an abstract implementation that iterates over the
///     collection
///     of JSONArray entities and verifies that each is a numerical value and then passes that along the abstract methods
/// </summary>
public abstract class AbstractAggregation : IPathFunction
{
    public const string EmptyArrayMessage = "Aggregation function attempted to calculate value using empty array";


    public object? Invoke(string currentPath, PathRef parent, object? model, IEvaluationContext ctx,
        SerializingList<Parameter>? parameters)
    {
        var count = 0;
        if (ctx.Configuration.JsonProvider.IsArray(model))
        {
            var objects = ctx.Configuration.JsonProvider.AsEnumerable(model).Cast<object>();
            foreach (var obj in objects)
                if (obj.TryConvertDouble(out var test))
                {
                    count++;
                    Next(test);
                }
        }

        if (parameters != null)
            foreach (var value in Parameter.ToList<double>(ctx, parameters))
            {
                count++;
                Next(value);
            }

        if (count != 0) return GetValue();
        throw new JsonPathException(EmptyArrayMessage);
    }

    /// <summary>
    ///     Defines the next value in the array to the mathmatical function
    /// </summary>
    /// <param name="value"> The numerical value to process next</param>
    protected abstract void Next(double value);

    /// <summary>
    ///     Obtains the value generated via the series of next value calls
    /// </summary>
    /// <returns> A numerical answer based on the input value provided</returns>
    protected abstract double GetValue();
}