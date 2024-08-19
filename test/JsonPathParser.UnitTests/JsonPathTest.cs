using XavierJefferson.JsonPathParser.Exceptions;
using XavierJefferson.JsonPathParser.Path;
using XavierJefferson.JsonPathParser.UnitTests.Extensions;
using XavierJefferson.JsonPathParser.UnitTests.TestData;

namespace XavierJefferson.JsonPathParser.UnitTests;

public class JsonPathTest : TestUtils
{
    public static readonly string Array = "[{\"value\": 1},{\"value\": 2}, {\"value\": 3},{\"value\": 4}]";

    public static readonly string Document =
        "{ \"store\": {\n" +
        "    \"book\": [ \n" +
        "      { \"category\": \"reference\",\n" +
        "        \"author\": \"Nigel Rees\",\n" +
        "        \"title\": \"Sayings of the Century\",\n" +
        "        \"display-price\": 8.95\n" +
        "      },\n" +
        "      { \"category\": \"fiction\",\n" +
        "        \"author\": \"Evelyn Waugh\",\n" +
        "        \"title\": \"Sword of Honour\",\n" +
        "        \"display-price\": 12.99\n" +
        "      },\n" +
        "      { \"category\": \"fiction\",\n" +
        "        \"author\": \"Herman Melville\",\n" +
        "        \"title\": \"Moby Dick\",\n" +
        "        \"isbn\": \"0-553-21311-3\",\n" +
        "        \"display-price\": 8.99\n" +
        "      },\n" +
        "      { \"category\": \"fiction\",\n" +
        "        \"author\": \"J. R. R. Tolkien\",\n" +
        "        \"title\": \"The Lord of the Rings\",\n" +
        "        \"isbn\": \"0-395-19395-8\",\n" +
        "        \"display-price\": 22.99\n" +
        "      }\n" +
        "    ],\n" +
        "    \"bicycle\": {\n" +
        "      \"color\": \"red\",\n" +
        "      \"display-price\": 19.95,\n" +
        "      \"foo:bar\": \"fooBar\",\n" +
        "      \"dot.notation\": \"new\",\n" +
        "      \"dash-notation\": \"dashes\"\n" +
        "    }\n" +
        "  }\n" +
        "}";

    public static readonly object ObjDocument = JsonPath.Parse(Document).Json;


    private static readonly string ProductJson = "{\n" +
                                                 "\t\"product\": [ {\n" +
                                                 "\t    \"version\": \"A\", \n" +
                                                 "\t    \"codename\": \"Seattle\", \n" +
                                                 "\t    \"attr.with.dot\": \"A\"\n" +
                                                 "\t},\n" +
                                                 "\t{\n" +
                                                 "\t    \"version\": \"4.0\", \n" +
                                                 "\t    \"codename\": \"Montreal\", \n" +
                                                 "\t    \"attr.with.dot\": \"B\"\n" +
                                                 "\t}]\n" +
                                                 "}";

    private static readonly string ArrayExpand =
        "[{\"parent\": \"ONE\", \"child\": {\"name\": \"NAME_ONE\"}}, [{\"parent\": \"TWO\", \"child\": {\"name\": \"NAME_TWO\"}}]]";

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void missing_prop(IProviderTypeTestCase testCase)
    {
        Assert.Throws<PathNotFoundException>(() =>
            JsonPath.Using(testCase.Configuration.AddOptions(Option.RequireProperties)).Parse(Document)
                .Read("$.store.book[*].fooBar.not"));
    }

    [Fact]
    public void bracket_notation_with_dots()
    {
        var json = "{\n" +
                   "    \"store\": {\n" +
                   "        \"book\": [\n" +
                   "            {\n" +
                   "                \"author.name\": \"Nigel Rees\", \n" +
                   "                \"category\": \"reference\", \n" +
                   "                \"price\": 8.95, \n" +
                   "                \"title\": \"Sayings of the Century\"\n" +
                   "            }\n" +
                   "        ]\n" +
                   "    }\n" +
                   "}";

        Assert.Equal("Nigel Rees", JsonPath.Read(json, "$.store.book[0]['author.name']"));
    }

    [Fact]
    public void null_object_in_path()
    {
        var json = "{\n" +
                   "  \"success\": true,\n" +
                   "  \"data\": {\n" +
                   "    \"user\": 3,\n" +
                   "    \"own\": null,\n" +
                   "    \"passes\": null,\n" +
                   "    \"completed\": null\n" +
                   "  },\n" +
                   "  \"data2\": {\n" +
                   "    \"user\": 3,\n" +
                   "    \"own\": null,\n" +
                   "    \"passes\": [{\"id\":\"1\"}],\n" +
                   "    \"completed\": null\n" +
                   "  },\n" +
                   "  \"version\": 1371160528774\n" +
                   "}";
        Assert.Throws<PathNotFoundException>(() => { JsonPath.Read(json, "$.data.passes[0].id"); });
        Assert.Equal("1", JsonPath.Read(json, "$.data2.passes[0].id"));
    }

