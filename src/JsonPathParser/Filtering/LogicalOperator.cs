using XavierJefferson.JsonPathParser.Exceptions;

namespace XavierJefferson.JsonPathParser.Filtering;

public class LogicalOperator
{
    public static readonly LogicalOperator And = new("&&");
    public static readonly LogicalOperator Not = new("!");
    public static readonly LogicalOperator Or = new("||");

    private LogicalOperator(string operatorString)
    {
        OperatorString = operatorString;
    }

    public string OperatorString { get; }


    public override string ToString()
    {
        return OperatorString;
    }

    public static LogicalOperator FromString(string operatorString)
    {
        if (And.OperatorString.Equals(operatorString)) return And;
        if (Not.OperatorString.Equals(operatorString)) return Not;
        if (Or.OperatorString.Equals(operatorString)) return Or;
        throw new InvalidPathException($"Failed to Parse operator {operatorString}");
    }
}