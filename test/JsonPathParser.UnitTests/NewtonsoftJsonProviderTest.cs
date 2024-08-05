using XavierJefferson.JsonPathParser.Mapper;
using XavierJefferson.JsonPathParser.Provider;
using XavierJefferson.JsonPathParser.UnitTests.Extensions;
using XavierJefferson.JsonPathParser.UnitTests.TestData;

namespace XavierJefferson.JsonPathParser.UnitTests;

public class NewtonsoftJsonProviderTest : TestUtils
{
    [Fact]
    public void an_object_can_be_read()
    {
        var book = JsonPath.Using(ConfigurationData.NewtonsoftJsonConfiguration).Parse(JsonTestData.JsonDocument)
            .Read<JpDictionary>("$.store.book[0]");

        Assert.Equal("Nigel Rees", book["author"].ToString());
    }

    [Fact]
    public void a_property_can_be_read()
    {
        var category = JsonPath.Using(ConfigurationData.NewtonsoftJsonConfiguration).Parse(JsonTestData.JsonDocument)
            .Read<string>("$.store.book[0].category");

        Assert.Equal("reference", category);
    }

    [Fact]
    public void a_filter_can_be_applied()
    {
        var fictionBooks = JsonPath.Using(ConfigurationData.NewtonsoftJsonConfiguration)
            .Parse(JsonTestData.JsonDocument).Read<JpObjectList>("$.store.book[?(@.category == 'fiction')]");

        Assert.Equal(3, fictionBooks.Count());
    }

    [Fact]
    public void result_can_be_mapped_to_object()
    {
        var books = JsonPath.Using(ConfigurationData.NewtonsoftJsonConfiguration).Parse(JsonTestData.JsonDocument)
            .Read<JpObjectList>("$.store.book");

        Assert.Equal(4, books.Count());
    }

    [Fact]
    public void read_books_with_isbn()
    {
        var books = JsonPath.Using(ConfigurationData.NewtonsoftJsonConfiguration).Parse(JsonTestData.JsonDocument)
            .Read<JpObjectList>("$..book[?(@.isbn)]");

        Assert.Equal(2, books.Count());
    }

    /**
     * Functions take parameters, the length parameter for example takes an entire document which we anticipate
     * will compute to a document that is an array of elements which can determine its length.
     * 
     * Since we translate this query from $..books.length() to length($..books) verify that this particular translation
     * works as anticipated.
     */
    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void read_book_length_using_translated_query(IProviderTypeTestCase testCase)
    {
        var result = JsonPath.Using(testCase.Configuration)
            .Parse(JsonTestData.JsonBookStoreDocument)
            .Read<int>("$..book.length()");
        Assert.Equal(4, result);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void read_book_length(IProviderTypeTestCase testCase)
    {
        var result = JsonPath.Using(testCase.Configuration)
            .Parse(JsonTestData.JsonBookStoreDocument)
            .Read<int>("$.length($..book)");
        Assert.Equal(4, result);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void test_getPropertyKeys_empty_object(IProviderTypeTestCase testCase)
    {
        var json = "{\"foo\": \"bar\", \"emptyObject\": {},\"emptyList\":[]}";
        var config = testCase.Configuration
            .CreateWithJsonProvider(new NewtonsoftJsonProvider())
            .CreateWithMappingProvider(new NewtonsoftJsonMappingProvider());
        var result = JsonPath.Using(config).Parse(json).Read("$..foo").AsList();
        MyAssert.ContainsExactly(result, "bar");
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void test_getPropertyKeys_empty_nest_object(IProviderTypeTestCase testCase)
    {
        var json = "{\"foo\": \"bar\", \"emptyObject\": {\"emptyList\":[]},\"emptyList\":[]}";
        var config = testCase.Configuration
            .CreateWithJsonProvider(new NewtonsoftJsonProvider())
            .CreateWithMappingProvider(new NewtonsoftJsonMappingProvider());
        var result = JsonPath.Using(config).Parse(json).Read("$..foo").AsList();
        MyAssert.ContainsExactly(result, "bar");
    }
}