    [Fact]
    public void array_start_expands()
    {
        MyAssert.ContainsAll(JsonPath.Read<List<object?>>(ArrayExpand, "$[?(@['parent'] == 'ONE')].child.name"),
            "NAME_ONE");
    }

    [Fact]
    public void bracket_notation_can_be_used_in_path()
    {
        Assert.Equal("new", JsonPath.Read(Document, "$.['store'].bicycle.['dot.notation']"));
        Assert.Equal("new", JsonPath.Read(Document, "$['store']['bicycle']['dot.notation']"));
        Assert.Equal("new", JsonPath.Read(Document, "$.['store']['bicycle']['dot.notation']"));
        Assert.Equal("new", JsonPath.Read(Document, "$.['store'].['bicycle'].['dot.notation']"));


        Assert.Equal("dashes", JsonPath.Read(Document, "$.['store'].bicycle.['dash-notation']"));
        Assert.Equal("dashes", JsonPath.Read(Document, "$['store']['bicycle']['dash-notation']"));
        Assert.Equal("dashes", JsonPath.Read(Document, "$.['store']['bicycle']['dash-notation']"));
        Assert.Equal("dashes", JsonPath.Read(Document, "$.['store'].['bicycle'].['dash-notation']"));
    }

    [Fact]
    public void filter_an_array()
    {
        var matches = JsonPath.Read(Array, "$.[?(@.value == 1)]").AsList();

        Assert.Single(matches);
    }

    [Fact]
    public void filter_an_array_on_index()
    {
        var matches = JsonPath.Read(Array, "$.[1].value");

        Assert.Equal(2d, matches);
    }

    [Fact]
    public void read_path_with_colon()
    {
        Assert.Equal(JsonPath.Read(Document, "$['store']['bicycle']['foo:bar']"), "fooBar");
    }

    [Fact]
    public void read_document_from_root()
    {
        var result = JsonPath.Read(Document, "$.store") as Dictionary<string, object?>;

        Assert.Equal(2, result.Values.Count());
    }

    [Fact]
    public void read_store_book_1()
    {
        var path = JsonPath.Compile("$.store.book[1]");

        var map = path.Read(Document) as IDictionary<string, object?>;

        Assert.Equal("Evelyn Waugh", map["author"]);
    }

    [Fact]
    public void read_store_book_wildcard()
    {
        var path = JsonPath.Compile("$.store.book[*]");

        var list = path.Read(Document).AsList();
        Assert.Equal(4, list.Count());
    }

    [Fact]
    public void read_store_book_author()
    {
        MyAssert.ContainsAll(JsonPath.Read<List<object?>>(Document, "$.store.book[0,1].author"), "Nigel Rees",
            "Evelyn Waugh");
        MyAssert.ContainsAll(JsonPath.Read<List<object?>>(Document, "$.store.book[*].author"), "Nigel Rees",
            "Evelyn Waugh", "Herman Melville", "J. R. R. Tolkien");
        MyAssert.ContainsAll(JsonPath.Read<List<object?>>(Document, "$.['store'].['book'][*].['author']"), "Nigel Rees",
            "Evelyn Waugh", "Herman Melville", "J. R. R. Tolkien");
        MyAssert.ContainsAll(JsonPath.Read<List<object?>>(Document, "$['store']['book'][*]['author']"), "Nigel Rees",
            "Evelyn Waugh", "Herman Melville", "J. R. R. Tolkien");
        MyAssert.ContainsAll(JsonPath.Read<List<object?>>(Document, "$['store'].book[*]['author']"), "Nigel Rees",
            "Evelyn Waugh", "Herman Melville", "J. R. R. Tolkien");
    }

