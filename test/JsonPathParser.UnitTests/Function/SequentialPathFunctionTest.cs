using XavierJefferson.JsonPathParser.UnitTests.TestData;

namespace XavierJefferson.JsonPathParser.UnitTests.Function;

/**
 * Test cases for functions
 * 
 * -first
 * -last
 * -index(X)
 * 
 * Created by git9527 on 6/11/22.
 */
public class SequentialPathFunctionTest : BaseFunctionTest
{
    

    [Theory]

    [ClassData(typeof(ProviderTypeTestCases))]
    public void TestFirstOfNumbers(IProviderTypeTestCase testCase)
    {
        VerifyFunction(testCase.Configuration, "$.numbers.first()", NumberSeries, 1d);
    }

    [Theory]

    [ClassData(typeof(ProviderTypeTestCases))]
    public void TestLastOfNumbers(IProviderTypeTestCase testCase)
    {
        VerifyFunction(testCase.Configuration, "$.numbers.last()", NumberSeries, 10d);
    }


    [Theory]

    [ClassData(typeof(ProviderTypeTestCases))]
    public void TestIndexOfNumbers(IProviderTypeTestCase testCase)
    {
        VerifyFunction(testCase.Configuration, "$.numbers.index(0)", NumberSeries, 1d);
        VerifyFunction(testCase.Configuration, "$.numbers.index(-1)", NumberSeries, 10d);
        VerifyFunction(testCase.Configuration, "$.numbers.index(1)", NumberSeries, 2d);
    }


    [Theory]

    [ClassData(typeof(ProviderTypeTestCases))]
    public void TestFirstOfText(IProviderTypeTestCase testCase)
    {
        VerifyFunction(testCase.Configuration, "$.text.first()", TextSeries, "a");
    }


    [Theory]

    [ClassData(typeof(ProviderTypeTestCases))]
    public void TestLastOfText(IProviderTypeTestCase testCase)
    {
        VerifyFunction(testCase.Configuration, "$.text.last()", TextSeries, "f");
    }


    [Theory]

    [ClassData(typeof(ProviderTypeTestCases))]
    public void TestIndexOfTex(IProviderTypeTestCase testCase)
    {
        VerifyFunction(testCase.Configuration, "$.text.index(0)", TextSeries, "a");
        VerifyFunction(testCase.Configuration, "$.text.index(-1)", TextSeries, "f");
        VerifyFunction(testCase.Configuration, "$.text.index(1)", TextSeries, "b");
    }
}