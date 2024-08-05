using XavierJefferson.JsonPathParser.UnitTests.Extensions;
using XavierJefferson.JsonPathParser.UnitTests.TestData;

namespace XavierJefferson.JsonPathParser.UnitTests.Internal;

using Assert = MyAssert;

public class PredicatePathTokenTest : TestUtils
{
    private static object Array(Configuration configuration)
    {
        return configuration.JsonProvider.Parse(
            "[" +
            "{\n" +
            "   \"foo\" : \"foo-val-0\"\n" +
            "}," +
            "{\n" +
            "   \"foo\" : \"foo-val-1\"\n" +
            "}," +
            "{\n" +
            "   \"foo\" : \"foo-val-2\"\n" +
            "}," +
            "{\n" +
            "   \"foo\" : \"foo-val-3\"\n" +
            "}," +
            "{\n" +
            "   \"foo\" : \"foo-val-4\"\n" +
            "}," +
            "{\n" +
            "   \"foo\" : \"foo-val-5\"\n" +
            "}," +
            "{\n" +
            "   \"foo\" : \"foo-val-6\"\n" +
            "}" +
            "]");
    }

    private static object GetArray2(Configuration configuration)
    {
        var tmp = "[" +
                  "{\n" +
                  "   \"foo\" : \"foo-val-0\",\n" +
                  "   \"int\" : 0\n," +
                  "   \"decimal\" : 0.0\n" +
                  "}," +
                  "{\n" +
                  "   \"foo\" : \"foo-val-1\",\n" +
                  "   \"int\" : 1,\n" +
                  "   \"decimal\" : 0.1\n" +
                  "}," +
                  "{\n" +
                  "   \"foo\" : \"foo-val-2\",\n" +
                  "   \"int\" : 2,\n" +
                  "   \"decimal\" : 0.2\n" +
                  "}," +
                  "{\n" +
                  "   \"foo\" : \"foo-val-3\",\n" +
                  "   \"int\" : 3,\n" +
                  "   \"decimal\" : 0.3\n" +
                  "}," +
                  "{\n" +
                  "   \"foo\" : \"foo-val-4\",\n" +
                  "   \"int\" : 4,\n" +
                  "   \"decimal\" : 0.4\n" +
                  "}," +
                  "{\n" +
                  "   \"foo\" : \"foo-val-5\",\n" +
                  "   \"int\" : 5,\n" +
                  "   \"decimal\" : 0.5\n" +
                  "}," +
                  "{\n" +
                  "   \"foo\" : \"foo-val-6\",\n" +
                  "   \"int\" : 6,\n" +
                  "   \"decimal\" : 0.6\n" +
                  "}," +
                  "{\n" +
                  "   \"foo\" : \"foo-val-7\",\n" +
                  "   \"int\" : 7,\n" +
                  "   \"decimal\" : 0.7,\n" +
                  "   \"bool\" : true\n" +
                  "}" +
                  "]";
        return configuration.JsonProvider.Parse(tmp);
    }


    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void a_filter_predicate_can_be_evaluated_on_string_criteria(IProviderTypeTestCase testCase)
    {
        var result = JsonPath.Read(Array(testCase.Configuration), "$[?(@.foo == 'foo-val-1')]").AsListOfMap();

        Assert.ContainsOnly(result, GetSingletonMap("foo", "foo-val-1"));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void a_filter_predicate_can_be_evaluated_on_int_criteria(IProviderTypeTestCase testCase)
    {
        var result = JsonPath.Read(GetArray2(testCase.Configuration), "$[?(@.int == 1)]").AsListOfMap();

        Xunit.Assert.Single(result);
        MyAssert.ContainsEntry(result[0], "int", 1d);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void a_filter_predicate_can_be_evaluated_on_decimal_criteria(IProviderTypeTestCase testCase)
    {
        var result = JsonPath.Read(GetArray2(testCase.Configuration), "$[?(@.decimal == 0.1)]").AsListOfMap();

        Xunit.Assert.Single(result);
        MyAssert.ContainsEntry(result[0], "decimal", 0.1d);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void multiple_criteria_can_be_used(IProviderTypeTestCase testCase)
    {
        var result = JsonPath.Read(GetArray2(testCase.Configuration), "$[?(@.decimal == 0.1 && @.int == 1)]")
            .AsListOfMap();

        Xunit.Assert.Single(result);
        MyAssert.ContainsEntry(result[0], "foo", "foo-val-1");
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void field_existence_can_be_checked(IProviderTypeTestCase testCase)
    {
        var result = JsonPath.Read(GetArray2(testCase.Configuration), "$[?(@.bool)]").AsListOfMap();

        Xunit.Assert.Single(result);
        MyAssert.ContainsEntry(result[0], "foo", "foo-val-7");
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void boolean_criteria_evaluates(IProviderTypeTestCase testCase)
    {
        var result = JsonPath.Read(GetArray2(testCase.Configuration), "$[?(@.bool == true)]").AsListOfMap();

        Xunit.Assert.Single(result);
        MyAssert.ContainsEntry(result[0], "foo", "foo-val-7");
    }
}