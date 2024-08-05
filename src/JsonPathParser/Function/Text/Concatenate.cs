using System.Text;
using XavierJefferson.JsonPathParser.Interfaces;
using XavierJefferson.JsonPathParser.PathRefs;

namespace XavierJefferson.JsonPathParser.Function.Text;

/// <summary>
///     string function concat - simple takes a list of arguments and/or an array and concatenates them together to form a
///     single string
///     */
public class Concatenate : IPathFunction
{
    public object? Invoke(string currentPath, PathRef parent, object? model, IEvaluationContext ctx,
        SerializingList<Parameter>? parameters)
    {
        var result = new StringBuilder();
        if (ctx.Configuration.JsonProvider.IsArray(model))
        {
            var objects = ctx.Configuration.JsonProvider.AsEnumerable(model).Cast<object>();
            foreach (var obj in objects)
                if (obj is string)
                    result.Append(obj);
        }

        if (parameters != null)
            foreach (var value in Parameter.ToList<string>(ctx, parameters))
                result.Append(value);
        return result.ToString();
    }
}