using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Filtering.ValueNodes;

public class NullNode : ValueNode
{
    public override Type Type(IPredicateContext context)
    {
        return typeof(void);
    }


    public override NullNode AsNullNode()
    {
        return this;
    }


    public override string ToString()
    {
        return "null";
    }

    public override int GetHashCode()
    {
        return 0;
    }

    public override bool Equals(object? o)
    {
        if (this == o) return true;
        return o is NullNode;
    }
}