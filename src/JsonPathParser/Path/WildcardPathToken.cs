using XavierJefferson.JsonPathParser.Enums;
using XavierJefferson.JsonPathParser.Exceptions;
using XavierJefferson.JsonPathParser.PathRefs;

namespace XavierJefferson.JsonPathParser.Path;

/// <summary>
///     */
public class WildcardPathToken : PathToken
{
    public override void Evaluate(string currentPath, PathRef parent, object? model, EvaluationContextImpl context)
    {
        if (context.JsonProvider.IsMap(model))
            foreach (var property in context.JsonProvider.GetPropertyKeys(model))
                HandleObjectProperty(currentPath, model, context, new SerializingList<string> { property });
        else if (context.JsonProvider.IsArray(model))
            for (var idx = 0; idx < context.JsonProvider.Length(model); idx++)
                try
                {
                    HandleArrayIndex(idx, currentPath, model, context);
                }
                catch (PathNotFoundException p)
                {
                    if (context.Options.Contains(ConfigurationOptionEnum.RequireProperties)) throw p;
                }
    }


    public override bool IsTokenDefinite()
    {
        return false;
    }


    public override string GetPathFragment()
    {
        return "[*]";
    }
}