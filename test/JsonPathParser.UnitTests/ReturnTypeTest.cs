using XavierJefferson.JsonPathParser.Interfaces;
using XavierJefferson.JsonPathParser.UnitTests.Extensions;
using XavierJefferson.JsonPathParser.UnitTests.TestData;

namespace XavierJefferson.JsonPathParser.UnitTests;

public class ReturnTypeTest : TestUtils
{
    private static readonly IReadContext Reader = JsonPath.Parse(JsonTestData.JsonDocument);

    [Fact]
    public void assert_strings_can_be_read()
    {
        Assert.Equal("string-value", (string)Reader.Read("$.string-property"));
    }

    [Fact]
    public void assert_ints_can_be_read()
    {
        Assert.Equal(int.MaxValue, Reader.Read<double>("$.int-max-property"));
    }

    [Fact]
    public void assert_longs_can_be_read()
    {
        Assert.Equal(long.MaxValue, Reader.Read<double>("$.long-max-property"));
    }

    [Fact]
    public void assert_boolean_values_can_be_read()
    {
        Assert.True(Reader.Read<bool>("$.bool-property"));
    }

    [Fact]
    public void assert_null_values_can_be_read()
    {
        Assert.Null((string)Reader.Read("$.null-property"));
    }

    [Fact]
    public void assert_arrays_can_be_read()
    {
        /*
        Object result = reader.read("$.store.book");

        Assert.True(reader.configuration().JsonProvider.isArray(result));

        Xunit.Assert.Equal(4, reader.configuration().JsonProvider.length(result));
        */
        Assert.Equal(4, Reader.Read("$.store.book", Constants.ListType).AsList().Count());
    }

    [Fact]
    public void assert_maps_can_be_read()
    {
        var n = Reader.Read<JpDictionary>("$.store.book[0]");


        MyAssert.ContainsEntry(n, "category", "reference");
        MyAssert.ContainsEntry(n, "author", "Nigel Rees");
        MyAssert.ContainsEntry(n, "title", "Sayings of the Century");
        MyAssert.ContainsEntry(n, "display-price", 8.95D);
    }

    [Fact]
    public void a_path_evaluation_can_be_returned_as_PATH_LIST()
    {
        var conf = Configuration.CreateBuilder().WithOptions(Option.AsPathList).Build();

        var pathList = JsonPath.Using(conf).Parse(JsonTestData.JsonDocument).Read("$..author").AsList();

        MyAssert.ContainsExactly(pathList, "$['store']['book'][0]['author']", "$['store']['book'][1]['author']",
            "$['store']['book'][2]['author']", "$['store']['book'][3]['author']");
    }

    [Fact]
    public void class_cast_exception_is_thrown_when_return_type_is_not_expected()
    {
        Assert.Null(Reader.Read("$.store.book[0].author").AsList());
    }
}