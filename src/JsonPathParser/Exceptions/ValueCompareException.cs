namespace XavierJefferson.JsonPathParser.Exceptions;

public class ValueCompareException : JsonPathException
{
    public ValueCompareException()
    {
    }

    /// <summary>
    ///     Construct the exception with message capturing the classes for two objects.
    /// </summary>
    /// <param name="left">first object</param>
    /// <param name="right">second object</param>
    public ValueCompareException(object? left, object? right) : base(
        $"Can not compare a {left?.GetType().FullName} with a {right?.GetType().FullName}")
    {
    }

    public ValueCompareException(string message) : base(message)
    {
    }

    public ValueCompareException(string message, Exception cause) : base(message, cause)
    {
    }
}