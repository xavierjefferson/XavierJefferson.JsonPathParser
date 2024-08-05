using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Filtering.ValueNodes;

public class UndefinedNode : ValueNode
{
    public override Type Type(IPredicateContext ctx)
    {
        return typeof(void);
    }

    public override UndefinedNode AsUndefinedNode()
    {
        return this;
    }

    public override int GetHashCode()
    {
        return 0;
    }

    public override bool Equals(object? o)
    {
        return false;
    }
}