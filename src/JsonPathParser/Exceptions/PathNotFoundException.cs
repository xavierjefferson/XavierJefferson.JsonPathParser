namespace XavierJefferson.JsonPathParser.Exceptions;

public class PathNotFoundException : InvalidPathException
{
    public PathNotFoundException()
    {
    }

    public PathNotFoundException(string message) : base(message)
    {
    }

    public PathNotFoundException(string message, Exception cause) : base(message, cause)
    {
    }

    public PathNotFoundException(Exception cause) : base(cause)
    {
    }


    public Exception FillInStackTrace()
    {
        return this;
    }
}