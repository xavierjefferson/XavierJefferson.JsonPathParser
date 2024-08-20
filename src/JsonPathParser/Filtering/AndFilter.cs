using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Filtering;

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


    public override bool Apply(IPredicateContext context)
    {
        return _predicates.All(i => i.Apply(context));
        //foreach (var predicate in _predicates)
        //    if (!predicate.Apply(context))
        //        return false;
        //return true;
    }

    public override string ToUnenclosedString()
    {
        return string.Join(" && ", _predicates.Select(i => i.ToUnenclosedString()));
    }


}