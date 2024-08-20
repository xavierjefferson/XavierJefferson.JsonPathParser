using XavierJefferson.JsonPathParser.Enums;
using XavierJefferson.JsonPathParser.Extensions;
using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Function;

/// <summary>
///     Defines a parameter as passed to a function with late binding support for lazy evaluation.
/// </summary>
public class Parameter
{
    public string Json { get;  set; }

    public Parameter()
    {
    }

    public Parameter(string json)
    {
        Json = json;
        ParameterType = ParameterTypeEnum.Json;
    }

    public Parameter(IPath path)
    {
        Path = path;
        ParameterType = ParameterTypeEnum.Path;
    }

    public bool Evaluated { get; set; }
    public ILateBindingValue LateBinding { get; set; }
    public IPath Path { get; set; }
    public ParameterTypeEnum ParameterType { get; set; }

    public object? GetValue()
    {
        return LateBinding.Get();
    }

    /// <summary>
    ///     Translate the collection of parameters into a collection of values of type T.
    /// </summary>
    /// <typeparam name="T">The type to translate the collection into.</typeparam>
    /// <param name="context">      Context.</param>
    /// <param name="parameters">      ICollection of parameters.</param>
    /// <returns>List of T either empty or containing contents.</returns>
    public static IList<T> ToList<T>(IEvaluationContext context, IEnumerable<Parameter>? parameters)
    {
        var values = new List<T>();
        if (parameters == null) return values;
        foreach (var param in parameters)
            Consume(context, values, param.GetValue());
        return values;
    }

    /// <summary>
    ///     Either consume the object as an array and.Add each element to the collection, or alternatively add each element
    /// </summary>
    /// <typeparam name="T">
    ///     the expected class type to consume, if null or not of this type the element is not
    ///     added to the array.
    /// </typeparam>
    /// <param name="context">      the JSON context to determine if this is an array or value.</param>
    /// <param name="collection">      The collection to append into.</param>
    /// <param name="value">      The value to evaluate.</param>
    public static void Consume<T>(IEvaluationContext context, ICollection<T> collection, object? value)
    {
        List<object?> toAdd;
        var expectedType = typeof(T);
        if (context.Configuration.JsonProvider.IsArray(value))
            toAdd = context.Configuration.JsonProvider.AsEnumerable(value);
        else
            toAdd = new List<object?> { value };

        foreach (var o in toAdd.Where(i => i != null))
            if (o is T instance)
                collection.Add(instance);
            else if (expectedType == typeof(double) && o.TryConvertDouble(out var number))
                collection.Add((T)(object)number);
            else if (expectedType == typeof(string)) collection.Add((T)(object)o.ToString());
    }
}