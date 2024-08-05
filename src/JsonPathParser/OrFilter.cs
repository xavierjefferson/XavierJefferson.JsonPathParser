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


    public override bool Apply(IPredicateContext ctx)
    {
        var a = _left.Apply(ctx);
        return a || _right.Apply(ctx);
    }


    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("[?(");

        var l = _left.ToString();
        var r = _right.ToString();

        if (l.StartsWith("[?(")) l = l.Subsequence(3, l.Length - 2);
        if (r.StartsWith("[?(")) r = r.Subsequence(3, r.Length - 2);

        sb.Append(l).Append(" || ").Append(r);

        sb.Append(")]");
        return sb.ToString();
    }
}