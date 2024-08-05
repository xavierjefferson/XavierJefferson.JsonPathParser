namespace XavierJefferson.JsonPathParser.Function.Numeric;

/// <summary>
///     Defines the summation of a series of JSONArray numerical values
/// </summary>
public class Max : AbstractAggregation
{
    private double _max = double.MinValue;


    protected override void Next(double value)
    {
        if (_max < value) _max = value;
    }


    protected override double GetValue()
    {
        return _max;
    }
}