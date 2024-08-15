using XavierJefferson.JsonPathParser.Interfaces;
using XavierJefferson.JsonPathParser.Path;
using XavierJefferson.JsonPathParser.PathRefs;

namespace XavierJefferson.JsonPathParser.Function.Text;

/// <summary>
///     Provides the length of a JSONArray object
/// </summary>
/// Created by mattg on 6/26/15.
/// </summary>
public class Length : IPathFunction
{
    public static readonly string TokenName = "length";

    /// <summary>
    ///     When we calculate the length of a path, what we're asking is given the node we land on how many children does it
    ///     have.  Thus when we wrote the original query what we really wanted was $..book.Length or $.Length($..book.*)
    /// </summary>
    /// <param name="currentPath">      The current path location inclusive of the function name</param>
    /// <param name="parent">      The path location above the current function</param>
    /// <param name="model">      The JSON model as input to this particular function</param>
    /// <param name="context">      Eval context, state bag used as the path is traversed, maintains the result of executing</param>
    /// <param name="parameters">
    ///     ///
    ///     <returns></returns>
    /// </param>
    public object? Invoke(string currentPath, PathRef parent, object? model, IEvaluationContext context,
        SerializingList<Parameter>? parameters)
    {
        if (null != parameters && parameters.Count() > 0)
        {
            // HashSet the tail of the first parameter, when its not a function path parameter (which wouldn't make sense
            // for length - to the wildcard such that we request all of its children so we can get back an array and
            // take its length.
            var lengthOfParameter = parameters[0];
            if (!lengthOfParameter.Path.IsFunctionPath())
            {
                var path = lengthOfParameter.Path;
                if (path is CompiledPath compiledPath)
                {
                    var root = compiledPath.GetRoot();
                    var tail = root.GetNext();
                    while (null != tail && null != tail.GetNext()) tail = tail.GetNext();
                    if (null != tail) tail.SetNext(new WildcardPathToken());
                }
            }

            var innerModel = parameters[0].Path.Evaluate(model, model, context.Configuration).GetValue();
            if (context.Configuration.JsonProvider.IsArray(innerModel))
                return context.Configuration.JsonProvider.Length(innerModel);
        }

        if (context.Configuration.JsonProvider.IsArray(model))
            return context.Configuration.JsonProvider.Length(model);
        if (context.Configuration.JsonProvider.IsMap(model)) return context.Configuration.JsonProvider.Length(model);
        return null;
    }
}