using XavierJefferson.JsonPathParser.Exceptions;
using XavierJefferson.JsonPathParser.UnitTests.Extensions;
using XavierJefferson.JsonPathParser.UnitTests.TestData;

namespace XavierJefferson.JsonPathParser.UnitTests;

/**
* Deep scan is indefinite, so certain "illegal" actions become a no-op instead of a path evaluation exception.
*/
public class DeepScanTest : TestUtils
{
    [Fact]
    public void when_deep_scanning_non_array_subscription_is_ignored()
    {
        var result = JsonPath.Parse("{\"x\": [0,1,[0,1,2,3,null],null]}").Read("$..[2][3]");
        MyAssert.ContainsOnly(result.AsList(), 3d);
        result = JsonPath.Parse("{\"x\": [0,1,[0,1,2,3,null],null], \"y\": [0,1,2]}").Read("$..[2][3]");
        MyAssert.ContainsOnly(result.AsList(), 3d);

        result = JsonPath.Parse("{\"x\": [0,1,[0,1,2],null], \"y\": [0,1,2]}").Read("$..[2][3]");
        Assert.Empty(result.AsList());
    }

    [Fact]
    public void when_deep_scanning_null_subscription_is_ignored()
    {
        var result = JsonPath.Parse("{\"x\": [null,null,[0,1,2,3,null],null]}").Read("$..[2][3]");
        MyAssert.ContainsOnly(result.AsList(), 3d);
        result = JsonPath.Parse("{\"x\": [null,null,[0,1,2,3,null],null], \"y\": [0,1,null]}").Read("$..[2][3]");
        MyAssert.ContainsOnly(result.AsList(), 3d);
    }

    [Fact]
    public void when_deep_scanning_array_index_oob_is_ignored()
    {
        var result = JsonPath.Parse("{\"x\": [0,1,[0,1,2,3,10],null]}").Read("$..[4]");
        MyAssert.ContainsOnly(result.AsList(), 10d);

        result = JsonPath.Parse("{\"x\": [null,null,[0,1,2,3]], \"y\": [null,null,[0,1]]}").Read("$..[2][3]");
        MyAssert.ContainsOnly(result.AsList(), 3d);
    }

    [Theory]
    [InlineData("{\"foo\": {\"bar\": null}}", "$.foo.bar.[5]")]
    [InlineData("{\"foo\": {\"bar\": null}}", "$.foo.bar.[5, 10]")]
    [InlineData("{\"foo\": {\"bar\": 4}}", "$.foo.bar.[5]")]
    [InlineData("{\"foo\": {\"bar\": 4}}", "$.foo.bar.[5, 10]")]
    [InlineData("{\"foo\": {\"bar\": []}}", "$.foo.bar.[5]")]
    public void definite_upstream_illegal_array_access_throws(string input, string path)
    {
        var testCase = ProviderTypeTestCases.RootData.First().Value;
        MyAssert.EvaluationThrows<PathNotFoundException>(input, path, testCase);
    }

    [Fact]
    public void when_deep_scanning_illegal_property_access_is_ignored()
    {
        const string json = "{\"x\": {\"foo\": {\"bar\": 4}}, \"y\": {\"foo\": 1}}";
        var result = JsonPath.Parse(json).Read("$..foo");
        Assert.Equal(2, result.AsList().Count());

        result = JsonPath.Parse(json).Read("$..foo.bar");
        MyAssert.ContainsOnly(result.AsList(), 4d);
        result = JsonPath.Parse(json).Read("$..[*].foo.bar");
        MyAssert.ContainsOnly(result.AsList(), 4d);
        result = JsonPath.Parse("{\"x\": {\"foo\": {\"baz\": 4}}, \"y\": {\"foo\": 1}}").Read("$..[*].foo.bar");
        Assert.Empty(result.AsList());
    }

