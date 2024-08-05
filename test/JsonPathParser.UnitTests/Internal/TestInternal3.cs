using XavierJefferson.JsonPathParser.Path;
using XavierJefferson.JsonPathParser.UnitTests.Extensions;
using XavierJefferson.JsonPathParser.UnitTests.TestData;

namespace XavierJefferson.JsonPathParser.UnitTests.Internal;

public class TestInternal3 : TestBase
{
    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void a_root_object_can_be_evaluated(IProviderTypeTestCase testCase)
    {
        var result =
            PathCompiler.Compile("$").Evaluate(Doc(testCase), Doc(testCase), Conf(testCase)).GetValue() as JpDictionary;

        MyAssert.ContainsKey(result, "store");
        Assert.Single(result);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void a_definite_array_item_property_can_be_evaluated(IProviderTypeTestCase testCase)
    {
        var result = PathCompiler.Compile("$.store.book[0].author")
            .Evaluate(Doc(testCase), Doc(testCase), Conf(testCase)).GetValue();

        Assert.Equal("Nigel Rees", result);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void a_wildcard_array_item_property_can_be_evaluated(IProviderTypeTestCase testCase)
    {
        var result = PathCompiler.Compile("$.store.book[*].author")
            .Evaluate(Doc(testCase), Doc(testCase), Conf(testCase)).GetValue().AsList();
        MyAssert.ContainsOnly(result, "Nigel Rees", "Evelyn Waugh", "Herman Melville", "J. R. R. Tolkien");
    }
}