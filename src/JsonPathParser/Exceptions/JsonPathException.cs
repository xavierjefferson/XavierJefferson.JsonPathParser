namespace XavierJefferson.JsonPathParser.Exceptions;

public class JsonPathException : Exception
{
    public JsonPathException()
    {
    }

    public JsonPathException(string message) : base(message)
    {
    }

    public JsonPathException(string message, Exception cause) : base(message, cause)
    {
    }

    public JsonPathException(Exception cause) : base(null, cause)
    {
    }
}