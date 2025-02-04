using XavierJefferson.JsonPathParser.Enums;
using XavierJefferson.JsonPathParser.Interfaces;
using XavierJefferson.JsonPathParser.UnitTests.Extensions;
using XavierJefferson.JsonPathParser.UnitTests.TestData;

namespace XavierJefferson.JsonPathParser.UnitTests;

public class ReturnTypeTest : TestUtils
{
    private static readonly IReadContext Reader = JsonPath.Parse(JsonTestData.JsonDocument);

    [Fact]
    public void AssertStringsCanBeRead()
    {
        Assert.Equal("string-value", (string)Reader.Read("$.string-property"));
    }

    [Fact]
    public void AssertIntsCanBeRead()
    {
        Assert.Equal(int.MaxValue, Reader.Read<double>("$.int-max-property"));
    }

    [Fact]
    public void AssertLongsCanBeRead()
    {
        Assert.Equal(long.MaxValue, Reader.Read<double>("$.long-max-property"));
    }

    [Fact]
    public void AssertBooleanValuesCanBeRead()
    {
        Assert.True(Reader.Read<bool>("$.bool-property"));
    }

    [Fact]
    public void AssertNullValuesCanBeRead()
    {
        Assert.Null((string)Reader.Read("$.null-property"));
    }

    [Fact]
    public void AssertArraysCanBeRead()
    {
        /*
        Object result = reader.read("$.store.book");

        Assert.True(reader.configuration().JsonProvider.isArray(result));

        Xunit.Assert.Equal(4, reader.configuration().JsonProvider.length(result));
        */
        Assert.Equal(4, Reader.Read("$.store.book", TypeConstants.ListType).AsList().Count());
    }

    [Fact]
    public void AssertMapsCanBeRead()
    {
        var n = Reader.Read<IDictionary<string, object?>>("$.store.book[0]");


        MyAssert.ContainsEntry(n, "category", "reference");
        MyAssert.ContainsEntry(n, "author", "Nigel Rees");
        MyAssert.ContainsEntry(n, "title", "Sayings of the Century");
        MyAssert.ContainsEntry(n, "display-price", 8.95D);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void APathEvaluationCanBeReturnedAsPathList(IProviderTypeTestCase testCase)
    {
        var conf = testCase.Configuration.SetOptions(ConfigurationOptionEnum.AsPathList);

        var pathList = JsonPath.Using(conf).Parse(JsonTestData.JsonDocument).Read("$..author").AsList();

        MyAssert.ContainsExactly(pathList, "$['store']['book'][0]['author']", "$['store']['book'][1]['author']",
            "$['store']['book'][2]['author']", "$['store']['book'][3]['author']");
    }

    [Fact]
    public void ClassCastExceptionIsThrownWhenReturnTypeIsNotExpected()
    {
        Assert.Null(Reader.Read("$.store.book[0].author").AsList());
    }
}