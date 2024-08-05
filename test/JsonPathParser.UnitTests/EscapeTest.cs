using XavierJefferson.JsonPathParser.Provider;
using XavierJefferson.JsonPathParser.UnitTests.Extensions;

namespace XavierJefferson.JsonPathParser.UnitTests;

public class EscapeTest : TestUtils
{
    [Fact]
    public void urls_are_not_escaped()
    {
        var json = "[" +
                   "\"https://a/b/1\"," +
                   "\"https://a/b/2\"," +
                   "\"https://a/b/3\"" +
                   "]";

        var resAsString = JsonPath.Using(new NewtonsoftJsonProvider()).Parse(json).Read("$").AsList();

        Assert.True(resAsString.Contains("https://a/b/1"));
    }
}