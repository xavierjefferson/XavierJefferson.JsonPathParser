using XavierJefferson.JsonPathParser.Exceptions;
using XavierJefferson.JsonPathParser.UnitTests.Extensions;
using XavierJefferson.JsonPathParser.UnitTests.TestData;

namespace XavierJefferson.JsonPathParser.UnitTests;

public class MultiPropTest : TestUtils
{
    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void multi_prop_can_be_read_from_root(IProviderTypeTestCase testCase)
    {
        var model = new JpDictionary
        {
            { "a", "a-val" },
            { "b", "b-val" },
            { "c", "c-val" }
        };

        var conf = testCase.Configuration;

        var n = JsonPath.Using(conf).Parse(model).Read<JpDictionary>("$['a', 'b']");
        MyAssert.ContainsEntry(n, "a", "a-val");
        MyAssert.ContainsEntry(n, "b", "b-val");

        // current semantics: absent props are skipped
        var o = JsonPath.Using(conf).Parse(model).Read<JpDictionary>("$['a', 'd']");
        Assert.Single(o);
        MyAssert.ContainsEntry(o, "a", "a-val");
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void multi_props_can_be_defaulted_to_null(IProviderTypeTestCase testCase)
    {
        var model = new JpDictionary
        {
            { "a", "a-val" },
            { "b", "b-val" },
            { "c", "c-val" }
        };

        var conf = testCase.Configuration.AddOptions(Option.DefaultPathLeafToNull);

        var n = JsonPath.Using(conf).Parse(model).Read<JpDictionary>("$['a', 'd']");
        MyAssert.ContainsEntry(n, "a", "a-val");
        MyAssert.ContainsEntry(n, "d", null);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void multi_props_can_be_required(IProviderTypeTestCase testCase)
    {
        var model = new JpDictionary
        {
            { "a", "a-val" },
            { "b", "b-val" },
            { "c", "c-val" }
        };

        var conf = testCase.Configuration.AddOptions(Option.RequireProperties);

        Assert.Throws<PathNotFoundException>(() =>
            JsonPath.Using(conf).Parse(model).Read("$['a', 'x']", Constants.MapType));
    }

    [Fact]
    public void multi_props_can_be_non_leafs()
    {
        var result = JsonPath.Parse("{\"a\": {\"v\": 5}, \"b\": {\"v\": 4}, \"c\": {\"v\": 1}}").Read(
            "$['a', 'c'].v");
        MyAssert.ContainsOnly(result.AsList(), 5d, 1d);
    }

    [Fact]
    public void nonexistent_non_leaf_multi_props_ignored()
    {
        var result = JsonPath.Parse("{\"a\": {\"v\": 5}, \"b\": {\"v\": 4}, \"c\": {\"v\": 1}}").Read(
            "$['d', 'a', 'c', 'm'].v");
        MyAssert.ContainsOnly(result.AsList(), 5d, 1d);
    }

    [Fact]
    public void multi_props_with_post_filter()
    {
        var result = JsonPath.Parse("{\"a\": {\"v\": 5}, \"b\": {\"v\": 4}, \"c\": {\"v\": 1, \"flag\": true}}").Read(
            "$['a', 'c'][?(@.flag)].v");
        MyAssert.ContainsOnly(result.AsList(), 1d);
    }

    [Fact]
    public void deep_scan_does_not_affect_non_leaf_multi_props()
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
    public void multi_props_can_be_in_the_middle(string path)
    {
        const string json = "{\"x\": [null, {\"a\": {\"v\": 5}, \"b\": {\"v\": 4}, \"c\": {\"v\": 1}}]}";
        var result = JsonPath.Parse(json).Read(path);
        MyAssert.ContainsOnly(result.AsList(), 5d, 1d);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void non_leaf_multi_props_can_be_required(IProviderTypeTestCase testCase)
    {
        var conf = testCase.Configuration.AddOptions(Option.RequireProperties);
        var json = "{\"a\": {\"v\": 5}, \"b\": {\"v\": 4}, \"c\": {\"v\": 1}}";

        MyAssert.ContainsOnly(JsonPath.Using(conf).Parse(json).Read<JpObjectList>("$['a', 'c'].v"), 5d, 1d);
        MyAssert.EvaluationThrows<PathNotFoundException>(json, "$['d', 'a', 'c', 'm'].v", conf);
    }
}