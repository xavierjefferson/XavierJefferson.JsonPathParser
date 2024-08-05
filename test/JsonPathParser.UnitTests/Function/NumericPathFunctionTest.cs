using XavierJefferson.JsonPathParser.Exceptions;
using XavierJefferson.JsonPathParser.Function.Numeric;
using XavierJefferson.JsonPathParser.UnitTests.TestData;

namespace XavierJefferson.JsonPathParser.UnitTests.Function;

/**
 * Defines functional tests around executing:
 * <p>
 *     - sum
 *     - avg
 *     - stddev
 *     <p>
 *         for each of the above, executes the test and verifies that the results are as expected based on a static input
 *         and static output.
 *         <p>
 *             Created by mattg on 6/26/15.
 */
public class NumericPathFunctionTest : BaseFunctionTest
{
    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void TestAverageOfDoubles(IProviderTypeTestCase testCase)
    {
        VerifyMathFunction(testCase.Configuration, "$.numbers.avg()", 5.5);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void TestAverageOfEmptyListNegative(IProviderTypeTestCase testCase)
    {
        var e = Assert.Throws<JsonPathException>(() =>
        {
            VerifyMathFunction(testCase.Configuration, "$.empty.avg()", null);
        });
        Assert.Equal(AbstractAggregation.EmptyArrayMessage, e.Message);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void TestSumOfDouble(IProviderTypeTestCase testCase)
    {
        VerifyMathFunction(testCase.Configuration, "$.numbers.sum()", 10d * (10d + 1d) / 2d);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void TestSumOfEmptyListNegative(IProviderTypeTestCase testCase)
    {
        var e = Assert.Throws<JsonPathException>(() =>
        {
            VerifyMathFunction(testCase.Configuration, "$.empty.sum()", null);
        });
        Assert.Equal(AbstractAggregation.EmptyArrayMessage, e.Message);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void TestMaxOfDouble(IProviderTypeTestCase testCase)
    {
        VerifyMathFunction(testCase.Configuration, "$.numbers.max()", 10d);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void TestMaxOfEmptyListNegative(IProviderTypeTestCase testCase)
    {
        var e = Assert.Throws<JsonPathException>(() =>
        {
            VerifyMathFunction(testCase.Configuration, "$.empty.max()", null);
        });
        Assert.Equal(AbstractAggregation.EmptyArrayMessage, e.Message);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void TestMinOfDouble(IProviderTypeTestCase testCase)
    {
        VerifyMathFunction(testCase.Configuration, "$.numbers.min()", 1d);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void TestMinOfEmptyListNegative(IProviderTypeTestCase testCase)
    {
        var e = Assert.Throws<JsonPathException>(() =>
        {
            VerifyMathFunction(testCase.Configuration, "$.empty.min()", null);
        });
        Assert.Equal(AbstractAggregation.EmptyArrayMessage, e.Message);
    }


    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void TestStdDevOfDouble(IProviderTypeTestCase testCase)
    {
        VerifyMathFunction(testCase.Configuration, "$.numbers.stddev()", 2.8722813232690143d);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void TestStddevOfEmptyListNegative(IProviderTypeTestCase testCase)
    {
        var e = Assert.Throws<JsonPathException>(() =>
        {
            VerifyMathFunction(testCase.Configuration, "$.empty.stddev()", null);
        });
        Assert.Equal(AbstractAggregation.EmptyArrayMessage, e.Message);
    }

    /**
     * Expect that for an invalid function name we'll get back the original input to the function
     */
    //    [Xunit.Fact]
    //    @Ignore
    //    public void testInvalidFunctionNameNegative() {
    //        JSONArray numberSeries = new JSONArray();
    //        numberSeries.addAll(Arrays.asList(1, 2, 3, 4, 5, 6, 7, 8, 9, 10));
    //        Xunit.Assert.Equal(numberSeries, JsonPath.Using(testCase.Configuration).Parse(NUMBER_SERIES).read("$.numbers.foo()"));
    //    }
}