    [Theory]
    [InlineData("$..foo[?(@.bar)].bar")]
    [InlineData("$..[*]foo[?(@.bar)].bar")]
    public void when_deep_scanning_illegal_predicate_is_ignored(string path)
    {
        const string json = "{\"x\": {\"foo\": {\"bar\": 4}}, \"y\": {\"foo\": 1}}";
        var result = JsonPath.Parse(json).Read<List<object?>>(path);
        MyAssert.ContainsOnly(result, 4d);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void when_deep_scanning_require_properties_is_ignored_on_scan_target(IProviderTypeTestCase testCase)
    {
        var conf = testCase.Configuration.AddOptions(Option.RequireProperties);

        var result = JsonPath.Parse("[{\"x\": {\"foo\": {\"x\": 4}, \"x\": null}, \"y\": {\"x\": 1}}, {\"x\": []}]")
            .Read<List<object?>>(
                "$..x");
        Assert.Equal(5, result.Count());


        var result1 = JsonPath.Using(conf).Parse("{\"foo\": {\"bar\": 4}}").Read<List<object?>>("$..foo.bar");
        MyAssert.ContainsExactly(result1, 4d);

        MyAssert.EvaluationThrows<PathNotFoundException>("{\"foo\": {\"baz\": 4}}", "$..foo.bar", conf);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void when_deep_scanning_require_properties_is_ignored_on_scan_target_but_not_on_children(
        IProviderTypeTestCase testCase)
    {
        var conf = testCase.Configuration.AddOptions(Option.RequireProperties);

        MyAssert.EvaluationThrows<PathNotFoundException>("{\"foo\": {\"baz\": 4}}", "$..foo.bar", conf);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void when_deep_scanning_leaf_multi_props_work(IProviderTypeTestCase testCase)
    {
        var result1 = JsonPath
            .Parse("[{\"a\": \"a-val\", \"b\": \"b-val\", \"c\": \"c-val\"}, [1, 5], {\"a\": \"a-val\"}]").Read(
                "$..['a', 'c']");
        // This is current deep scan semantics: only objects containing all properties specified in multiprops token are
        // considered.
        Assert.Equal(1, result1.AsList().Count());
        var result = result1.AsList()[0] as IDictionary<string, object?>;
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Equal(new Dictionary<string, object?> { { "a", "a-val" }, { "c", "c-val" } }, result);


        //Assert.True((JpDictionary)result).hasSize(2).ContainsEntry("a", "a-val").ContainsEntry("c", "c-val");

        // But this semantics changes when DEFAULT_PATH_LEAF_TO_NULL comes into play.
        var conf = testCase.Configuration.AddOptions(Option.DefaultPathLeafToNull);
        var result2 = JsonPath.Using(conf)
            .Parse("[{\"a\": \"a-val\", \"b\": \"b-val\", \"c\": \"c-val\"}, [1, 5], {\"a\": \"a-val\"}]")
            .Read<List<object?>>(
                "$..['a', 'c']");
        // todo: deep equality test, but not tied to any json provider
        Assert.Equal(2, result2.Count());
        foreach (var node in result2)
        {
            var dict = node as IDictionary<string, object?>;
            Assert.NotNull(dict);
            MyAssert.ContainsKey(dict, "a");
            Assert.Equal("a-val", dict["a"]);
        }
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void require_single_property_ok(IProviderTypeTestCase testCase)
    {
        var json = new List<IDictionary<string, object?>>
        {
            GetSingletonMap("a", "a0"),
            GetSingletonMap("a", "a1")
        };

        var configuration = testCase.Configuration.AddOptions(Option.RequireProperties);

        var result = JsonPath.Using(configuration).Parse(json).Read("$..a");

        MyAssert.ContainsExactly(result.AsList(), "a0", "a1");
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void require_single_property(IProviderTypeTestCase testCase)
    {
        var json = new List<object?>
        {
            GetSingletonMap("a", "a0"),
            GetSingletonMap("b", "b2")
        };

        var configuration = testCase.Configuration.AddOptions(Option.RequireProperties);

        var result = JsonPath.Using(configuration).Parse(json).Read("$..a");

        MyAssert.ContainsExactly(result.AsList(), "a0");
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void require_multi_property_all_match(IProviderTypeTestCase testCase)
    {
        var ab = new Dictionary<string, object?>
        {
            { "a", "aa" },
            { "b", "bb" }
        };

        var json = new List<object?>
        {
            ab,
            ab
        };

        var configuration = testCase.Configuration.AddOptions(Option.RequireProperties);

        var result = JsonPath.Using(configuration).Parse(json).Read("$..['a', 'b']").AsListOfMap();

        MyAssert.ContainsExactly(result, ab, ab);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void require_multi_property_some_match(IProviderTypeTestCase testCase)
    {
        var ab = new Dictionary<string, object?>
        {
            { "a", "aa" },
            { "b", "bb" }
        };

        var ad = new Dictionary<string, object?>
        {
            { "a", "aa" },
            { "d", "dd" }
        };

        var json = new List<object?>
        {
            ab,
            ad
        };

        var configuration = testCase.Configuration.AddOptions(Option.RequireProperties);

        var result = JsonPath.Using(configuration).Parse(json).Read("$..['a', 'b']").AsListOfMap();

        MyAssert.ContainsExactly(result, ab);
    }

    [Fact]
    public void scan_for_single_property()
    {
        var a = new Dictionary<string, object?>
        {
            { "a", "aa" }
        };
        var b = new Dictionary<string, object?>
        {
            { "b", "bb" }
        };
        var ab = new Dictionary<string, object?>
        {
            { "a", a },
            { "b", b }
        };
        var bAb = new Dictionary<string, object?>
        {
            { "b", b },
            { "ab", ab }
        };
        var json = new List<object?>
        {
            a,
            b,
            bAb
        };
        MyAssert.ContainsExactly(JsonPath.Parse(json).Read<List<object?>>("$..['a']"), "aa", a, "aa");
    }

    [Fact]
    public void scan_for_property_path()
    {
        var a = new Dictionary<string, object?>
        {
            { "a", "aa" }
        };
        var x = new Dictionary<string, object?>
        {
            { "x", "xx" }
        };
        var y = new Dictionary<string, object?>
        {
            { "a", x }
        };
        var z = new Dictionary<string, object?>
        {
            { "z", y }
        };
        var json = new List<object?>
        {
            a,
            x,
            y,
            z
        };
        MyAssert.ContainsExactly(JsonPath.Parse(json).Read<List<object?>>("$..['a'].x"), "xx", "xx");
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void scan_for_property_path_missing_required_property(IProviderTypeTestCase testCase)
    {
        var a = new Dictionary<string, object?>
        {
            { "a", "aa" }
        };
        var x = new Dictionary<string, object?>
        {
            { "x", "xx" }
        };
        var y = new Dictionary<string, object?>
        {
            { "a", x }
        };
        var z = new Dictionary<string, object?>
        {
            { "z", y }
        };
        var json = new List<object?>
        {
            a,
            x,
            y,
            z
        };
        Assert.True(JsonPath.Using(testCase.Configuration.AddOptions(Option.RequireProperties))
            .Parse(json)
            .Read<List<object?>>("$..['a'].x").ContainsExactly("xx", "xx"));
    }


    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void scans_can_be_filtered(IProviderTypeTestCase testCase)
    {
        var brown = GetSingletonMap("val", "brown");
        var white = GetSingletonMap("val", "white");

        var cow = new Dictionary<string, object?>
        {
            { "mammal", true },
            { "color", brown }
        };
        var dog = new Dictionary<string, object?>
        {
            { "mammal", true },
            { "color", white }
        };
        var frog = new Dictionary<string, object?>
        {
            { "mammal", false }
        };
        var animals = new List<object?>
        {
            cow,
            dog,
            frog
        };
        var jpObjectList = JsonPath
            .Using(testCase.Configuration.AddOptions(Option.RequireProperties)).Parse(animals)
            .Read<List<object?>>("$..[?(@.mammal == true)].color");
        MyAssert.ContainsExactly(jpObjectList, brown, white);
    }

    [Fact]
    public void scan_with_a_function_filter()
    {
        var result = JsonPath.Parse(JsonTestData.JsonDocument).Read<List<object?>>("$..*[?(@.length() > 5)]");
        Assert.Single(result);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void DeepScanPathDefault(IProviderTypeTestCase testCase)
    {
        ExecuteScanPath(testCase);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void DeepScanPathRequireProperties(IProviderTypeTestCase testCase)
    {
        ExecuteScanPath(testCase, Option.RequireProperties);
    }

    private void ExecuteScanPath(IProviderTypeTestCase testCase, params Option[] options)
    {
        var json = "{'index': 'index', 'data': {'array': [{ 'object1': { 'name': 'robert'} }]}}";
        var expected = new Dictionary<string, object?>
        {
            {
                "object1", new Dictionary<string, object?>
                {
                    { "name", "robert" }
                }
            }
        };


        var configuration = testCase.Configuration.SetOptions(options);

        var result = JsonPath
            .Using(configuration)
            .Parse(json)
            .Read("$..array[0]").AsList();
        Assert.Equal(expected, result[0]);
    }
}