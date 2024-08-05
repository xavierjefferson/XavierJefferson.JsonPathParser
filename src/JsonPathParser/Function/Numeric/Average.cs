namespace XavierJefferson.JsonPathParser.Function.Numeric;

/// <summary>
///     Provides the average of a series of numbers in a JSONArray
/// </summary>
public class Average : AbstractAggregation
{
    private double _count;

    private double _summation;


    protected override void Next(double value)
    {
        _count++;
        _summation += value;
    }


    protected override double GetValue()
    {
        if (_count != 0d) return _summation / _count;
        return 0;
    }
}