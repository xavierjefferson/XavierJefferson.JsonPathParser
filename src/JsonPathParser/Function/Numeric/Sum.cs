namespace XavierJefferson.JsonPathParser.Function.Numeric;

/// <summary>
///     Defines the summation of a series of JSONArray numerical values
/// </summary>
/// Created by mattg on 6/26/15.
public class Sum : AbstractAggregation
{
    private double _summation;


    protected override void Next(double value)
    {
        _summation += value;
    }


    protected override double GetValue()
    {
        return _summation;
    }
}