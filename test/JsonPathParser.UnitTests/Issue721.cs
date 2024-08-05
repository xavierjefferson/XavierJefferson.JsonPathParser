using XavierJefferson.JsonPathParser.UnitTests.Extensions;
using XavierJefferson.JsonPathParser.UnitTests.Internal;
using XavierJefferson.JsonPathParser.UnitTests.TestData;

namespace XavierJefferson.JsonPathParser.UnitTests;

public class Issue721 : TestBase
{
    public static Configuration JsonConf(IProviderTypeTestCase testCase)
    {
        return testCase.Configuration.AddOptions(Option.SuppressExceptions);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void test_delete_1(IProviderTypeTestCase testCase)
    {
        // originally 
        var dc = JsonPath.Using(JsonConf(testCase))
            .Parse("{\"top\": {\"middle\": null}}")
            .Delete(JsonPath.Compile("$.top.middle.bottom"));
        var ans = dc.Read<JpDictionary>("$");
        Assert.True(ans.Equals(GetSingletonMap("top", GetSingletonMap("middle", null))));
        //System.out.println(ans);
        //Assert.Equal("{top={middle=null}", ans.ToString());
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void test_delete_2(IProviderTypeTestCase testCase)
    {
        // originally passed
        var documentContext = JsonPath.Using(JsonConf(testCase))
            .Parse("[" +
                   "{\"top\": {\"middle\": null}}," +
                   "{\"top\": {\"middle\": {}  }}," +
                   "{\"top\": {\"middle\": {bottom: 2}  }}" +
                   "]");
        var dc = documentContext
            .Delete(JsonPath.Compile("$[*].top.middle.bottom"));
        var ans = dc.Read("$").AsListOfMap();
        //System.out.println(ans);
        Assert.Equal(3, ans.Count());
        Assert.True(ans[0].Equals(GetSingletonMap("top", GetSingletonMap("middle", null))));
        Assert.True(ans[1].Equals(GetSingletonMap("top", GetSingletonMap("middle", new JpDictionary()))));
        Assert.True(ans[2].Equals(GetSingletonMap("top", GetSingletonMap("middle", new JpDictionary()))));
        //Assert.Equal("[{\"top\":{\"middle\":null},{\"top\":{\"middle\":{}},{\"top\":{\"middle\":{}}]", ans.ToString());
    }
}