namespace XavierJefferson.JsonPathParser.Function.Numeric;

/// <summary>
///     Defines the summation of a series of JSONArray numerical values
/// </summary>
public class Min : AbstractAggregation
{
    private double _minimum = double.MaxValue;


    protected override void Next(double value)
    {
        if (_minimum > value) _minimum = value;
    }


    protected override double GetValue()
    {
        return _minimum;
    }
}