    [Fact]
    public void all_authors()
    {
        MyAssert.ContainsAll(JsonPath.Read<List<object?>>(Document, "$..author"), "Nigel Rees", "Evelyn Waugh",
            "Herman Melville", "J. R. R. Tolkien");
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void all_store_properties(IProviderTypeTestCase testCase)
    {
        /*
        var  itemsInStore = JsonPath.Read(DOCUMENT, "$.store.*").asList();

        Xunit.Assert.Equal(JsonPath.Read(itemsInStore, "$.[0].[0].author"), "Nigel Rees");
        Xunit.Assert.Equal(JsonPath.Read(itemsInStore, "$.[0][0].author"), "Nigel Rees");
        */
        var result = PathCompiler.Compile("$.store.*")
            .Evaluate(ObjDocument, ObjDocument, testCase.Configuration).GetPathList();

        Assert.True(result.ContainsOnly(
            "$['store']['bicycle']",
            "$['store']['book']"));
    }

    [Fact]
    public void all_prices_in_store()
    {
        MyAssert.ContainsAll(JsonPath.Read<List<object?>>(Document, "$.store..['display-price']"), 8.95D, 12.99D, 8.99D,
            19.95D);
    }

    [Fact]
    public void access_array_by_index_from_tail()
    {
        MyAssert.ContainsAll(JsonPath.Read<List<object?>>(Document, "$..book[1:].author"), "Evelyn Waugh",
            "Herman Melville", "J. R. R. Tolkien");
    }

    [Fact]
    public void read_store_book_index_0_and_1()
    {
        MyAssert.ContainsAll(JsonPath.Read<List<object?>>(Document, "$.store.book[0,1].author"), "Nigel Rees",
            "Evelyn Waugh");
        Assert.Equal(2, JsonPath.Read<List<object?>>(Document, "$.store.book[0,1].author").Count());
    }

    [Fact]
    public void read_store_book_pull_first_2()
    {
        MyAssert.ContainsAll(JsonPath.Read<List<object?>>(Document, "$.store.book[:2].author"), "Nigel Rees",
            "Evelyn Waugh");
        Assert.Equal(2, JsonPath.Read<List<object?>>(Document, "$.store.book[:2].author").Count());
    }


    [Fact]
    public void read_store_book_filter_by_isbn()
    {
        MyAssert.ContainsAll(JsonPath.Read<List<object?>>(Document, "$.store.book[?(@.isbn)].isbn"), "0-553-21311-3",
            "0-395-19395-8");
        Assert.Equal(2, JsonPath.Read<List<object?>>(Document, "$.store.book[?(@.isbn)].isbn").Count());
        Assert.Equal(2, JsonPath.Read<List<object?>>(Document, "$.store.book[?(@['isbn'])].isbn").Count());
    }

    [Fact]
    public void all_books_cheaper_than_10()
    {
        MyAssert.ContainsAll(JsonPath.Read<List<object?>>(Document, "$..book[?(@['display-price'] < 10)].title"),
            "Sayings of the Century", "Moby Dick");
        MyAssert.ContainsAll(JsonPath.Read<List<object?>>(Document, "$..book[?(@.display-price < 10)].title"),
            "Sayings of the Century", "Moby Dick");
    }

    [Fact]
    public void all_books()
    {
        Assert.Equal(1, JsonPath.Read<List<object?>>(Document, "$..book").Count());
    }

    [Fact]
    public void dot_in_predicate_works()
    {
        MyAssert.ContainsAll(JsonPath.Read<List<object?>>(ProductJson, "$.product[?(@.version=='4.0')].codename"),
            "Montreal");
    }

    [Fact]
    public void dots_in_predicate_works()
    {
        MyAssert.ContainsAll(
            JsonPath.Read<List<object?>>(ProductJson, "$.product[?(@.['attr.with.dot']=='A')].codename"), "Seattle");
    }

    [Fact]
    public void all_books_with_category_reference()
    {
        MyAssert.ContainsAll(JsonPath.Read<List<object?>>(Document, "$..book[?(@.category=='reference')].title"),
            "Sayings of the Century");
        MyAssert.ContainsAll(JsonPath.Read<List<object?>>(Document, "$.store.book[?(@.category=='reference')].title"),
            "Sayings of the Century");
    }

    [Fact]
    public void all_members_of_all_documents()
    {
        var all = JsonPath.Read(Document, "$..*").AsList();
    }

    [Fact]
    public void access_index_out_of_bounds_does_not_throw_exception()
    {
        Assert.Throws<PathNotFoundException>(() => JsonPath.Read(Document, "$.store.book[100].author"));
    }

    [Fact]
    public void exists_filter_with_nested_path()
    {
        Assert.Single(JsonPath.Read<List<object?>>(Document, "$..[?(@.bicycle.color)]"));
        Assert.Empty(JsonPath.Read<List<object?>>(Document, "$..[?(@.bicycle.numberOfGears)]"));
    }

    [Fact]
    // see https://code.google.com/p/json-path/issues/detail?id=58
    public void invalid_paths_throw_invalid_path_exception()
    {
        foreach (var path in new[] { "$.", "$.results[?" })
            Assert.Throws<InvalidPathException>(() => { JsonPath.Compile(path); });
    }

    [Fact]
    //see https://github.com/json-path/JsonPath/issues/428
    public void prevent_stack_overflow_error_when_unclosed_property()
    {
        Assert.Throws<InvalidPathException>(() => JsonPath.Compile("$['boo','foo][?(@ =~ /bar/)]"));
    }
}