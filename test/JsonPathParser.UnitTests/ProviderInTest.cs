using XavierJefferson.JsonPathParser.UnitTests.TestData;

namespace XavierJefferson.JsonPathParser.UnitTests;

public class ProviderInTest
{
    private const string Json = "[{\"foo\": \"bar\"}, {\"foo\": \"baz\"}]";
    private const string EqualsFilter = "$.[?(@.foo == {0})].foo";
    private const string InFilter = "$.[?(@.foo in [{0}])].foo";
    private const string DoubleQuotes = "\"bar\"";
    private const string SingleQuotes = "'bar'";
    private static readonly string DoubleQuotesEqualsFilter = string.Format(EqualsFilter, DoubleQuotes);
    private static readonly string DoubleQuotesInFilter = string.Format(InFilter, DoubleQuotes);
    private static readonly string SingleQuotesEqualsFilter = string.Format(EqualsFilter, SingleQuotes);
    private static readonly string SingleQuotesInFilter = string.Format(InFilter, SingleQuotes);


    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void TestJsonPathQuotes(IProviderTypeTestCase testCase)
    {
        var configuration = testCase.Configuration;
        var context = JsonPath.Using(configuration).Parse((object)Json);

        var doubleQuoteEqualsResult = context.Read(DoubleQuotesEqualsFilter) as List<object?>;
        Assert.Equal("bar", doubleQuoteEqualsResult[0]);

        var singleQuoteEqualsResult = context.Read(SingleQuotesEqualsFilter) as List<object?>;
        Assert.Equal(doubleQuoteEqualsResult[0], singleQuoteEqualsResult[0]);

        var doubleQuoteInResult = context.Read(DoubleQuotesInFilter) as List<object?>;
        Assert.Equal(doubleQuoteInResult[0], doubleQuoteEqualsResult[0]);

        var singleQuoteInResult = context.Read(SingleQuotesInFilter) as List<object?>;
        Assert.Equal(doubleQuoteInResult[0], singleQuoteInResult[0]);
    }
}