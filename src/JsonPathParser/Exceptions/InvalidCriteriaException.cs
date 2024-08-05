namespace XavierJefferson.JsonPathParser.Exceptions;

public class InvalidCriteriaException : JsonPathException
{
    public InvalidCriteriaException()
    {
    }

    public InvalidCriteriaException(string message) : base(message)
    {
        ;
    }

    public InvalidCriteriaException(string message, Exception cause) : base(message, cause)
    {
        ;
    }

    public InvalidCriteriaException(Exception cause) : base(cause)
    {
    }
}