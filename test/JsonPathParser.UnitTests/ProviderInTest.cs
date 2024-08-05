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


    [Fact]
    public void TestJsonPathQuotesGson()
    {
        var gson = ConfigurationData.SystemTextJsonConfiguration;
        var ctx = JsonPath.Using(gson).Parse((object)Json);

        var doubleQuoteEqualsResult = ctx.Read(DoubleQuotesEqualsFilter) as JpObjectList;
        Assert.Equal("bar", doubleQuoteEqualsResult[0].ToString());

        var singleQuoteEqualsResult = ctx.Read(SingleQuotesEqualsFilter) as JpObjectList;
        Assert.Equal(doubleQuoteEqualsResult, singleQuoteEqualsResult);

        var doubleQuoteInResult = ctx.Read(DoubleQuotesInFilter) as JpObjectList;
        Assert.Equal(doubleQuoteInResult, doubleQuoteEqualsResult);
        var singleQuoteInResult = ctx.Read(SingleQuotesInFilter) as JpObjectList;
        Assert.Equal(doubleQuoteInResult, singleQuoteInResult);
    }

    [Fact]
    public void TestJsonPathQuotesJsonOrg()
    {
        var configuration = ConfigurationData.NewtonsoftJsonConfiguration;
        var ctx = JsonPath.Using(configuration).Parse((object)Json);

        var doubleQuoteEqualsResult = ctx.Read(DoubleQuotesEqualsFilter) as JpObjectList;
        Assert.Equal("bar", doubleQuoteEqualsResult[0]);

        var singleQuoteEqualsResult = ctx.Read(SingleQuotesEqualsFilter) as JpObjectList;
        Assert.Equal(doubleQuoteEqualsResult[0], singleQuoteEqualsResult[0]);

        var doubleQuoteInResult = ctx.Read(DoubleQuotesInFilter) as JpObjectList;
        Assert.Equal(doubleQuoteInResult[0], doubleQuoteEqualsResult[0]);

        var singleQuoteInResult = ctx.Read(SingleQuotesInFilter) as JpObjectList;
        Assert.Equal(doubleQuoteInResult[0], singleQuoteInResult[0]);
    }
}