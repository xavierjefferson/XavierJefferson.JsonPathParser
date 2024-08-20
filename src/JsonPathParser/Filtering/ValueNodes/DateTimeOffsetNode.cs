namespace XavierJefferson.JsonPathParser.Filtering.ValueNodes;

public class DateTimeOffsetNode : TypedValueNode<DateTimeOffset>
{
    private readonly DateTimeOffset _dateTimeOffset;

    public DateTimeOffsetNode(DateTimeOffset dateTimeOffset)
    {
        _dateTimeOffset = dateTimeOffset;
    }

    public DateTimeOffsetNode(string date)
    {
        _dateTimeOffset = DateTimeOffset.Parse(date);
    }

    public override DateTimeOffset Value => _dateTimeOffset;

    public override int GetHashCode()
    {
        return _dateTimeOffset.GetHashCode();
    }


    public override StringNode AsStringNode()
    {
        return new StringNode(_dateTimeOffset.ToString(), false);
    }

    [Obsolete]
    public DateTimeOffset GetDate()
    {
        return _dateTimeOffset;
    }

    public override DateTimeNode AsDateTimeNode()
    {
        return new DateTimeNode(_dateTimeOffset.DateTime);
    }

    public override DateTimeOffsetNode AsDateTimeOffsetNode()
    {
        return this;
    }


    public override string ToString()
    {
        return _dateTimeOffset.ToString();
    }


    public override bool Equals(object? o)
    {
        if (this == o) return true;
        if (o is DateTimeOffsetNode || o is StringNode)
        {
            var that = ((ValueNode)o).AsDateTimeOffsetNode();
            return _dateTimeOffset.CompareTo(that._dateTimeOffset) == 0;
        }

        return false;
    }
}