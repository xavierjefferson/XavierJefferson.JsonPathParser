using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Filtering;

public class SimplePredicate : IPredicate
{
    private Func<IPredicateContext, bool> _func;

    private SimplePredicate()
    {
    }

    public bool Apply(IPredicateContext context)
    {
        return _func(context);
    }

    public string ToUnenclosedString()
    {
        return _func.ToString();
    }

    public static SimplePredicate Create(Func<IPredicateContext, bool> func)
    {
        return new SimplePredicate { _func = func };
    }
}