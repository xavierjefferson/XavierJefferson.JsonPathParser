using XavierJefferson.JsonPathParser.Interfaces;
using XavierJefferson.JsonPathParser.PathRefs;

namespace XavierJefferson.JsonPathParser.Function.Json;

/// <summary>
///     Author: Sergey Saiyan sergey.sova42@gmail.com
///     Created at 21/02/2018.
public class KeySetFunction : IPathFunction
{
    public object? Invoke(string currentPath, PathRef parent, object? model, IEvaluationContext context,
        SerializingList<Parameter>? parameters)
    {
        if (context.Configuration.JsonProvider.IsMap(model)) return context.Configuration.JsonProvider.GetPropertyKeys(model);
        return null;
    }
}