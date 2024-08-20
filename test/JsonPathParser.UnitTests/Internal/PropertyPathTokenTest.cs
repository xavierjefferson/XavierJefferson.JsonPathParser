using XavierJefferson.JsonPathParser.Enums;
using XavierJefferson.JsonPathParser.Exceptions;
using XavierJefferson.JsonPathParser.Interfaces;
using XavierJefferson.JsonPathParser.Logging;
using XavierJefferson.JsonPathParser.UnitTests.Extensions;
using XavierJefferson.JsonPathParser.UnitTests.TestData;

namespace XavierJefferson.JsonPathParser.UnitTests.Internal;

public class PropertyPathTokenTest
{
    private static readonly ILog Logger = LoggerFactory.GetLogger(typeof(PropertyPathTokenTest));

    private readonly string _simpleArray = "[" +
                                           "{\n" +
                                           "   \"foo\" : \"foo-val-0\",\n" +
                                           "   \"bar\" : \"bar-val-0\",\n" +
                                           "   \"baz\" : {\"baz-child\" : \"baz-child-val\"}\n" +
                                           "}," +
                                           "{\n" +
                                           "   \"foo\" : \"foo-val-1\",\n" +
                                           "   \"bar\" : \"bar-val-1\",\n" +
                                           "   \"baz\" : {\"baz-child\" : \"baz-child-val\"}\n" +
                                           "}" +
                                           "]";

    private readonly string _simpleMap = "{\n" +
                                         "   \"foo\" : \"foo-val\",\n" +
                                         "   \"bar\" : \"bar-val\",\n" +
                                         "   \"baz\" : {\"baz-child\" : \"baz-child-val\"}\n" +
                                         "}";


    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void property_not_found(IProviderTypeTestCase testCase)
    {
        //String result = JsonPath.Read(SIMPLE_MAP, "$.not-found");

        //Assert.Null(result);

        var configuration = testCase.Configuration.SetOptions(ConfigurationOptionEnum.SuppressExceptions);

        var json = "{\"a\":{\"b\":1,\"c\":2}";
        Assert.Null(JsonPath.Parse(_simpleMap, configuration).Read("$.not-found"));
    }

    [Fact]
    public void property_not_found_deep()
    {
        Assert.Throws<PathNotFoundException>(() => JsonPath.Read(_simpleMap, "$.foo.not-found"));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void property_not_found_option_throw(IProviderTypeTestCase testCase)
    {
        Assert.Throws<PathNotFoundException>(() =>
            JsonPath.Using(testCase.Configuration).Parse(_simpleMap).Read("$.not-found"));
    }

    [Fact]
    public void map_value_can_be_read_from_map()
    {
        var result = JsonPath.Read<string>(_simpleMap, "$.foo");

        Assert.Equal("foo-val", result);
    }

    [Fact]
    public void map_value_can_be_read_from_array()
    {
        var result = JsonPath.Read(_simpleArray, "$[*].foo").AsList();

        MyAssert.ContainsOnly(result, "foo-val-0", "foo-val-1");
    }

    [Fact]
    public void map_value_can_be_read_from_child_map()
    {
        var result = JsonPath.Read<string>(_simpleMap, "$.baz.baz-child");

        Assert.Equal("baz-child-val", result);
    }
}