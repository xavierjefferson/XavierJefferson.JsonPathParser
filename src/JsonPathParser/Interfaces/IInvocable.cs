using XavierJefferson.JsonPathParser.Function;
using XavierJefferson.JsonPathParser.PathRefs;

namespace XavierJefferson.JsonPathParser.Interfaces;

public interface IInvocable
{
    object? Invoke(string currentPath, PathRef parent, object? model, IEvaluationContext context,
        IList<Parameter>? parameters);
}