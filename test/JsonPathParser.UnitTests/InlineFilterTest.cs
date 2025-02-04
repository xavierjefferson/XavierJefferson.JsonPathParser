using XavierJefferson.JsonPathParser.UnitTests.Extensions;
using XavierJefferson.JsonPathParser.UnitTests.TestData;

namespace XavierJefferson.JsonPathParser.UnitTests;

public class InlineFilterTest : TestUtils
{
    private static readonly int BookCount = 4;

    public static string MultiStoreJsonDocument = "{\n" +
                                                  "   \"store\" : [{\n" +
                                                  "      \"name\": \"First\"," +
                                                  "      \"book\" : [\n" +
                                                  "         {\n" +
                                                  "            \"category\" : \"reference\",\n" +
                                                  "            \"author\" : \"Nigel Rees\",\n" +
                                                  "            \"title\" : \"Sayings of the Century\",\n" +
                                                  "            \"display-price\" : 8.95\n" +
                                                  "         },\n" +
                                                  "         {\n" +
                                                  "            \"category\" : \"fiction\",\n" +
                                                  "            \"author\" : \"Evelyn Waugh\",\n" +
                                                  "            \"title\" : \"Sword of Honour\",\n" +
                                                  "            \"display-price\" : 12.99\n" +
                                                  "         },\n" +
                                                  "         {\n" +
                                                  "            \"category\" : \"fiction\",\n" +
                                                  "            \"author\" : \"Herman Melville\",\n" +
                                                  "            \"title\" : \"Moby Dick\",\n" +
                                                  "            \"isbn\" : \"0-553-21311-3\",\n" +
                                                  "            \"display-price\" : 8.99\n" +
                                                  "         },\n" +
                                                  "         {\n" +
                                                  "            \"category\" : \"fiction\",\n" +
                                                  "            \"author\" : \"J. R. R. Tolkien\",\n" +
                                                  "            \"title\" : \"The Lord of the Rings\",\n" +
                                                  "            \"isbn\" : \"0-395-19395-8\",\n" +
                                                  "            \"display-price\" : 22.99\n" +
                                                  "         }]\n" +
                                                  "      },\n" +
                                                  "      {\n" +
                                                  "       \"name\": \"Second\",\n" +
                                                  "       \"book\": [\n" +
                                                  "         {\n" +
                                                  "            \"category\" : \"fiction\",\n" +
                                                  "            \"author\" : \"Ernest Hemmingway\",\n" +
                                                  "            \"title\" : \"The Old Man and the Sea\",\n" +
                                                  "            \"display-price\" : 12.99\n" +
                                                  "         }]\n" +
                                                  "      }]}";


    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void RootContextCanBeReferredInPredicate(IProviderTypeTestCase testCase)
    {
        var prices = JsonPath.Using(testCase.Configuration).Parse(JsonTestData.JsonDocument)
            .Read("store.book[?(@.display-price <= $.max-price)].display-price", TypeConstants.ListType).AsList();

        MyAssert.ContainsAll(prices, 8.95D, 8.99D);
    }

