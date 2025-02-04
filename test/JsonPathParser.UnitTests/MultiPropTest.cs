using XavierJefferson.JsonPathParser.Enums;
using XavierJefferson.JsonPathParser.Exceptions;
using XavierJefferson.JsonPathParser.UnitTests.Extensions;
using XavierJefferson.JsonPathParser.UnitTests.TestData;

namespace XavierJefferson.JsonPathParser.UnitTests;

public class MultiPropTest : TestUtils
{
    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void MultiPropCanBeReadFromRoot(IProviderTypeTestCase testCase)
    {
        var model = new Dictionary<string, object?>
        {
            { "a", "a-val" },
            { "b", "b-val" },
            { "c", "c-val" }
        };

        var conf = testCase.Configuration;

        var n = JsonPath.Using(conf).Parse(model).Read<IDictionary<string, object?>>("$['a', 'b']");
        MyAssert.ContainsEntry(n, "a", "a-val");
        MyAssert.ContainsEntry(n, "b", "b-val");

        // current semantics: absent props are skipped
        var o = JsonPath.Using(conf).Parse(model).Read<IDictionary<string, object?>>("$['a', 'd']");
        Assert.Single(o);
        MyAssert.ContainsEntry(o, "a", "a-val");
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void MultiPropsCanBeDefaultedToNull(IProviderTypeTestCase testCase)
    {
        var model = new Dictionary<string, object?>
        {
            { "a", "a-val" },
            { "b", "b-val" },
            { "c", "c-val" }
        };

        var conf = testCase.Configuration.AddOptions(ConfigurationOptionEnum.DefaultPathLeafToNull);

        var n = JsonPath.Using(conf).Parse(model).Read<IDictionary<string, object?>>("$['a', 'd']");
        MyAssert.ContainsEntry(n, "a", "a-val");
        MyAssert.ContainsEntry(n, "d", null);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void MultiPropsCanBeRequired(IProviderTypeTestCase testCase)
    {
        var model = new Dictionary<string, object?>
        {
            { "a", "a-val" },
            { "b", "b-val" },
            { "c", "c-val" }
        };

        var conf = testCase.Configuration.AddOptions(ConfigurationOptionEnum.RequireProperties);

        Assert.Throws<PathNotFoundException>(() =>
            JsonPath.Using(conf).Parse(model).Read("$['a', 'x']", TypeConstants.DictionaryType));
    }

    [Fact]
    public void MultiPropsCanBeNonLeafs()
    {
        var result = JsonPath.Parse("{\"a\": {\"v\": 5}, \"b\": {\"v\": 4}, \"c\": {\"v\": 1}}").Read(
            "$['a', 'c'].v");
        MyAssert.ContainsOnly(result.AsList(), 5d, 1d);
    }

    [Fact]
    public void NonexistentNonLeafMultiPropsIgnored()
    {
        var result = JsonPath.Parse("{\"a\": {\"v\": 5}, \"b\": {\"v\": 4}, \"c\": {\"v\": 1}}").Read(
            "$['d', 'a', 'c', 'm'].v");
        MyAssert.ContainsOnly(result.AsList(), 5d, 1d);
    }

    [Fact]
    public void MultiPropsWithPostFilter()
    {
        var result = JsonPath.Parse("{\"a\": {\"v\": 5}, \"b\": {\"v\": 4}, \"c\": {\"v\": 1, \"flag\": true}}").Read(
            "$['a', 'c'][?(@.flag)].v");
        MyAssert.ContainsOnly(result.AsList(), 1d);
    }

    [Fact]
    public void DeepScanDoesNotAffectNonLeafMultiProps()
    {
        // deep scan + multiprop is quite redundant scenario, but it's not forbidden, so we'd better check
        var json = "{\"v\": [[{}, 1, {\"a\": {\"v\": 5}, \"b\": {\"v\": 4}, \"c\": {\"v\": 1, \"flag\": true}}]]}";
        var result = JsonPath.Parse(json).Read("$..['a', 'c'].v");
        MyAssert.ContainsOnly(result.AsList(), 5d, 1d);

        result = JsonPath.Parse(json).Read("$..['a', 'c'][?(@.flag)].v");
        MyAssert.ContainsOnly(result.AsList(), 1d);
    }

    [Theory]
    [InlineData("$.x[1]['a', 'c'].v")]
    [InlineData("$.x[*]['a', 'c'].v")]
    [InlineData("$[*][*]['a', 'c'].v")]
    [InlineData("$.x[1]['d', 'a', 'c', 'm'].v")]
    [InlineData("$.x[*]['d', 'a', 'c', 'm'].v")]
    public void MultiPropsCanBeInTheMiddle(string path)
    {
        const string json = "{\"x\": [null, {\"a\": {\"v\": 5}, \"b\": {\"v\": 4}, \"c\": {\"v\": 1}}]}";
        var result = JsonPath.Parse(json).Read(path);
        MyAssert.ContainsOnly(result.AsList(), 5d, 1d);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void NonLeafMultiPropsCanBeRequired(IProviderTypeTestCase testCase)
    {
        var conf = testCase.Configuration.AddOptions(ConfigurationOptionEnum.RequireProperties);
        var json = "{\"a\": {\"v\": 5}, \"b\": {\"v\": 4}, \"c\": {\"v\": 1}}";

        MyAssert.ContainsOnly(JsonPath.Using(conf).Parse(json).Read<List<object?>>("$['a', 'c'].v"), 5d, 1d);
        MyAssert.EvaluationThrows<PathNotFoundException>(json, "$['d', 'a', 'c', 'm'].v", conf);
    }
}