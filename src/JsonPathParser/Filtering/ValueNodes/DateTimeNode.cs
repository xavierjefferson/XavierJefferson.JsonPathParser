namespace XavierJefferson.JsonPathParser.Filtering.ValueNodes;

public class DateTimeNode : TypedValueNode<DateTime>
{
    private readonly DateTime _dateTime;

    public DateTimeNode(DateTime dateTime)
    {
        _dateTime = dateTime;
    }

    public DateTimeNode(string date)
    {
        _dateTime = DateTime.Parse(date);
    }

    public override DateTime Value => _dateTime;

    public override int GetHashCode()
    {
        return _dateTime.GetHashCode();
    }


    public override StringNode AsStringNode()
    {
        return new StringNode(_dateTime.ToString(), false);
    }

    [Obsolete]
    public DateTime GetDate()
    {
        return _dateTime;
    }

    public override DateTimeOffsetNode AsDateTimeOffsetNode()
    {
        return new DateTimeOffsetNode(new DateTimeOffset(_dateTime));
    }

    public override DateTimeNode AsDateTimeNode()
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
        if (o is DateTimeNode || o is StringNode)
        {
            var that = ((ValueNode)o).AsDateTimeNode();
            return _dateTime.CompareTo(that._dateTime) == 0;
        }

        return false;
    }
}