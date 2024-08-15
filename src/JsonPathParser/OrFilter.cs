using System.Text;
using XavierJefferson.JsonPathParser.Helpers;
using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser;

public class OrFilter : Filter
{
    private readonly IPredicate _left;
    private readonly IPredicate _right;

    public OrFilter(IPredicate left, IPredicate right)
    {
        _left = left;
        _right = right;
    }

    public override Filter And(IPredicate other)
    {
        return new OrFilter(_left, new AndFilter(_right, other));
    }


    public override bool Apply(IPredicateContext context)
    {
        return _left.Apply(context) || _right.Apply(context);
    }

    public override string ToUnenclosedString()
    {
        return $"{_left.ToUnenclosedString()} || {_right.ToUnenclosedString()}";
    }

  
}