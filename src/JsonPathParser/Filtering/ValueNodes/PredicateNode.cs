using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Filtering.ValueNodes;

public class PredicateNode : TypedValueNode<IPredicate>
{
    private readonly IPredicate _predicate;

    public override IPredicate Value => _predicate;

    public PredicateNode(IPredicate predicate)
    {
        this._predicate = predicate;
    }

    public override PredicateNode AsPredicateNode()
    {
        return this;
    }


    public override Type Type(IPredicateContext ctx)
    {
        return typeof(void);
    }


    public override bool Equals(object? o)
    {
        return false;
    }

    public override int GetHashCode()
    {
        return _predicate.GetHashCode();
    }

    public override string ToString()
    {
        return _predicate.ToString();
    }
}