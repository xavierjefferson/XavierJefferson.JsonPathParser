using XavierJefferson.JsonPathParser.Interfaces;
using XavierJefferson.JsonPathParser.PathRefs;

namespace XavierJefferson.JsonPathParser.Function.Json;

/// <summary>
///     Author: Sergey Saiyan sergey.sova42@gmail.com
///     Created at 21/02/2018.
public class KeySetFunction : IPathFunction
{
    public object? Invoke(string currentPath, PathRef parent, object? model, IEvaluationContext ctx,
        SerializingList<Parameter>? parameters)
    {
        if (ctx.Configuration.JsonProvider.IsMap(model)) return ctx.Configuration.JsonProvider.GetPropertyKeys(model);
        return null;
    }
}