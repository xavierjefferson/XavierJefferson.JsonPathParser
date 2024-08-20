using XavierJefferson.JsonPathParser.Interfaces;
using XavierJefferson.JsonPathParser.PathRefs;

namespace XavierJefferson.JsonPathParser.Function;

/// <summary>
///     Defines the default behavior which is to return the model that is provided as input as output
/// </summary>
public class PassthruPathFunction : IPathFunction
{
    public object? Invoke(string currentPath, PathRef parent, object? model, IEvaluationContext context,
        IList<Parameter>? parameters)
    {
        return model;
    }
}