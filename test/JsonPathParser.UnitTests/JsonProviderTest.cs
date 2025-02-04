using XavierJefferson.JsonPathParser.UnitTests.TestData;

namespace XavierJefferson.JsonPathParser.UnitTests;

public class JsonProviderTest : TestUtils
{
    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void StringsAreUnwrapped(IProviderTypeTestCase testCase)
    {
        Assert.Equal("string-value",
            JsonPath.Using(testCase.Configuration).Parse(JsonTestData.JsonDocument)
                .Read("$.string-property", typeof(string)));
    }


    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void IntegersAreUnwrapped(IProviderTypeTestCase testCase)
    {
        Assert.Equal(int.MaxValue,
            JsonPath.Using(testCase.Configuration).Parse(JsonTestData.JsonDocument)
                .Read("$.int-max-property", typeof(int)));
    }


    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void IntsAreUnwrapped(IProviderTypeTestCase testCase)
    {
        Assert.Equal(int.MaxValue,
            JsonPath.Using(testCase.Configuration).Parse(JsonTestData.JsonDocument)
                .Read("$.int-max-property", typeof(int)));
    }
}