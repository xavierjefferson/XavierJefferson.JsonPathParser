using XavierJefferson.JsonPathParser.Interfaces;
using XavierJefferson.JsonPathParser.Logging;
using XavierJefferson.JsonPathParser.UnitTests.TestData;

namespace XavierJefferson.JsonPathParser.UnitTests.Function;

public class NestedFunctionTest : BaseFunctionTest
{
    private static readonly ILog Logger = LoggerFactory.Logger(typeof(NumericPathFunctionTest));

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void TestParameterAverageFunctionCall(IProviderTypeTestCase testCase)
    {
        VerifyMathFunction(testCase.Configuration, "$.avg($.numbers.min(), $.numbers.max())", 5.5);
    }


    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void TestArrayAverageFunctionCall(IProviderTypeTestCase testCase)
    {
        VerifyMathFunction(testCase.Configuration, "$.numbers.avg()", 5.5);
    }

    /**
     * This test calculates the following:
     * <p>
     *     For each number in $.numbers 1 -> 10 Add each number up,
     *     then Add 1 (min), 10 (max)
     *     <p>
     *         Alternatively 1+2+3+4+5+6+7+8+9+10+1+10 == 66
     */
    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void TestArrayAverageFunctionCallWithParameters(IProviderTypeTestCase testCase)
    {
        VerifyMathFunction(testCase.Configuration, "$.numbers.sum($.numbers.min(), $.numbers.max())", 66.0);
    }


    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void TestJsonInnerArgumentArray(IProviderTypeTestCase testCase)
    {
        VerifyMathFunction(testCase.Configuration, "$.sum(5, 3, $.numbers.max(), 2)", 20.0);
    }


    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void TestSimpleLiteralArgument(IProviderTypeTestCase testCase)
    {
        VerifyMathFunction(testCase.Configuration, "$.sum(5)", 5.0);
        VerifyMathFunction(testCase.Configuration, "$.sum(50)", 50.0);
    }


    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void TestStringConcat(IProviderTypeTestCase testCase)
    {
        VerifyTextFunction(testCase.Configuration, "$.text.concat()", "abcdef");
    }


    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void TestStringAndNumberConcat(IProviderTypeTestCase testCase)
    {
        VerifyTextAndNumberFunction(testCase.Configuration, "$.concat($.text[0], $.numbers[0])", "a1");
    }


    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void TestStringConcatWithJsonParameter(IProviderTypeTestCase testCase)
    {
        VerifyTextFunction(testCase.Configuration, "$.text.concat(\"-\", \"ghijk\")", "abcdef-ghijk");
    }


    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void TestAppendNumber(IProviderTypeTestCase testCase)
    {
        VerifyMathFunction(testCase.Configuration,
            "$.numbers.append(11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 0).avg()", 10.0);
    }

    /**
     * Aggregation function should ignore text values
     */
    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void TestAppendTextAndNumberThenSum(IProviderTypeTestCase testCase)
    {
        VerifyMathFunction(testCase.Configuration, "$.numbers.append(\"0\", \"11\").sum()", 55.0);
    }


    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void TestErrantCloseBraceNegative(IProviderTypeTestCase testCase)
    {
        var e = Assert.ThrowsAny<Exception>(() =>
        {
            JsonPath.Using(testCase.Configuration).Parse(NumberSeries)
                .Read("$.numbers.append(0, 1, 2}).avg()");
        });
        Assert.StartsWith("Unexpected close brace", e.Message);
    }


    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void TestErrantCloseBracketNegative(IProviderTypeTestCase testCase)
    {
        var e = Assert.ThrowsAny<Exception>(() =>
        {
            JsonPath.Using(testCase.Configuration).Parse(NumberSeries)
                .Read("$.numbers.append(0, 1, 2]).avg()");
        });
        Assert.StartsWith("Unexpected close bracket", e.Message);
    }


    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void TestUnclosedFunctionCallNegative(IProviderTypeTestCase testCase)
    {
        var e = Assert.ThrowsAny<Exception>(() =>
        {
            JsonPath.Using(testCase.Configuration).Parse(NumberSeries).Read("$.numbers.append(0, 1, 2");
        });
        Assert.StartsWith("Arguments to function: 'append'", e.Message);
    }
}