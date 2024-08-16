using XavierJefferson.JsonPathParser.Exceptions;
using XavierJefferson.JsonPathParser.UnitTests.Extensions;
using XavierJefferson.JsonPathParser.UnitTests.TestData;

namespace XavierJefferson.JsonPathParser.UnitTests;

public class NullHandlingTest
{
    public static string Document = "{\n" +
                                    "   \"root-property\": \"root-property-value\",\n" +
                                    "   \"root-property-null\": null,\n" +
                                    "   \"children\": [\n" +
                                    "      {\n" +
                                    "         \"id\": 0,\n" +
                                    "         \"name\": \"name-0\",\n" +
                                    "         \"age\": 0\n" +
                                    "      },\n" +
                                    "      {\n" +
                                    "         \"id\": 1,\n" +
                                    "         \"name\": \"name-1\",\n" +
                                    "         \"age\": null" +
                                    "      },\n" +
                                    "      {\n" +
                                    "         \"id\": 3,\n" +
                                    "         \"name\": \"name-3\"\n" +
                                    "      }\n" +
                                    "   ]\n" +
                                    "}";


    [Fact]
    public void not_defined_property_throws_PathNotFoundException()
    {
        Assert.Throws<PathNotFoundException>(() => JsonPath.Read(Document, "$.children[0].child.age"));
    }


    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void last_token_defaults_to_null(IProviderTypeTestCase testCase)
    {
        var configuration = testCase.Configuration.SetOptions(Option.DefaultPathLeafToNull);

        Assert.Null(JsonPath.Parse(Document, configuration).Read("$.children[2].age"));
    }


    [Fact]
    public void null_property_returns_null()
    {
        var age = JsonPath.Read<double?>(Document, "$.children[1].age");
        Assert.Null(age);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void the_age_of_all_with_age_defined(IProviderTypeTestCase testCase)
    {
        var result = JsonPath.Using(testCase.Configuration.SetOptions(Option.SuppressExceptions))
            .Parse(Document).Read("$.children[*].age").AsList();

        MyAssert.ContainsExactly(result, 0d, null);

    }

    [Fact]
    public void Path2()
    {
        var result = JsonPath.Read("{\"a\":[{\"b\":1,\"c\":2},{\"b\":5,\"c\":2}]}", "a[?(@.b==4)].c").AsList();
        Assert.Empty(result);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void Path(IProviderTypeTestCase testCase)
    {
        var json = "{\"a\":[{\"b\":1,\"c\":2},{\"b\":5,\"c\":2}]}";

        var result = JsonPath.Using(testCase.Configuration.SetOptions(Option.DefaultPathLeafToNull))
            .Parse(json).Read("a[?(@.b==5)].d").AsList();

        Assert.Single(result);
        Assert.Null(result[0]);
    }
}