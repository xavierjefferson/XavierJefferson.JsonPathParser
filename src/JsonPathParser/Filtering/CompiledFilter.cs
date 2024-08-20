using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Filtering;

public class CompiledFilter : Filter
{
    private readonly IPredicate _predicate;

    public CompiledFilter(IPredicate predicate)
    {
        _predicate = predicate;
    }


    public override bool Apply(IPredicateContext context)
    {
        return _predicate.Apply(context);
    }

    public override string ToUnenclosedString()
    {
        return _predicate.ToUnenclosedString();
    }

    public override string ToString()
    {
        return $"[?({ToUnenclosedString()})]";
    }
}