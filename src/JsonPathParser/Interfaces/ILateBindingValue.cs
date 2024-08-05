namespace XavierJefferson.JsonPathParser.Interfaces;

/// <summary>
///     Obtain the late binding value at runtime rather than storing the value in the cache thus trashing the cache
/// </summary>
public interface ILateBindingValue
{
    /// <summary>
    ///     Obtain the value of the parameter at runtime using the parameter state and invocation of other late binding values
    ///     rather than maintaining cached state which ends up in a global store and won't change as a result of external
    ///     reference changes.
    /// </summary>
    /// <returns>The value of evaluating the context at runtime.</returns>
    object? Get();
}