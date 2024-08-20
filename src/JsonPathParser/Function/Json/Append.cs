using XavierJefferson.JsonPathParser.Interfaces;
using XavierJefferson.JsonPathParser.PathRefs;

namespace XavierJefferson.JsonPathParser.Function.Json;

/// <summary>
///     Appends JSON structure to the current document so that you can utilize the JSON added thru another function call.
///     If there are multiple parameters then this function call will.Add each element that is json to the structure
///     * Created by mgreenwood on 12/14/15.
/// </summary>
public class Append : IPathFunction, IInvocable
{
    public object? Invoke(string currentPath, PathRef parent, object? model, IEvaluationContext context,
        IList<Parameter>? parameters)
    {
        var jsonProvider = context.Configuration.JsonProvider;
        if (parameters != null && parameters.Count() > 0)
            foreach (var param in parameters)
                if (jsonProvider.IsArray(model))
                {
                    var len = jsonProvider.Length(model);
                    jsonProvider.SetArrayIndex(model, len, param.GetValue());
                }

        return model;
    }
}