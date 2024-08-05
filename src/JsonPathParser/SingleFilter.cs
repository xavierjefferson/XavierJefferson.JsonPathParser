using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser;

public class SingleFilter : Filter
{
    private readonly IPredicate _predicate;

    public SingleFilter(IPredicate predicate)
    {
        _predicate = predicate;
    }


    public override bool Apply(IPredicateContext ctx)
    {
        return _predicate.Apply(ctx);
    }


    public override string ToString()
    {
        var predicateString = _predicate?.ToString();
        if (predicateString.StartsWith("("))
            return "[?" + predicateString + "]";
        return "[?(" + predicateString + ")]";
    }
}