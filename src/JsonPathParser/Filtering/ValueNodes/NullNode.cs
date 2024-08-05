using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Filtering.ValueNodes;

public class NullNode : ValueNode
{
    //private NullNode() { }


    public override Type Type(IPredicateContext ctx)
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
        if (!(o is NullNode)) return false;

        return true;
    }
}