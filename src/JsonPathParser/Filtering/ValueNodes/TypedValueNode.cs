using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Filtering.ValueNodes;

public abstract class TypedValueNode<T> : ValueNode
{
    public abstract T Value { get; }
    public override Type Type(IPredicateContext ctx)
    {
        return typeof(T);
    }
}
