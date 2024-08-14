namespace XavierJefferson.JsonPathParser.Filtering.ValueNodes;

public class NumberNode : TypedValueNode<double>
{
    public static NumberNode Nan = new();

    private readonly double _value;

    private NumberNode()
    {
    }

    public NumberNode(double number)
    {
        _value = number;
    }

    public NumberNode(string num)
    {
        _value = double.Parse(num);
    }

    public override double Value => _value;

    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }

    public override StringNode AsStringNode()
    {
        return new StringNode(_value.ToString(), false);
    }

    [Obsolete]
    public double GetNumber()
    {
        return _value;
    }


    public override NumberNode AsNumberNode()
    {
        return this;
    }


    public override string ToString()
    {
        return _value.ToString();
    }


    public override bool Equals(object? o)
    {
        if (this == o) return true;
        if (!(o is NumberNode) && !(o is StringNode)) return false;

        var that = ((ValueNode)o).AsNumberNode();

        if (that == Nan) return false;

        //if (_number == null && that._number == null)
        //    return true;
        //if (_number == null || that._number == null)
        //    return false;
        return _value.CompareTo(that._value) == 0;
    }
}