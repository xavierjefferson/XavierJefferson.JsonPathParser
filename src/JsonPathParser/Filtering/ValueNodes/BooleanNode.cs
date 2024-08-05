using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Filtering.ValueNodes;

public class BooleanNode : TypedValueNode<bool>
{
    private readonly bool _value;

    public override bool Value => _value;

    public BooleanNode(string boolValue)
    {
        _value = bool.Parse(boolValue);
    }


    public override BooleanNode AsBooleanNode()
    {
        return this;
    }


    public override string ToString()
    {
        return _value.ToString().ToLower();
    }

    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }

    public override bool Equals(object? o)
    {
        if (this == o) return true;
        if (o is BooleanNode that)
        {
            return _value.Equals(that._value);
        }
        return false;
    }
}