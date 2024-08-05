namespace XavierJefferson.JsonPathParser.UnitTests.Extensions;

public static class MathHelpers
{
    // Return the standard deviation of an array of Doubles.
    //
    // If the second argument is True, evaluate as a sample.
    // If the second argument is False, evaluate as a population.
    public static double StdDev(this IEnumerable<double> values,
        bool asSample)
    {
        // Get the mean.
        var mean = values.Select(i => Convert.ToDouble(i)).Sum() / values.Count();

        // Get the sum of the squares of the differences
        // between the values and the mean.
        var squaresQuery = values.Select(value => (value - mean) * (value - mean));
        var sumOfSquares = squaresQuery.Sum();

        if (asSample)
            return Math.Sqrt(sumOfSquares / (values.Count() - 1));
        return Math.Sqrt(sumOfSquares / values.Count());
    }
}