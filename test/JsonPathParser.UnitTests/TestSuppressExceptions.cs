using XavierJefferson.JsonPathParser.Enums;
using XavierJefferson.JsonPathParser.Mapper;
using XavierJefferson.JsonPathParser.Provider;
using XavierJefferson.JsonPathParser.UnitTests.Extensions;
using XavierJefferson.JsonPathParser.UnitTests.TestData;

namespace XavierJefferson.JsonPathParser.UnitTests;

public class TestSuppressExceptions
{
    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void TestSuppressExceptionsIsRespected(IProviderTypeTestCase testCase)
    {
        var parseContext = JsonPath.Using(testCase.Configuration.SetOptions(ConfigurationOptionEnum.SuppressExceptions));
        var json = "{}";
        Assert.Null(parseContext.Parse(json).Read(JsonPath.Compile("$.missing")));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void TestSuppressExceptionsIsRespectedPath(IProviderTypeTestCase testCase)
    {
        var parseContext = JsonPath.Using(testCase.Configuration
            .SetOptions(ConfigurationOptionEnum.SuppressExceptions, ConfigurationOptionEnum.AsPathList));
        var json = "{}";

        var result = parseContext.Parse(json).Read(JsonPath.Compile("$.missing")).AsList();
        Assert.Empty(result);
    }
}