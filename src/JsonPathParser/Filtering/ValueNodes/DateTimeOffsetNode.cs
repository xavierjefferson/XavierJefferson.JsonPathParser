namespace XavierJefferson.JsonPathParser.Filtering.ValueNodes;

public class DateTimeOffsetNode : TypedValueNode<DateTimeOffset>
{
    private readonly DateTimeOffset _dateTime;

    public DateTimeOffsetNode(DateTimeOffset dateTime)
    {
        _dateTime = dateTime;
    }

    public DateTimeOffsetNode(string date)
    {
        _dateTime = DateTimeOffset.Parse(date);
    }

    public override DateTimeOffset Value => _dateTime;

    public override int GetHashCode()
    {
        return _dateTime.GetHashCode();
    }


    public override StringNode AsStringNode()
    {
        return new StringNode(_dateTime.ToString(), false);
    }

    [Obsolete]
    public DateTimeOffset GetDate()
    {
        return _dateTime;
    }


    public override DateTimeOffsetNode AsDateTimeOffsetNode()
    {
        return this;
    }


    public override string ToString()
    {
        return _dateTime.ToString();
    }


    public override bool Equals(object? o)
    {
        if (this == o) return true;
        if (!(o is DateTimeOffsetNode) && !(o is StringNode)) return false;
        var that = ((ValueNode)o).AsDateTimeOffsetNode();
        return _dateTime.CompareTo(that._dateTime) == 0;
    }
}