using System.Text.RegularExpressions;
using Moq;
using XavierJefferson.JsonPathParser.Interfaces;
using XavierJefferson.JsonPathParser.UnitTests.Extensions;
using XavierJefferson.JsonPathParser.UnitTests.TestData;

namespace XavierJefferson.JsonPathParser.UnitTests;

public class OldFilterTest : TestUtils
{
    public static readonly string Document =
        "{ \"store\": {\n" +
        "    \"book\": [ \n" +
        "      { \"category\": \"reference\",\n" +
        "        \"author\": \"Nigel Rees\",\n" +
        "        \"title\": \"Sayings of the Century\",\n" +
        "        \"price\": 8.95\n" +
        "      },\n" +
        "      { \"category\": \"fiction\",\n" +
        "        \"author\": \"Evelyn Waugh\",\n" +
        "        \"title\": \"Sword of Honour\",\n" +
        "        \"price\": 12.99\n" +
        "      },\n" +
        "      { \"category\": \"fiction\",\n" +
        "        \"author\": \"Herman Melville\",\n" +
        "        \"title\": \"Moby Dick\",\n" +
        "        \"isbn\": \"0-553-21311-3\",\n" +
        "        \"price\": 8.99\n" +
        "      },\n" +
        "      { \"category\": \"fiction\",\n" +
        "        \"author\": \"J. R. R. Tolkien\",\n" +
        "        \"title\": \"The Lord of the Rings\",\n" +
        "        \"isbn\": \"0-395-19395-8\",\n" +
        "        \"price\": 22.99\n" +
        "      }\n" +
        "    ],\n" +
        "    \"bicycle\": {\n" +
        "      \"color\": \"red\",\n" +
        "      \"price\": 19.95,\n" +
        "      \"foo:bar\": \"fooBar\",\n" +
        "      \"dot.notation\": \"new\"\n" +
        "    }\n" +
        "  }\n" +
        "}";

    private static Configuration Conf(IProviderTypeTestCase testCase)
    {
        return testCase.Configuration;
    }

    private static IJsonProvider Jp(IProviderTypeTestCase testCase)
    {
        return Conf(testCase).JsonProvider;
    }

    private IPredicate MakePredicate(Func<IPredicateContext, bool> predicate)
    {
        var customFilterMock = new Mock<IPredicate>();
        customFilterMock.Setup(i => i.Apply(It.IsAny<IPredicateContext>())).Returns(predicate);
        return customFilterMock.Object;
    }

    public void is_filters_evaluates(IProviderTypeTestCase testCase)
    {
        var check = new Dictionary<string, object>();
        check["foo"] = "foo";
        check["bar"] = null;

        Assert.True(Filter.Create(Criteria.Where("bar").Is(null)).Apply(CreatePredicateContext(check, testCase)));
        Assert.True(Filter.Create(Criteria.Where("foo").Is("foo")).Apply(CreatePredicateContext(check, testCase)));
        Assert.False(Filter.Create(Criteria.Where("foo").Is("xxx")).Apply(CreatePredicateContext(check, testCase)));
        Assert.False(Filter.Create(Criteria.Where("bar").Is("xxx")).Apply(CreatePredicateContext(check, testCase)));
    }


    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void ne_filters_evaluates(IProviderTypeTestCase testCase)
    {
        var check = new Dictionary<string, object?>();
        check["foo"] = "foo";
        check["bar"] = null;

        Assert.True(Filter.Create(Criteria.Where("foo").Ne(null)).Apply(CreatePredicateContext(check, testCase)));
        Assert.True(Filter.Create(Criteria.Where("foo").Ne("not foo")).Apply(CreatePredicateContext(check, testCase)));
        Assert.False(Filter.Create(Criteria.Where("foo").Ne("foo")).Apply(CreatePredicateContext(check, testCase)));
        Assert.False(Filter.Create(Criteria.Where("bar").Ne(null)).Apply(CreatePredicateContext(check, testCase)));
    }

