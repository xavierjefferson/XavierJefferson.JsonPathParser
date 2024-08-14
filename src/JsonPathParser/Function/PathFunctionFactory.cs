using System.Collections.ObjectModel;
using XavierJefferson.JsonPathParser.Exceptions;
using XavierJefferson.JsonPathParser.Function.Json;
using XavierJefferson.JsonPathParser.Function.Numeric;
using XavierJefferson.JsonPathParser.Function.Sequence;
using XavierJefferson.JsonPathParser.Function.Text;
using Index = XavierJefferson.JsonPathParser.Function.Sequence.Index;

namespace XavierJefferson.JsonPathParser.Function;

/// <summary>
///     Implements a factory that given a name of the function will return the Function implementation, or null
///     if the value is not obtained.
///     * Leverages the function's name in order to determine which function to execute which is maintained internally
///     here via a static map
///     */
public class PathFunctionFactory
{
    public static readonly ReadOnlyDictionary<string, Type> Functions;

    static PathFunctionFactory()
    {
        // New functions should be added here and ensure the name is not overridden
        var map = new Dictionary<string, Type>(StringComparer.InvariantCultureIgnoreCase);

        // Math Functions
        map.Add("avg", typeof(Average));
        map.Add("stddev", typeof(StandardDeviation));
        map.Add("sum", typeof(Sum));
        map.Add("Min", typeof(Min));
        map.Add("max", typeof(Max));

        // Text Functions
        map.Add("concat", typeof(Concatenate));

        // JSON Entity Functions
        map.Add(Length.TokenName, typeof(Length));
        map.Add("size", typeof(Length));
        map.Add("append", typeof(Append));
        map.Add("keys", typeof(KeySetFunction));

        // Sequential Functions
        map.Add("first", typeof(First));
        map.Add("last", typeof(Last));
        map.Add("index", typeof(Index));


        Functions = new ReadOnlyDictionary<string, Type>(map);
    }

    /// <summary>
    ///     Returns the function by name or  if function not found.
    /// </summary>
    /// @see #FUNCTIONS
    /// @see PathFunction
    /// </summary>
    /// <param name="name">*      The name of the function</param>
    /// <returns>The implementation of a function</returns>
    public static IPathFunction NewFunction(string? name)
    {
        if (string.IsNullOrWhiteSpace(name) || !Functions.ContainsKey(name))
            throw new InvalidPathException("Function of name: " + name + " cannot be created");
        var functionType = Functions[name];
        try
        {
            return (IPathFunction)Activator.CreateInstance(functionType);
        }
        catch (Exception e)
        {
            throw new InvalidPathException("Function of name: " + name + " cannot be created", e);
        }
    }
}