using XavierJefferson.JsonPathParser.Exceptions;

namespace XavierJefferson.JsonPathParser.Path;

public abstract class ArrayPathToken : PathToken
{
    /// <summary>
    ///     Check if model is non-null and array.
    /// </summary>
    /// <param name="currentPath">* @param model</param>
    /// <param name="ctx"></param>
    /// @ if model is null and evaluation must be interrupted
    /// @ if model is not an array and evaluation must be interrupted
    /// <returns> false if current evaluation call must be skipped, true otherwise</returns>
    protected bool CheckArrayModel(string currentPath, object? model, EvaluationContextImpl ctx)
    {
        if (model == null)
        {
            if (!IsUpstreamDefinite()
                || ctx.Options.Contains(Option.SuppressExceptions))
                return false;
            throw new PathNotFoundException($"The path {currentPath}" + " is null");
        }

        if (!ctx.JsonProvider.IsArray(model))
        {
            if (!IsUpstreamDefinite()
                || ctx.Options.Contains(Option.SuppressExceptions))
                return false;
            throw new PathNotFoundException(
                $"Filter: {ToString()} can only be applied to arrays. Current context is: {model}");
        }

        return true;
    }
}