    private double AsDouble(object object1)
    {
        return (double)object1;
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void MultipleContextObjectCanBeRefered(IProviderTypeTestCase testCase)
    {
        var all = JsonPath.Using(testCase.Configuration).Parse(JsonTestData.JsonDocument)
            .Read("store.book[ ?(@.category == @.category) ]", TypeConstants.ListType).AsList();
        Assert.Equal(BookCount, all.Count());

        var all2 = JsonPath.Using(testCase.Configuration).Parse(JsonTestData.JsonDocument)
            .Read("store.book[ ?(@.category == @['category']) ]", TypeConstants.ListType).AsList();
        Assert.Equal(BookCount, all2.Count());

        var all3 = JsonPath.Using(testCase.Configuration).Parse(JsonTestData.JsonDocument)
            .Read("store.book[ ?(@ == @) ]", TypeConstants.ListType).AsList();
        Assert.Equal(BookCount, all3.Count());

        var none = JsonPath.Using(testCase.Configuration).Parse(JsonTestData.JsonDocument)
            .Read("store.book[ ?(@.category != @.category) ]", TypeConstants.ListType).AsList();
        Assert.Equal(0, none.Count());

        var none2 = JsonPath.Using(testCase.Configuration).Parse(JsonTestData.JsonDocument)
            .Read("store.book[ ?(@.category != @) ]", TypeConstants.ListType).AsList();
        Assert.Equal(4, none2.Count());
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void SimpleInlineOrStatementEvaluates(IProviderTypeTestCase testCase)
    {
        var a = JsonPath.Using(testCase.Configuration).Parse(JsonTestData.JsonDocument)
            .Read("store.book[ ?(@.author == 'Nigel Rees' || @.author == 'Evelyn Waugh') ].author",
                TypeConstants.ListType)
            .AsList();
        MyAssert.ContainsExactly(a, "Nigel Rees", "Evelyn Waugh");

        var b = JsonPath.Using(testCase.Configuration).Parse(JsonTestData.JsonDocument)
            .Read(
                "store.book[ ?((@.author == 'Nigel Rees' || @.author == 'Evelyn Waugh') && @.display-price < 15) ].author",
                TypeConstants.ListType).AsList();
        MyAssert.ContainsExactly(b, "Nigel Rees", "Evelyn Waugh");

        var c = JsonPath.Using(testCase.Configuration).Parse(JsonTestData.JsonDocument)
            .Read(
                "store.book[ ?((@.author == 'Nigel Rees' || @.author == 'Evelyn Waugh') && @.category == 'reference') ].author",
                TypeConstants.ListType).AsList();
        MyAssert.ContainsExactly(c, "Nigel Rees");

        var d = JsonPath.Using(testCase.Configuration).Parse(JsonTestData.JsonDocument)
            .Read(
                "store.book[ ?((@.author == 'Nigel Rees') || (@.author == 'Evelyn Waugh' && @.category != 'fiction')) ].author",
                TypeConstants.ListType).AsList();
        MyAssert.ContainsExactly(d, "Nigel Rees");
    }


    public void NoPathRefInFilterHitAll()
    {
        var res = JsonPath.Parse(JsonTestData.JsonDocument).Read("$.store.book[?('a' == 'a')].author").AsList();

        MyAssert.ContainsExactly(res, "Nigel Rees", "Evelyn Waugh", "Herman Melville", "J. R. R. Tolkien");
    }

    [Fact]
    public void NoPathRefInFilterHitNone()
    {
        var res = JsonPath.Parse(JsonTestData.JsonDocument).Read("$.store.book[?('a' == 'b')].author").AsList();

        Assert.Empty(res);
    }

    [Fact]
    public void PathCanBeOnEitherSideOfOperator()
    {
        var resLeft = JsonPath.Parse(JsonTestData.JsonDocument)
            .Read("$.store.book[?(@.category == 'reference')].author").AsList();
        var resRight = JsonPath.Parse(JsonTestData.JsonDocument)
            .Read("$.store.book[?('reference' == @.category)].author").AsList();

        MyAssert.ContainsExactly(resLeft, "Nigel Rees");
        MyAssert.ContainsExactly(resRight, "Nigel Rees");
    }

    [Fact]
    public void PathCanBeOnBothSideOfOperator()
    {
        var res = JsonPath.Parse(JsonTestData.JsonDocument).Read("$.store.book[?(@.category == @.category)].author")
            .AsList();

        MyAssert.ContainsExactly(res, "Nigel Rees", "Evelyn Waugh", "Herman Melville", "J. R. R. Tolkien");
    }

    [Fact]
    public void PatternsCanBeEvaluated()
    {
        var resLeft = JsonPath.Parse(JsonTestData.JsonDocument)
            .Read("$.store.book[?(@.category =~ /reference/)].author").AsList();
        MyAssert.ContainsExactly(resLeft, "Nigel Rees");

        resLeft = JsonPath.Parse(JsonTestData.JsonDocument).Read("$.store.book[?(/reference/ =~ @.category)].author")
            .AsList();
        MyAssert.ContainsExactly(resLeft, "Nigel Rees");
    }

    [Fact]
    public void PatternsCanBeEvaluatedWithIgnoreCase()
    {
        var resLeft = JsonPath.Parse(JsonTestData.JsonDocument)
            .Read("$.store.book[?(@.category =~ /REFERENCE/)].author").AsList();
        Assert.Empty(resLeft);

        resLeft = JsonPath.Parse(JsonTestData.JsonDocument).Read("$.store.book[?(@.category =~ /REFERENCE/i)].author")
            .AsList();
        MyAssert.ContainsExactly(resLeft, "Nigel Rees");
    }

    [Fact]
    public void PatternsMatchAgainstLists()
    {
        var haveRefBooks = JsonPath.Parse(MultiStoreJsonDocument)
            .Read("$.store[?(@.book[*].category =~ /Reference/i)].name").AsList();
        MyAssert.ContainsExactly(haveRefBooks, "First");
    }

    [Fact]
    public void NegateExistsCheck()
    {
        var hasIsbn = JsonPath.Parse(JsonTestData.JsonDocument).Read("$.store.book[?(@.isbn)].author").AsList();
        MyAssert.ContainsExactly(hasIsbn, "Herman Melville", "J. R. R. Tolkien");

        var noIsbn = JsonPath.Parse(JsonTestData.JsonDocument).Read("$.store.book[?(!@.isbn)].author").AsList();

        MyAssert.ContainsExactly(noIsbn, "Nigel Rees", "Evelyn Waugh");
    }

    [Fact]
    public void NegateExistsCheckPrimitive()
    {
        var ints = new List<object?>
        {
            0d,
            1d,
            null,
            2d,
            3d
        };


        var hits = JsonPath.Parse(ints).Read("$[?(@)]").AsList();
        MyAssert.ContainsExactly(hits, 0d, 1d, null, 2d, 3d);

        hits = JsonPath.Parse(ints).Read("$[?(@ != null)]").AsList();
        MyAssert.ContainsExactly(hits, 0d, 1d, 2d, 3d);

        var isNull = JsonPath.Parse(ints).Read("$[?(!@)]").AsList();
        Assert.Empty(isNull);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void EqualityCheckDoesNotBreakEvaluation(IProviderTypeTestCase testCase)
    {
        MyAssert.HasOneResult("[{\"value\":\"5\"}]", "$[?(@.value=='5')]", testCase.Configuration);
        MyAssert.HasOneResult("[{\"value\":5}]", "$[?(@.value==5)]", testCase.Configuration);

        MyAssert.HasOneResult("[{\"value\":\"5.1.26\"}]", "$[?(@.value=='5.1.26')]", testCase.Configuration);

        MyAssert.HasNoResults("[{\"value\":\"5\"}]", "$[?(@.value=='5.1.26')]", testCase.Configuration);
        MyAssert.HasNoResults("[{\"value\":5}]", "$[?(@.value=='5.1.26')]", testCase.Configuration);
        MyAssert.HasNoResults("[{\"value\":5.1}]", "$[?(@.value=='5.1.26')]", testCase.Configuration);

        MyAssert.HasNoResults("[{\"value\":\"5.1.26\"}]", "$[?(@.value=='5')]", testCase.Configuration);
        MyAssert.HasNoResults("[{\"value\":\"5.1.26\"}]", "$[?(@.value==5)]", testCase.Configuration);
        MyAssert.HasNoResults("[{\"value\":\"5.1.26\"}]", "$[?(@.value==5.1)]", testCase.Configuration);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void LtCheckDoesNotBreakEvaluation(IProviderTypeTestCase testCase)
    {
        MyAssert.HasOneResult("[{\"value\":\"5\"}]", "$[?(@.value<'7')]", testCase.Configuration);

        MyAssert.HasNoResults("[{\"value\":\"7\"}]", "$[?(@.value<'5')]", testCase.Configuration);

        MyAssert.HasOneResult("[{\"value\":5}]", "$[?(@.value<7)]", testCase.Configuration);
        MyAssert.HasNoResults("[{\"value\":7}]", "$[?(@.value<5)]", testCase.Configuration);

        MyAssert.HasOneResult("[{\"value\":5}]", "$[?(@.value<7.1)]", testCase.Configuration);
        MyAssert.HasNoResults("[{\"value\":7}]", "$[?(@.value<5.1)]", testCase.Configuration);

        MyAssert.HasOneResult("[{\"value\":5.1}]", "$[?(@.value<7)]", testCase.Configuration);
        MyAssert.HasNoResults("[{\"value\":7.1}]", "$[?(@.value<5)]", testCase.Configuration);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void EscapedLiterals(IProviderTypeTestCase testCase)
    {
        MyAssert.HasOneResult("[\"\\'foo\"]", "$[?(@ == '\\'foo')]", testCase.Configuration);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void EscapedLiterals2(IProviderTypeTestCase testCase)
    {
        MyAssert.HasOneResult("[\"\\\\'foo\"]", "$[?(@ == \"\\\\'foo\")]", testCase.Configuration);
    }


    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void EscapePattern(IProviderTypeTestCase testCase)
    {
        MyAssert.HasOneResult("[\"x\"]", "$[?(@ =~ /\\/|x/)]", testCase.Configuration);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void EscapePatternAfterLiteral(IProviderTypeTestCase testCase)
    {
        MyAssert.HasOneResult("[\"x\"]", "$[?(@ == \"abc\" || @ =~ /\\/|x/)]", testCase.Configuration);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void EscapePatternBeforeLiteral(IProviderTypeTestCase testCase)
    {
        MyAssert.HasOneResult("[\"x\"]", "$[?(@ =~ /\\/|x/ || @ == \"abc\")]", testCase.Configuration);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void FilterEvaluationDoesNotBreakPathEvaluation(IProviderTypeTestCase testCase)
    {
        MyAssert.HasOneResult("[{\"s\": \"fo\", \"expected_size\": \"m\"}, {\"s\": \"lo\", \"expected_size\": 2}]",
            "$[?(@.s size @.expected_size)]", testCase.Configuration);
    }
}