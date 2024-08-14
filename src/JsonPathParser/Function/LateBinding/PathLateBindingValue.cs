using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Function.LateBinding;

/// <summary>
///     Defines the contract for late bindings, provides document state and enough context to perform the evaluation at a
///     later
///     date such that we can operate on a dynamically changing value.
///     * Acts like a lambda function with references, but since we're supporting JDK 6+, we're left doing params this[]
///     */
public class PathLateBindingValue : ILateBindingValue
{
    private readonly Configuration _configuration;
    private readonly IPath _path;
    private readonly object? _result;
    private readonly string _rootDocument;

    public PathLateBindingValue(IPath path, object? rootDocument, Configuration configuration)
    {
        _path = path;
        _rootDocument = rootDocument?.ToString();
        _configuration = configuration;
        _result = path.Evaluate(rootDocument, rootDocument, configuration).GetValue();
    }

    /// <summary>
    /// </summary>
    /// <returns> the late value</returns>
    public object? Get()
    {
        return _result;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_path, _rootDocument, _configuration);
    }


    public override bool Equals(object? o)
    {
        if (this == o) return true;
        if (o == null || GetType() != o.GetType()) return false;
        var that = (PathLateBindingValue)o;
        return Equals(_path, that._path) &&
               _rootDocument.Equals(that._rootDocument) &&
               Equals(_configuration, that._configuration);
    }
}