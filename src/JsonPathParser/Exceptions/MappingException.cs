namespace XavierJefferson.JsonPathParser.Exceptions;

public class MappingException : JsonPathException
{
    public MappingException(Exception cause) : base(cause)
    {
    }

    public MappingException(string message) : base(message)
    {
    }
}