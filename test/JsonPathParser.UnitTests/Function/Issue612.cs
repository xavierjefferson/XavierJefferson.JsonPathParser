using XavierJefferson.JsonPathParser.Exceptions;
using XavierJefferson.JsonPathParser.UnitTests.TestData;

namespace XavierJefferson.JsonPathParser.UnitTests.Function;

public class Issue612
{
    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void Test(IProviderTypeTestCase testCase)
    {
        var config = testCase.Configuration.SetOptions(Option.SuppressExceptions);
        var json = "{\"1\":{\"2\":null}}";
        var documentContext = JsonPath.Using(config).Parse(json);
        var query = JsonPath.Compile("$.1.2.a.b.c");
        Assert.Null(documentContext.Read(query));
        Assert.Null(documentContext.Map(query, (a, _) => a));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void Test2(IProviderTypeTestCase testCase)
    {
        var config = testCase.Configuration;
        var json = "{\"1\":{\"2\":null}}";
        var documentContext = JsonPath.Using(config).Parse(json);
        var query = JsonPath.Compile("$.1.2.a.b.c");

        Assert.Throws<PathNotFoundException>(() => documentContext.Map(query, (a, _) => a));
    }
}