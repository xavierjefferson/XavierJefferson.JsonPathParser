namespace XavierJefferson.JsonPathParser.Exceptions;

public class InvalidPathException : JsonPathException
{
    public InvalidPathException()
    {
    }

    public InvalidPathException(string message) : base(message)
    {
    }

    public InvalidPathException(string message, Exception cause) : base(message, cause)
    {
    }

    public InvalidPathException(Exception cause) : base(cause)
    {
    }
}