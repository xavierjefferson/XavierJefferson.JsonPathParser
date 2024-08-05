using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Filtering.ValueNodes;

public class ClassNode : TypedValueNode<Type>
{
    private readonly Type _type;

    public override Type Value => _type;

    public ClassNode(Type type)
    {
        _type = type;
    }

    public override ClassNode AsClassNode()
    {
        return this;
    }


    public override string ToString()
    {
        return _type.FullName;
    }

    public override int GetHashCode()
    {
        return _type.GetHashCode();
    }

    public override bool Equals(object? o)
    {
        if (this == o) return true;
        if (o is ClassNode that)
        {
            return _type != null && _type.Equals(that._type);
        }
        return false;
    }
}