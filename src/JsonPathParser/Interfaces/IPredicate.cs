namespace XavierJefferson.JsonPathParser.Interfaces;

public interface IPredicate
{
    bool Apply(IPredicateContext context);

    public string ToUnenclosedString();
}