    [Theory]
    [InlineData(true, "foo", 12D)]
    [InlineData(false, "foo", null)]
    [InlineData(false, "foo", 20D)]
    [InlineData(false, "foo_null", 20D)]
    public void gt_filters_evaluates(bool isTrue, string where, object toCompare)
    {
        var testCase = ProviderTypeTestCases.Cases.First().Value;
        var check = new Dictionary<string, object?>();
        check["foo"] = 12.5D;
        check["foo_null"] = null;
        var tmp = Filter.Create(Criteria.Where(where).Gt(toCompare)).Apply(CreatePredicateContext(check, testCase));
        Assert.Equal(isTrue, tmp);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void gte_filters_evaluates(IProviderTypeTestCase testCase)
    {
        var check = new Dictionary<string, object>();
        check["foo"] = 12.5D;
        check["foo_null"] = null;

        Assert.True(Filter.Create(Criteria.Where("foo").Gte(12D)).Apply(CreatePredicateContext(check, testCase)));
        Assert.True(Filter.Create(Criteria.Where("foo").Gte(12.5D)).Apply(CreatePredicateContext(check, testCase)));
        Assert.False(Filter.Create(Criteria.Where("foo").Gte(null)).Apply(CreatePredicateContext(check, testCase)));
        Assert.False(Filter.Create(Criteria.Where("foo").Gte(20D)).Apply(CreatePredicateContext(check, testCase)));
        Assert.False(Filter.Create(Criteria.Where("foo_null").Gte(20D)).Apply(CreatePredicateContext(check, testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void lt_filters_evaluates(IProviderTypeTestCase testCase)
    {
        var check = new Dictionary<string, object>();
        check["foo"] = 10.5D;
        check["foo_null"] = null;

        //Assert.True(Filter.filter(Criteria.where("foo").lt(12D)).Apply(createPredicateContext(check)));
        Assert.False(Filter.Create(Criteria.Where("foo").Lt(null)).Apply(CreatePredicateContext(check, testCase)));
        //Assert.False(Filter.filter(Criteria.where("foo").lt(5D)).Apply(createPredicateContext(check)));
        //Assert.False(Filter.filter(Criteria.where("foo_null").lt(5D)).Apply(createPredicateContext(check)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void lte_filters_evaluates(IProviderTypeTestCase testCase)
    {
        var check = new Dictionary<string, object>();
        check["foo"] = 12.5D;
        check["foo_null"] = null;

        Assert.True(Filter.Create(Criteria.Where("foo").Lte(13D)).Apply(CreatePredicateContext(check, testCase)));
        Assert.False(Filter.Create(Criteria.Where("foo").Lte(null)).Apply(CreatePredicateContext(check, testCase)));
        Assert.False(Filter.Create(Criteria.Where("foo").Lte(5D)).Apply(CreatePredicateContext(check, testCase)));
        Assert.False(Filter.Create(Criteria.Where("foo_null").Lte(5D)).Apply(CreatePredicateContext(check, testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void in_filters_evaluates(IProviderTypeTestCase testCase)
    {
        var check = new Dictionary<string, object>();
        check["item"] = 3;
        check["null_item"] = null;

        Assert.True(Filter.Create(Criteria.Where("item").In(1, 2, 3)).Apply(CreatePredicateContext(check, testCase)));
        Assert.True(Filter.Create(Criteria.Where("item").In(AsList(1, 2, 3)))
            .Apply(CreatePredicateContext(check, testCase)));
        Assert.False(Filter.Create(Criteria.Where("item").In(4, 5, 6)).Apply(CreatePredicateContext(check, testCase)));
        Assert.False(Filter.Create(Criteria.Where("item").In(AsList(4, 5, 6)))
            .Apply(CreatePredicateContext(check, testCase)));
        Assert.False(Filter.Create(Criteria.Where("item").In(AsList('A')))
            .Apply(CreatePredicateContext(check, testCase)));
        Assert.False(Filter.Create(Criteria.Where("item").In(AsList((object)null)))
            .Apply(CreatePredicateContext(check, testCase)));

        Assert.True(Filter.Create(Criteria.Where("null_item").In((object)null))
            .Apply(CreatePredicateContext(check, testCase)));
        Assert.False(Filter.Create(Criteria.Where("null_item").In(1, 2, 3))
            .Apply(CreatePredicateContext(check, testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void nin_filters_evaluates(IProviderTypeTestCase testCase)
    {
        var check = new Dictionary<string, object>();
        check["item"] = 3;
        check["null_item"] = null;

        Assert.True(Filter.Create(Criteria.Where("item").Nin(4, 5)).Apply(CreatePredicateContext(check, testCase)));
        Assert.True(Filter.Create(Criteria.Where("item").Nin(AsList(4, 5)))
            .Apply(CreatePredicateContext(check, testCase)));
        Assert.True(Filter.Create(Criteria.Where("item").Nin(AsList('A')))
            .Apply(CreatePredicateContext(check, testCase)));
        Assert.True(Filter.Create(Criteria.Where("null_item").Nin(1, 2, 3))
            .Apply(CreatePredicateContext(check, testCase)));
        Assert.True(Filter.Create(Criteria.Where("item").Nin(AsList((object?)null)))
            .Apply(CreatePredicateContext(check, testCase)));

        Assert.False(Filter.Create(Criteria.Where("item").Nin(3)).Apply(CreatePredicateContext(check, testCase)));
        Assert.False(
            Filter.Create(Criteria.Where("item").Nin(AsList(3))).Apply(CreatePredicateContext(check, testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void all_filters_evaluates(IProviderTypeTestCase testCase)
    {
        var check = new Dictionary<string, object>();
        check.Add("items", AsList(1, 2, 3));

        Assert.True(Filter.Create(Criteria.Where("items").All(1, 2, 3))
            .Apply(CreatePredicateContext(check, testCase)));
        Assert.False(Filter.Create(Criteria.Where("items").All(1, 2, 3, 4))
            .Apply(CreatePredicateContext(check, testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void size_filters_evaluates(IProviderTypeTestCase testCase)
    {
        var check = new Dictionary<string, object?>();
        check.Add("items", AsList(1, 2, 3));
        check.Add("items_empty", AsList());

        Assert.True(Filter.Create(Criteria.Where("items").Size(3)).Apply(CreatePredicateContext(check, testCase)));
        Assert.True(Filter.Create(Criteria.Where("items_empty").Size(0))
            .Apply(CreatePredicateContext(check, testCase)));
        Assert.False(Filter.Create(Criteria.Where("items").Size(2)).Apply(CreatePredicateContext(check, testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void exists_filters_evaluates(IProviderTypeTestCase testCase)
    {
        var check = new Dictionary<string, object>();
        check["foo"] = "foo";
        check["foo_null"] = null;

        Assert.True(Filter.Create(Criteria.Where("foo").Exists(true)).Apply(CreatePredicateContext(check, testCase)));
        Assert.False(Filter.Create(Criteria.Where("foo").Exists(false)).Apply(CreatePredicateContext(check, testCase)));

        Assert.True(Filter.Create(Criteria.Where("foo_null").Exists(true))
            .Apply(CreatePredicateContext(check, testCase)));
        Assert.False(Filter.Create(Criteria.Where("foo_null").Exists(false))
            .Apply(CreatePredicateContext(check, testCase)));

        Assert.True(Filter.Create(Criteria.Where("bar").Exists(false)).Apply(CreatePredicateContext(check, testCase)));
        Assert.False(Filter.Create(Criteria.Where("bar").Exists(true)).Apply(CreatePredicateContext(check, testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void type_filters_evaluates(IProviderTypeTestCase testCase)
    {
        var check = new Dictionary<string, object>
        {
            ["string"] = "foo",
            ["string_null"] = null,
            ["int"] = 1,
            ["long"] = 1L,
            ["double"] = 1.12D,
            ["bool"] = true
        };

        Assert.False(Filter.Create(Criteria.Where("string_null").Type(typeof(string)))
            .Apply(CreatePredicateContext(check, testCase)));
        Assert.True(Filter.Create(Criteria.Where("string").Type(typeof(string)))
            .Apply(CreatePredicateContext(check, testCase)));
        Assert.False(Filter.Create(Criteria.Where("string").Type(typeof(double)))
            .Apply(CreatePredicateContext(check, testCase)));

        Assert.True(Filter.Create(Criteria.Where("int").Type(typeof(double)))
            .Apply(CreatePredicateContext(check, testCase)));
        Assert.False(Filter.Create(Criteria.Where("int").Type(typeof(string)))
            .Apply(CreatePredicateContext(check, testCase)));

        Assert.True(Filter.Create(Criteria.Where("long").Type(typeof(double)))
            .Apply(CreatePredicateContext(check, testCase)));
        Assert.False(Filter.Create(Criteria.Where("long").Type(typeof(string)))
            .Apply(CreatePredicateContext(check, testCase)));

        Assert.True(Filter.Create(Criteria.Where("double").Type(typeof(double)))
            .Apply(CreatePredicateContext(check, testCase)));
        Assert.False(Filter.Create(Criteria.Where("double").Type(typeof(string)))
            .Apply(CreatePredicateContext(check, testCase)));

        Assert.True(Filter.Create(Criteria.Where("bool").Type(typeof(bool)))
            .Apply(CreatePredicateContext(check, testCase)));
        Assert.False(Filter.Create(Criteria.Where("bool").Type(typeof(string)))
            .Apply(CreatePredicateContext(check, testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void pattern_filters_evaluates(IProviderTypeTestCase testCase)
    {
        var check = new Dictionary<string, object>
        {
            ["name"] = "kalle",
            ["name_null"] = null
        };

        Assert.False(Filter.Create(Criteria.Where("name_null").Regex(new Regex(".alle")))
            .Apply(CreatePredicateContext(check, testCase)));
        Assert.True(
            Filter.Create(Criteria.Where("name").Regex(new Regex(".alle")))
                .Apply(CreatePredicateContext(check, testCase)));
        Assert.False(Filter.Create(Criteria.Where("name").Regex(new Regex("KALLE")))
            .Apply(CreatePredicateContext(check, testCase)));
        Assert.True(Filter.Create(Criteria.Where("name").Regex(new Regex("KALLE", RegexOptions.IgnoreCase)))
            .Apply(CreatePredicateContext(check, testCase)));
    }

    [Fact]
    public void combine_filter_deep_criteria()
    {
        var json = "[\n" +
                   "   {\n" +
                   "      \"first-name\" : \"John\",\n" +
                   "      \"last-name\" : \"Irving\",\n" +
                   "      \"address\" : {\"state\" : \"Texas\"}\n" +
                   "   },\n" +
                   "   {\n" +
                   "      \"first-name\" : \"Jock\",\n" +
                   "      \"last-name\" : \"Ewing\",\n" +
                   "      \"address\" : {\"state\" : \"Texas\"}\n" +
                   "   },\n" +
                   "   {\n" +
                   "      \"first-name\" : \"Jock\",\n" +
                   "      \"last-name\" : \"Barnes\",\n" +
                   "      \"address\" : {\"state\" : \"Nevada\"}\n" +
                   "   } \n" +
                   "]";


        var filter = Filter.Create(Criteria.Where("first-name").Is("Jock")
            .And("address.state").Is("Texas"));

        var jocksInTexas1 = JsonPath.Read(json, "$[?]", filter).AsListOfMap();
        var jocksInTexas2 = JsonPath.Read(json, "$[?(@.first-name == 'Jock' && @.address.state == 'Texas')]")
            .AsListOfMap();


        JsonPath.Parse(json);

        Assert.Equal("Texas", JsonPath.Read(jocksInTexas1, "$[0].address.state"));
        Assert.Equal("Jock", JsonPath.Read(jocksInTexas1, "$[0].first-name"));
        Assert.Equal("Ewing", JsonPath.Read(jocksInTexas1, "$[0].last-name"));
    }

    //-------------------------------------------------
    //
    // Single filter tests
    //
    //-------------------------------------------------

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void filters_can_be_combined(IProviderTypeTestCase testCase)
    {
        var check = new Dictionary<string, object>
        {
            { "string", "foo" },
            { "string_null", null },
            { "int", 10 },
            { "long", 1L },
            { "double", 1.12D }
        };

        var shouldMarch = Filter.Create(Criteria.Where("string").Is("foo").And("int").Lt(11));
        var shouldNotMarch = Filter.Create(Criteria.Where("string").Is("foo").And("int").Gt(11));

        Assert.True(shouldMarch.Apply(CreatePredicateContext(check, testCase)));
        Assert.False(shouldNotMarch.Apply(CreatePredicateContext(check, testCase)));
    }


    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void arrays_of_maps_can_be_filtered(IProviderTypeTestCase testCase)
    {
        var rootGrandChildA = new Dictionary<string, object>();
        rootGrandChildA["name"] = "rootGrandChild_A";

        var rootGrandChildB = new Dictionary<string, object>();
        rootGrandChildB["name"] = "rootGrandChild_B";

        var rootGrandChildC = new Dictionary<string, object>();
        rootGrandChildC["name"] = "rootGrandChild_C";


        var rootChildA = new Dictionary<string, object>();
        rootChildA["name"] = "rootChild_A";
        rootChildA.Add("children", AsList(rootGrandChildA, rootGrandChildB, rootGrandChildC));

        var rootChildB = new Dictionary<string, object>();
        rootChildB["name"] = "rootChild_B";
        rootChildB.Add("children", AsList(rootGrandChildA, rootGrandChildB, rootGrandChildC));

        var rootChildC = new Dictionary<string, object>();
        rootChildC["name"] = "rootChild_C";
        rootChildC.Add("children", AsList(rootGrandChildA, rootGrandChildB, rootGrandChildC));

        var root = new Dictionary<string, object>();
        root.Add("children", AsList(rootChildA, rootChildB, rootChildC));


        var customFilter = MakePredicate(ctx =>
        {
            if (ctx.Configuration.JsonProvider.GetMapValue(ctx.Item, "name").Equals("rootGrandChild_A")) return true;
            return false;
        });


        var rootChildFilter = Filter.Create(Criteria.Where("name").Regex(new Regex("rootChild_[A|B]")));
        var rootGrandChildFilter = Filter.Create(Criteria.Where("name").Regex(new Regex("rootGrandChild_[A|B]")));

        var read = JsonPath
            .Read(root, "children[?].children[?, ?]", rootChildFilter, rootGrandChildFilter, customFilter).AsList();
    }


    [Fact]
    public void arrays_of_objects_can_be_filtered()
    {
        var doc = new Dictionary<string, object>();
        doc["items"] = new object[] { 1d, 2d, 3d };

        var customFilter = MakePredicate(ctx => { return 1.0d.Equals(ctx.Item); });


        var res = JsonPath.Read(doc, "$.items[?]", customFilter).AsList();

        Assert.Equal(1d, res[0]);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void filters_can_contain_json_path_expressions(IProviderTypeTestCase testCase)
    {
        var doc = testCase.Configuration.JsonProvider.Parse(Document);

        Assert.False(
            Filter.Create(Criteria.Where("$.store.bicycle.color").Ne("red"))
                .Apply(CreatePredicateContext(doc, testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void not_empty_filter_evaluates(IProviderTypeTestCase testCase)
    {
        var json = "{\n" +
                   "    \"fields\": [\n" +
                   "        {\n" +
                   "            \"errors\": [], \n" +
                   "            \"name\": \"\", \n" +
                   "            \"empty\": true \n" +
                   "        }, \n" +
                   "        {\n" +
                   "            \"errors\": [], \n" +
                   "            \"name\": \"foo\"\n" +
                   "        }, \n" +
                   "        {\n" +
                   "            \"errors\": [\n" +
                   "                \"first\", \n" +
                   "                \"second\"\n" +
                   "            ], \n" +
                   "            \"name\": \"invalid\"\n" +
                   "        }\n" +
                   "    ]\n" +
                   "}\n";


        var doc = testCase.Configuration.JsonProvider.Parse(json);

        var result = JsonPath.Read(doc, "$.fields[?]", Filter.Create(Criteria.Where("errors").Empty(false)))
            .AsListOfMap();
        Assert.Single(result);

        var result2 = JsonPath.Read(doc, "$.fields[?]", Filter.Create(Criteria.Where("name").Empty(false)))
            .AsListOfMap();
        Assert.Equal(2, result2.Count());
    }

    [Fact]
    public void contains_filter_evaluates_on_array()
    {
        var json = "{\n" +
                   "\"store\": {\n" +
                   "    \"book\": [\n" +
                   "        {\n" +
                   "            \"category\": \"reference\",\n" +
                   "            \"authors\" : [\n" +
                   "                 {\n" +
                   "                     \"firstName\" : \"Nigel\",\n" +
                   "                     \"lastName\" :  \"Rees\"\n" +
                   "                  }\n" +
                   "            ],\n" +
                   "            \"title\": \"Sayings of the Century\",\n" +
                   "            \"price\": 8.95\n" +
                   "        },\n" +
                   "        {\n" +
                   "            \"category\": \"fiction\",\n" +
                   "            \"authors\": [\n" +
                   "                 {\n" +
                   "                     \"firstName\" : \"Evelyn\",\n" +
                   "                     \"lastName\" :  \"Waugh\"\n" +
                   "                  },\n" +
                   "                 {\n" +
                   "                     \"firstName\" : \"Another\",\n" +
                   "                     \"lastName\" :  \"Author\"\n" +
                   "                  }\n" +
                   "            ],\n" +
                   "            \"title\": \"Sword of Honour\",\n" +
                   "            \"price\": 12.99\n" +
                   "        }\n" +
                   "    ]\n" +
                   "  }\n" +
                   "}";


        var filter = Filter.Create(Criteria.Where("authors[*].lastName").Contains("Waugh"));

        var result = JsonPath.Parse(json).Read("$.store.book[?].title", filter).AsList();

        MyAssert.ContainsExactly(result, "Sword of Honour");
    }


    [Fact]
    public void contains_filter_evaluates_on_string()
    {
        var json = "{\n" +
                   "\"store\": {\n" +
                   "    \"book\": [\n" +
                   "        {\n" +
                   "            \"category\": \"reference\",\n" +
                   "            \"title\": \"Sayings of the Century\",\n" +
                   "            \"price\": 8.95\n" +
                   "        },\n" +
                   "        {\n" +
                   "            \"category\": \"fiction\",\n" +
                   "            \"title\": \"Sword of Honour\",\n" +
                   "            \"price\": 12.99\n" +
                   "        }\n" +
                   "    ]\n" +
                   "  }\n" +
                   "}";


        var filter = Filter.Create(Criteria.Where("category").Contains("fic"));

        var result = JsonPath.Parse(json).Read("$.store.book[?].title", filter).AsList();

        MyAssert.ContainsExactly(result, "Sword of Honour");
    }
}