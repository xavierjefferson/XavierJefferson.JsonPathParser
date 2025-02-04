using XavierJefferson.JsonPathParser.UnitTests.TestData;

namespace XavierJefferson.JsonPathParser.UnitTests;

public class Issue487
{
    public static Configuration JsonConf(IProviderTypeTestCase testCase)
    {
        return testCase.Configuration;
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void TestReadWithComma1(IProviderTypeTestCase testCase)
    {
        // originally 
        var dc = JsonPath.Using(JsonConf(testCase))
            .Parse("{ \"key,\" : \"value\" }");
        var ans = dc.Read(JsonPath.Compile("$['key,']"));
        //System.out.println(ans);
        Assert.Equal("value", ans.ToString());
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void TestReadWithComma2(IProviderTypeTestCase testCase)
    {
        // originally passed
        var dc = JsonPath.Using(JsonConf(testCase))
            .Parse("{ \"key,\" : \"value\" }");
        var ans = dc.Read(JsonPath.Compile("$['key\\,']"));
        //System.out.println(ans);
        Assert.Equal("value", ans.ToString());
    }
}