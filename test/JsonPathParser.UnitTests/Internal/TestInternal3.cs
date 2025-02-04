using XavierJefferson.JsonPathParser.Path;
using XavierJefferson.JsonPathParser.UnitTests.Extensions;
using XavierJefferson.JsonPathParser.UnitTests.TestData;

namespace XavierJefferson.JsonPathParser.UnitTests.Internal;

public class TestInternal3 : TestBase
{
    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void ARootObjectCanBeEvaluated(IProviderTypeTestCase testCase)
    {
        var result =
            PathCompiler.Compile("$").Evaluate(Doc(testCase), Doc(testCase), Conf(testCase)).GetValue() as
                IDictionary<string, object?>;

        MyAssert.ContainsKey(result, "store");
        Assert.Single(result);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void ADefiniteArrayItemPropertyCanBeEvaluated(IProviderTypeTestCase testCase)
    {
        var result = PathCompiler.Compile("$.store.book[0].author")
            .Evaluate(Doc(testCase), Doc(testCase), Conf(testCase)).GetValue();

        Assert.Equal("Nigel Rees", result);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void AWildcardArrayItemPropertyCanBeEvaluated(IProviderTypeTestCase testCase)
    {
        var result = PathCompiler.Compile("$.store.book[*].author")
            .Evaluate(Doc(testCase), Doc(testCase), Conf(testCase)).GetValue().AsList();
        MyAssert.ContainsOnly(result, "Nigel Rees", "Evelyn Waugh", "Herman Melville", "J. R. R. Tolkien");
    }
}