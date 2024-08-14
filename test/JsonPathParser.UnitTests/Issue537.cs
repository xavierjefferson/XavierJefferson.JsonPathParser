using XavierJefferson.JsonPathParser.Extensions;
using XavierJefferson.JsonPathParser.UnitTests.Extensions;
using XavierJefferson.JsonPathParser.UnitTests.Internal;
using XavierJefferson.JsonPathParser.UnitTests.TestData;

namespace XavierJefferson.JsonPathParser.UnitTests;

public class Issue537 : TestBase
{
    private static Configuration GetConfiguration(IProviderTypeTestCase testCase)
    {
        return testCase.Configuration.AddOptions(Option.SuppressExceptions);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void test_read(IProviderTypeTestCase testCase)
    {
        // originally passed
        var ans = JsonPath.Using(GetConfiguration(testCase)).Parse("{}").Read("missing");
        Assert.Null(ans);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void test_renameKey(IProviderTypeTestCase testCase)
    {
        // originally 
        var ans = JsonPath.Using(GetConfiguration(testCase))
            .Parse("{\"list\":[" +
                   "{\"data\":{\"old\":1}}," +
                   "{\"data\":{}}," +
                   "{\"data\":{\"old\":2}}" +
                   "]}")
            .RenameKey("$..data", "old", "new")
            .Read("$.list").AsListOfMap();
        Assert.Equal(3, ans.Count);
        Assert.True(ans[0].DeepEquals(GetSingletonMap("data", GetSingletonMap("new", 1d))));
        Assert.True(ans[1].DeepEquals(GetSingletonMap("data", new Dictionary<string, object?>())));
        Assert.True(ans[2].DeepEquals(GetSingletonMap("data", GetSingletonMap("new", 2d))));
        //Assert.Equal("[{\"data\":{\"new\":1},{\"data\":{},{\"data\":{\"new\":2}]", ans.ToString());
    }
}