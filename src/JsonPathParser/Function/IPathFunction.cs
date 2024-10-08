using XavierJefferson.JsonPathParser.Interfaces;
using XavierJefferson.JsonPathParser.PathRefs;

namespace XavierJefferson.JsonPathParser.Function;

/// <summary>
///     Defines the pattern by which a function can be executed over the result set in the particular path
///     being grabbed.  The Function's input is the content of the data from the json path selector and its output
///     is defined via the functions behavior.  Thus transformations in types can take place.  Additionally, functions
///     can accept multiple selectors in order to produce their output.
///     * Created by matt@mjgreenwood.net on 6/26/15.
/// </summary>
public interface IPathFunction
{
    /// <summary>
    ///     Invoke the function and output a JSON object (or scalar) value which will be the result of executing the path
    /// </summary>
    /// <param name="currentPath">      The current path location inclusive of the function name</param>
    /// <param name="parent">      The path location above the current function</param>
    /// <param name="model">      The JSON model as input to this particular function</param>
    /// <param name="context">      Eval context, state bag used as the path is traversed, maintains the result of executing</param>
    /// <param name="parameters">
    //////<returns> result</returns></param>
    object? Invoke(string currentPath, PathRef parent, object? model, IEvaluationContext context,
        IList<Parameter>? parameters);
}