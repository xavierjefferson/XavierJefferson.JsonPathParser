using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Function.LateBinding;

/// <summary>
///     Defines the JSON document Late binding approach to function arguments.
/// </summary>
public class JsonLateBindingValue : ILateBindingValue
{
    private readonly Parameter _jsonParameter;
    private readonly IJsonProvider _jsonProvider;

    public JsonLateBindingValue(IJsonProvider jsonProvider, Parameter jsonParameter)
    {
        _jsonProvider = jsonProvider;
        _jsonParameter = jsonParameter;
    }

    /// <summary>
    ///     Evaluate the JSON document at the point of need using the JSON parameter and associated document model which may
    ///     itself originate from yet another function thus recursively invoking late binding methods.
    /// </summary>
    /// <returns> the late value</returns>
    public object? Get()
    {
        return _jsonProvider.Parse(_jsonParameter.Json);
    }
}