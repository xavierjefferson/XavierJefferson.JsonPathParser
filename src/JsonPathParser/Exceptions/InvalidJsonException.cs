namespace XavierJefferson.JsonPathParser.Exceptions;

public class InvalidJsonException : JsonPathException
{
    /// <summary>
    ///     Problematic JSON if available.
    /// </summary>
    private readonly string _json;

    public InvalidJsonException()
    {
        _json = null;
    }

    public InvalidJsonException(string message) : base(message)
    {
        _json = null;
    }

    public InvalidJsonException(string message, Exception cause) : base(message, cause)
    {
        _json = null;
    }

    public InvalidJsonException(Exception cause) : base(cause)
    {
        _json = null;
    }

    /// <summary>
    ///     Rethrow the exception with the problematic JSON captured.
    /// </summary>
    public InvalidJsonException(Exception cause, string json) : base(cause)
    {
        _json = json;
    }

    /// <summary>
    ///     <returns> the problematic JSON if available.</returns>
    public string GetJson()
    {
        return _json;
    }
}