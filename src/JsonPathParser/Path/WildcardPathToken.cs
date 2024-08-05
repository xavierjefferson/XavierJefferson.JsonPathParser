using XavierJefferson.JsonPathParser.Exceptions;
using XavierJefferson.JsonPathParser.PathRefs;

namespace XavierJefferson.JsonPathParser.Path;

/// <summary>
///     */
public class WildcardPathToken : PathToken
{
    public override void Evaluate(string currentPath, PathRef parent, object? model, EvaluationContextImpl ctx)
    {
        if (ctx.JsonProvider.IsMap(model))
            foreach (var property in ctx.JsonProvider.GetPropertyKeys(model))
                HandleObjectProperty(currentPath, model, ctx, new SerializingList<string> { property });
        else if (ctx.JsonProvider.IsArray(model))
            for (var idx = 0; idx < ctx.JsonProvider.Length(model); idx++)
                try
                {
                    HandleArrayIndex(idx, currentPath, model, ctx);
                }
                catch (PathNotFoundException p)
                {
                    if (ctx.Options.Contains(Option.RequireProperties)) throw p;
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