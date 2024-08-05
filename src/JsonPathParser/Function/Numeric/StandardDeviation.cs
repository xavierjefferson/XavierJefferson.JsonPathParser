namespace XavierJefferson.JsonPathParser.Function.Numeric;

/// <summary>
///     Provides the standard deviation of a series of numbers
/// </summary>
public class StandardDeviation : AbstractAggregation
{
    private double _count;
    private double _sum;
    private double _sumSq;


    protected override void Next(double value)
    {
        _sum += value;
        _sumSq += value * value;
        _count++;
    }


    protected override double GetValue()
    {
        return Math.Sqrt(_sumSq / _count - _sum * _sum / _count / _count);
    }
}