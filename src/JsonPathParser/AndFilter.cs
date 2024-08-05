using System.Text;
using XavierJefferson.JsonPathParser.Helpers;
using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser;

public class AndFilter : Filter
{
    private readonly ICollection<IPredicate> _predicates;

    public AndFilter(ICollection<IPredicate> predicates)
    {
        _predicates = predicates;
    }

    public AndFilter(IPredicate left, IPredicate right) : this(new[] { left, right })
    {
    }

    public override Filter And(IPredicate other)
    {
        ICollection<IPredicate> newPredicates = new SerializingList<IPredicate>(_predicates);
        newPredicates.Add(other);
        return new AndFilter(newPredicates);
    }


    public override bool Apply(IPredicateContext ctx)
    {
        foreach (var predicate in _predicates)
            if (!predicate.Apply(ctx))
                return false;
        return true;
    }


    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("[?(");
        foreach (var px in _predicates.Select((i, j) => new { i = i.ToString(), j }))
        {
            var p = px.i;
            if (px.j > 0) sb.Append(" && ");

            if (p.StartsWith("[?(")) p = p.Subsequence(3, p.Length - 2);
            sb.Append(p);
        }

        sb.Append(")]");
        return sb.ToString();
    }
}