using XavierJefferson.JsonPathParser.Enums;
using XavierJefferson.JsonPathParser.Exceptions;
using XavierJefferson.JsonPathParser.UnitTests.Extensions;
using XavierJefferson.JsonPathParser.UnitTests.TestData;

namespace XavierJefferson.JsonPathParser.UnitTests;

public class OptionsTest : TestUtils
{
    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void a_leafs_is_not_defaulted_to_null(IProviderTypeTestCase testCase)
    {
        var conf = testCase.Configuration;

        Assert.Throws<PathNotFoundException>(() => JsonPath.Using(conf).Parse("{\"foo\" : \"bar\"}").Read("$.baz"));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void a_leafs_can_be_defaulted_to_null(IProviderTypeTestCase testCase)
    {
        var conf = testCase.Configuration.SetOptions(ConfigurationOptionEnum.DefaultPathLeafToNull);

        Assert.Null(JsonPath.Using(conf).Parse("{\"foo\" : \"bar\"}").Read<object?>("$.baz"));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void a_definite_path_is_not_returned_as_list_by_default(IProviderTypeTestCase testCase)
    {
        var conf = testCase.Configuration;

        Assert.IsType<string>(JsonPath.Using(conf).Parse("{\"foo\" : \"bar\"}").Read("$.foo"));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void a_definite_path_can_be_returned_as_list(IProviderTypeTestCase testCase)
    {
        var conf = testCase.Configuration.SetOptions(ConfigurationOptionEnum.AlwaysReturnList);
        Assert.IsType<List<object?>>(JsonPath.Using(conf).Parse("{\"foo\" : \"bar\"}").Read("$.foo"));

        Assert.IsType<List<object?>>(JsonPath.Using(conf).Parse("{\"foo\": null}").Read("$.foo"));
        var aa = JsonPath.Using(conf).Parse("{\"foo\": [1, 4, 8]}").Read("$.foo").AsList();
        MyAssert.ContainsExactly(aa[0].AsList(), 1d, 4d, 8d);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void an_indefinite_path_can_be_returned_as_list(IProviderTypeTestCase testCase)
    {
        var conf = testCase.Configuration.SetOptions(ConfigurationOptionEnum.AlwaysReturnList);
        var result = JsonPath.Using(conf).Parse("{\"bar\": {\"foo\": null}}").Read("$..foo").AsList();
        Assert.Single(result);
        Assert.Null(result[0]);

        MyAssert.ContainsExactly(
            JsonPath.Using(conf).Parse("{\"bar\": {\"foo\": [1, 4, 8]}}").Read("$..foo").AsList()[0].AsList(), 1d, 4d,
            8d);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void a_path_evaluation_is_returned_as_VALUE_by_default(IProviderTypeTestCase testCase)
    {
        var conf = testCase.Configuration;

        Assert.Equal("bar", (string)JsonPath.Using(conf).Parse("{\"foo\" : \"bar\"}").Read("$.foo"));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void a_path_evaluation_can_be_returned_as_PATH_LIST(IProviderTypeTestCase testCase)
    {
        var conf = testCase.Configuration.SetOptions(ConfigurationOptionEnum.AsPathList);

        var pathList = JsonPath.Using(conf).Parse("{\"foo\" : \"bar\"}").Read("$.foo").AsList();

        MyAssert.ContainsOnly(pathList, "$['foo']");
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void multi_properties_are_merged_by_default(IProviderTypeTestCase testCase)
    {
        var model = new Dictionary<string, object?>
        {
            //{ "a", "a"},
            { "a", "a" },
            { "b", "b" },
            { "c", "c" }
        };


        var conf = testCase.Configuration;

        var result = JsonPath.Using(conf).Parse(model).Read<IDictionary<string, object?>>("$.['a', 'b']");

        //Assert.True(result).isInstanceOf(Json123.Constants.ListType);
        //Assert.True(result).containsOnly("a", "b");
        MyAssert.ContainsEntry(result, "a", "a");
        MyAssert.ContainsEntry(result, "b", "b");
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void when_property_is_required_exception_is_thrown(IProviderTypeTestCase testCase)
    {
        var model = new ObjectList(GetSingletonMap("a", "a-val"), GetSingletonMap("b", "b-val"));

        var conf = testCase.Configuration;

        MyAssert.ContainsExactly(JsonPath.Using(conf).Parse(model).Read<List<object?>>("$[*].a"), "a-val");


        conf = conf.AddOptions(ConfigurationOptionEnum.RequireProperties);

        Assert.Throws<PathNotFoundException>(() =>
        {
            JsonPath.Using(conf).Parse(model).Read("$[*].a", TypeConstants.ListType);
        });
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void when_property_is_required_exception_is_thrown_2(IProviderTypeTestCase testCase)
    {
        var model = new Dictionary<string, object?>
        {
            { "a", GetSingletonMap("a-key", "a-val") },
            { "b", GetSingletonMap("b-key", "b-val") }
        };


        var conf = testCase.Configuration;

        MyAssert.ContainsExactly(JsonPath.Using(conf).Parse(model).Read<List<object?>>("$.*.a-key"), "a-val");


        conf = conf.AddOptions(ConfigurationOptionEnum.RequireProperties);
        Assert.Throws<PathNotFoundException>(() =>
        {
            JsonPath.Using(conf).Parse(model).Read("$.*.a-key", TypeConstants.ListType);
        });
    }


    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void issue_suppress_exceptions_does_not_break_indefinite_evaluation(IProviderTypeTestCase testCase)
    {
        var conf = testCase.Configuration.SetOptions(ConfigurationOptionEnum.SuppressExceptions);

        MyAssert.ContainsOnly(JsonPath.Using(conf).Parse("{\"foo2\": [5]}").Read("$..foo2[0]").AsList(), 5d);
        Assert.True(JsonPath.Using(conf).Parse("{\"foo\" : {\"foo2\": [5]}}").Read("$..foo2[0]").AsList()
            .ContainsOnly(5d));
        Assert.True(JsonPath.Using(conf).Parse("[null, [{\"foo\" : {\"foo2\": [5]}}]]").Read("$..foo2[0]").AsList()
            .ContainsOnly(5d));

        Assert.True(JsonPath.Using(conf).Parse("[null, [{\"foo\" : {\"foo2\": [5]}}]]").Read("$..foo.foo2[0]").AsList()
            .ContainsOnly(5d));

        Assert.True(JsonPath.Using(conf).Parse("{\"aoo\" : {}, \"foo\" : {\"foo2\": [5]}, \"zoo\" : {}}")
            .Read("$[*].foo2[0]").AsList().ContainsOnly(5d));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void isbn_is_defaulted_when_option_is_provided(IProviderTypeTestCase testCase)
    {
        var result1 = JsonPath.Using(testCase.Configuration).Parse(JsonTestData.JsonDocument)
            .Read<List<object?>>("$.store.book.*.isbn");

        MyAssert.ContainsExactly(result1, "0-553-21311-3", "0-395-19395-8");

        var result2 = JsonPath
            .Using(testCase.Configuration.AddOptions(ConfigurationOptionEnum.DefaultPathLeafToNull))
            .Parse(JsonTestData.JsonDocument).Read<List<object?>>("$.store.book.*.isbn");

        MyAssert.ContainsExactly(result2, null, null, "0-553-21311-3", "0-395-19395-8");
    }
}