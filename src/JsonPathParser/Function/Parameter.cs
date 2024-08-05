using XavierJefferson.JsonPathParser.Extensions;
using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Function;

/// <summary>
///     Defines a parameter as passed to a function with late binding support for lazy evaluation.
/// </summary>
public class Parameter
{
    private string _json;

    public Parameter()
    {
    }

    public Parameter(string json)
    {
        this._json = json;
        ParameterType = ParamType.Json;
    }

    public Parameter(IPath path)
    {
        Path = path;
        ParameterType = ParamType.Path;
    }

    public bool Evaluated { get; set; }
    public ILateBindingValue LateBinding { get; set; }
    public IPath Path { get; set; }
    public ParamType ParameterType { get; set; }

    public object? GetValue()
    {
        return LateBinding.Get();
    }

    public string GetJson()
    {
        return _json;
    }

    public void SetJson(string json)
    {
        this._json = json;
    }

    /// <summary>
    ///     Translate the collection of parameters into a collection of values of type T.
    /// </summary>
    /// <param name="type">*      The type to translate the collection into.</param>
    /// <param name="ctx">*      Context.</param>
    /// <param name="parameters">*      ICollection of parameters.</param>
    /// <param name="
    /// 
    /// <T>
    ///     ">*      Type T returned as a List of T.</param>
    ///     <returns>List of T either empty or containing contents.</returns>
    ///     </summary>
    public static SerializingList<T> ToList<T>(IEvaluationContext ctx, SerializingList<Parameter>? parameters)
    {
        var values = new SerializingList<T>();
        if (parameters == null) return values;
        foreach (var param in parameters)
            Consume(ctx, values, param.GetValue());
        return values;
    }

    /// <summary>
    ///     Either consume the object as an array and.Add each element to the collection, or alternatively.Add each element
    /// </summary>
    /// <param name="expectedType">
    ///     *      the expected class type to consume, if null or not of this type the element is not
    ///     added to the array.
    /// </param>
    /// <param name="ctx">*      the JSON context to determine if this is an array or value.</param>
    /// <param name="collection">*      The collection to append into.</param>
    /// <param name="value">*      The value to evaluate.</param>
    public static void Consume<T>(IEvaluationContext ctx, ICollection<T> collection, object? value)
    {
        JpObjectList toAdd;
        var expectedType = typeof(T);
        var canAddNull = false;
        if (ctx.Configuration.JsonProvider.IsArray(value))
            toAdd = new JpObjectList(ctx.Configuration.JsonProvider.AsEnumerable(value).Cast<object>()
                .Where(i => i != null));
        else
            toAdd = new JpObjectList { value };

        foreach (var o in toAdd.Where(i => i != null))
        {
            double number;
            if (o != null && expectedType == typeof(double) && o.TryConvertDouble(out number))
                collection.Add((T)(object)number);
            else if (o != null && expectedType.IsAssignableFrom(o.GetType()))
                collection.Add((T)o);
            else if (o != null && expectedType == typeof(string)) collection.Add((T)(object)o.ToString());
        }
    }
}