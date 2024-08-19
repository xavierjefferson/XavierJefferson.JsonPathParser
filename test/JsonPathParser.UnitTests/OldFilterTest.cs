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
        var jsonProvider = testCase.Configuration.JsonProvider;
        var check = new Dictionary<string, object?>();
        check["foo"] = "foo";
        check["bar"] = null;

        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "bar").Is(jsonProvider, null)).Apply(CreatePredicateContext(check, testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "foo").Is(jsonProvider, "foo")).Apply(CreatePredicateContext(check, testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "foo").Is(jsonProvider, "xxx")).Apply(CreatePredicateContext(check, testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "bar").Is(jsonProvider, "xxx")).Apply(CreatePredicateContext(check, testCase)));
    }


    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void ne_filters_evaluates(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var check = new Dictionary<string, object?>();
        check["foo"] = "foo";
        check["bar"] = null;

        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "foo").Ne(jsonProvider, null)).Apply(CreatePredicateContext(check, testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "foo").Ne(jsonProvider, "not foo")).Apply(CreatePredicateContext(check, testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "foo").Ne(jsonProvider, "foo")).Apply(CreatePredicateContext(check, testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "bar").Ne(jsonProvider, null)).Apply(CreatePredicateContext(check, testCase)));
    }

    [Theory]
    [InlineData(true, "foo", 12D)]
    [InlineData(false, "foo", null)]
    [InlineData(false, "foo", 20D)]
    [InlineData(false, "foo_null", 20D)]
    public void gt_filters_evaluates(bool isTrue, string where, object toCompare)
    {
        var testCase = ProviderTypeTestCases.Cases.First().Value;
        var jsonProvider = testCase.Configuration.JsonProvider;
        var check = new Dictionary<string, object?>();
        check["foo"] = 12.5D;
        check["foo_null"] = null;
        var tmp = Filter.Create(Criteria.Where(jsonProvider, where).Gt(jsonProvider, toCompare)).Apply(CreatePredicateContext(check, testCase));
        Assert.Equal(isTrue, tmp);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void gte_filters_evaluates(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var check = new Dictionary<string, object?>();
        check["foo"] = 12.5D;
        check["foo_null"] = null;

        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "foo").Gte(jsonProvider, 12D)).Apply(CreatePredicateContext(check, testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "foo").Gte(jsonProvider, 12.5D)).Apply(CreatePredicateContext(check, testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "foo").Gte(jsonProvider, null)).Apply(CreatePredicateContext(check, testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "foo").Gte(jsonProvider, 20D)).Apply(CreatePredicateContext(check, testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "foo_null").Gte(jsonProvider, 20D)).Apply(CreatePredicateContext(check, testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void lt_filters_evaluates(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var check = new Dictionary<string, object?>();
        check["foo"] = 10.5D;
        check["foo_null"] = null;

        //Assert.True(Filter.filter(Criteria.where("foo").lt(12D)).Apply(createPredicateContext(check)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "foo").Lt(jsonProvider, null)).Apply(CreatePredicateContext(check, testCase)));
        //Assert.False(Filter.filter(Criteria.where("foo").lt(5D)).Apply(createPredicateContext(check)));
        //Assert.False(Filter.filter(Criteria.where("foo_null").lt(5D)).Apply(createPredicateContext(check)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void lte_filters_evaluates(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var check = new Dictionary<string, object?>();
        check["foo"] = 12.5D;
        check["foo_null"] = null;

        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "foo").Lte(jsonProvider, 13D)).Apply(CreatePredicateContext(check, testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "foo").Lte(jsonProvider, null)).Apply(CreatePredicateContext(check, testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "foo").Lte(jsonProvider, 5D)).Apply(CreatePredicateContext(check, testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "foo_null").Lte(jsonProvider, 5D)).Apply(CreatePredicateContext(check, testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void in_filters_evaluates(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var check = new Dictionary<string, object?>();
        check["item"] = 3;
        check["null_item"] = null;

        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "item").In(jsonProvider, 1, 2, 3)).Apply(CreatePredicateContext(check, testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "item").In((ICollection<object?>)AsList(1, 2, 3), (IJsonProvider)jsonProvider))
            .Apply(CreatePredicateContext(check, testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "item").In(jsonProvider, 4, 5, 6)).Apply(CreatePredicateContext(check, testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "item").In((ICollection<object?>)AsList(4, 5, 6), (IJsonProvider)jsonProvider))
            .Apply(CreatePredicateContext(check, testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "item").In((ICollection<object?>)AsList('A'), (IJsonProvider)jsonProvider))
            .Apply(CreatePredicateContext(check, testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "item").In((ICollection<object?>)AsList((object)null), (IJsonProvider)jsonProvider))
            .Apply(CreatePredicateContext(check, testCase)));

        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "null_item").In(jsonProvider, (object)null))
            .Apply(CreatePredicateContext(check, testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "null_item").In(jsonProvider, 1, 2, 3))
            .Apply(CreatePredicateContext(check, testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void nin_filters_evaluates(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var check = new Dictionary<string, object?>();
        check["item"] = 3;
        check["null_item"] = null;

        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "item").Nin(jsonProvider, 4, 5)).Apply(CreatePredicateContext(check, testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "item").Nin((IJsonProvider)jsonProvider, AsList(4, 5)))
            .Apply(CreatePredicateContext(check, testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "item").Nin((IJsonProvider)jsonProvider, AsList('A')))
            .Apply(CreatePredicateContext(check, testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "null_item").Nin(jsonProvider, 1, 2, 3))
            .Apply(CreatePredicateContext(check, testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "item").Nin((IJsonProvider)jsonProvider, AsList((object?)null)))
            .Apply(CreatePredicateContext(check, testCase)));

        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "item").Nin(jsonProvider, 3)).Apply(CreatePredicateContext(check, testCase)));
        Assert.False(
            Filter.Create(Criteria.Where(jsonProvider, "item").Nin((IJsonProvider)jsonProvider, AsList(3))).Apply(CreatePredicateContext(check, testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void all_filters_evaluates(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var check = new Dictionary<string, object?>();
        check.Add("items", AsList(1, 2, 3));

        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "items").All(jsonProvider, 1, 2, 3))
            .Apply(CreatePredicateContext(check, testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "items").All(jsonProvider, 1, 2, 3, 4))
            .Apply(CreatePredicateContext(check, testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void size_filters_evaluates(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var check = new Dictionary<string, object?>();
        check.Add("items", AsList(1, 2, 3));
        check.Add("items_empty", AsList());

        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "items").Size(jsonProvider, 3)).Apply(CreatePredicateContext(check, testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "items_empty").Size(jsonProvider, 0))
            .Apply(CreatePredicateContext(check, testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "items").Size(jsonProvider, 2)).Apply(CreatePredicateContext(check, testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void exists_filters_evaluates(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var check = new Dictionary<string, object?>();
        check["foo"] = "foo";
        check["foo_null"] = null;

        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "foo").Exists(jsonProvider, true)).Apply(CreatePredicateContext(check, testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "foo").Exists(jsonProvider, false)).Apply(CreatePredicateContext(check, testCase)));

        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "foo_null").Exists(jsonProvider, true))
            .Apply(CreatePredicateContext(check, testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "foo_null").Exists(jsonProvider, false))
            .Apply(CreatePredicateContext(check, testCase)));

        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "bar").Exists(jsonProvider, false)).Apply(CreatePredicateContext(check, testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "bar").Exists(jsonProvider, true)).Apply(CreatePredicateContext(check, testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void type_filters_evaluates(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var check = new Dictionary<string, object?>
        {
            ["string"] = "foo",
            ["string_null"] = null,
            ["int"] = 1,
            ["long"] = 1L,
            ["double"] = 1.12D,
            ["bool"] = true
        };

        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "string_null").Type(typeof(string)))
            .Apply(CreatePredicateContext(check, testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "string").Type(typeof(string)))
            .Apply(CreatePredicateContext(check, testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "string").Type(typeof(double)))
            .Apply(CreatePredicateContext(check, testCase)));

        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "int").Type(typeof(double)))
            .Apply(CreatePredicateContext(check, testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "int").Type(typeof(string)))
            .Apply(CreatePredicateContext(check, testCase)));

        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "long").Type(typeof(double)))
            .Apply(CreatePredicateContext(check, testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "long").Type(typeof(string)))
            .Apply(CreatePredicateContext(check, testCase)));

        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "double").Type(typeof(double)))
            .Apply(CreatePredicateContext(check, testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "double").Type(typeof(string)))
            .Apply(CreatePredicateContext(check, testCase)));

        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "bool").Type(typeof(bool)))
            .Apply(CreatePredicateContext(check, testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "bool").Type(typeof(string)))
            .Apply(CreatePredicateContext(check, testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void pattern_filters_evaluates(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var check = new Dictionary<string, object?>
        {
            ["name"] = "kalle",
            ["name_null"] = null
        };

        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "name_null").Regex(jsonProvider, new Regex(".alle")))
            .Apply(CreatePredicateContext(check, testCase)));
        Assert.True(
            Filter.Create(Criteria.Where(jsonProvider, "name").Regex(jsonProvider, new Regex(".alle")))
                .Apply(CreatePredicateContext(check, testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "name").Regex(jsonProvider, new Regex("KALLE")))
            .Apply(CreatePredicateContext(check, testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "name").Regex(jsonProvider, new Regex("KALLE", RegexOptions.IgnoreCase)))
            .Apply(CreatePredicateContext(check, testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void combine_filter_deep_criteria(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
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


        var filter = Filter.Create(Criteria.Where(jsonProvider, "first-name").Is(jsonProvider, "Jock")
            .And(jsonProvider, "address.state").Is(jsonProvider, "Texas"));

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
        var jsonProvider = testCase.Configuration.JsonProvider;
        var check = new Dictionary<string, object?>
        {
            { "string", "foo" },
            { "string_null", null },
            { "int", 10 },
            { "long", 1L },
            { "double", 1.12D }
        };

        var shouldMarch = Filter.Create(Criteria.Where(jsonProvider, "string").Is(jsonProvider, "foo").And(jsonProvider, "int").Lt(jsonProvider, 11));
        var shouldNotMarch = Filter.Create(Criteria.Where(jsonProvider, "string").Is(jsonProvider, "foo").And(jsonProvider, "int").Gt(jsonProvider, 11));

        Assert.True(shouldMarch.Apply(CreatePredicateContext(check, testCase)));
        Assert.False(shouldNotMarch.Apply(CreatePredicateContext(check, testCase)));
    }


    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void arrays_of_maps_can_be_filtered(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var rootGrandChildA = new Dictionary<string, object?>();
        rootGrandChildA["name"] = "rootGrandChild_A";

        var rootGrandChildB = new Dictionary<string, object?>();
        rootGrandChildB["name"] = "rootGrandChild_B";

        var rootGrandChildC = new Dictionary<string, object?>();
        rootGrandChildC["name"] = "rootGrandChild_C";


        var rootChildA = new Dictionary<string, object?>();
        rootChildA["name"] = "rootChild_A";
        rootChildA.Add("children", AsList(rootGrandChildA, rootGrandChildB, rootGrandChildC));

        var rootChildB = new Dictionary<string, object?>();
        rootChildB["name"] = "rootChild_B";
        rootChildB.Add("children", AsList(rootGrandChildA, rootGrandChildB, rootGrandChildC));

        var rootChildC = new Dictionary<string, object?>();
        rootChildC["name"] = "rootChild_C";
        rootChildC.Add("children", AsList(rootGrandChildA, rootGrandChildB, rootGrandChildC));

        var root = new Dictionary<string, object?>();
        root.Add("children", AsList(rootChildA, rootChildB, rootChildC));


        var customFilter = MakePredicate(context =>
        {
            if (context.Configuration.JsonProvider.GetMapValue(context.Item, "name").Equals("rootGrandChild_A")) return true;
            return false;
        });


        var rootChildFilter = Filter.Create(Criteria.Where(jsonProvider, "name").Regex(jsonProvider, new Regex("rootChild_[A|B]")));
        var rootGrandChildFilter = Filter.Create(Criteria.Where(jsonProvider, "name").Regex(jsonProvider, new Regex("rootGrandChild_[A|B]")));

        var read = JsonPath
            .Read(root, "children[?].children[?, ?]", rootChildFilter, rootGrandChildFilter, customFilter).AsList();
    }


    [Fact]
    public void arrays_of_objects_can_be_filtered()
    {
        var doc = new Dictionary<string, object?>();
        doc["items"] = new object[] { 1d, 2d, 3d };

        var customFilter = MakePredicate(context => { return 1.0d.Equals(context.Item); });


        var res = JsonPath.Read(doc, "$.items[?]", customFilter).AsList();

        Assert.Equal(1d, res[0]);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void filters_can_contain_json_path_expressions(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var doc = testCase.Configuration.JsonProvider.Parse(Document);

        Assert.False(
            Filter.Create(Criteria.Where(jsonProvider, "$.store.bicycle.color").Ne(jsonProvider, "red"))
                .Apply(CreatePredicateContext(doc, testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void not_empty_filter_evaluates(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
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

        var result = JsonPath.Read(doc, "$.fields[?]", Filter.Create(Criteria.Where(jsonProvider, "errors").Empty(false)))
            .AsListOfMap();
        Assert.Single(result);

        var result2 = JsonPath.Read(doc, "$.fields[?]", Filter.Create(Criteria.Where(jsonProvider, "name").Empty(false)))
            .AsListOfMap();
        Assert.Equal(2, result2.Count());
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void contains_filter_evaluates_on_array(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
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


        var filter = Filter.Create(Criteria.Where(jsonProvider, "authors[*].lastName").Contains(jsonProvider, "Waugh"));

        var result = JsonPath.Parse(json).Read("$.store.book[?].title", filter).AsList();

        MyAssert.ContainsExactly(result, "Sword of Honour");
    }


    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void contains_filter_evaluates_on_string(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;

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


        var filter = Filter.Create(Criteria.Where(jsonProvider, "category").Contains(jsonProvider, "fic"));

        var result = JsonPath.Parse(json).Read("$.store.book[?].title", filter).AsList();

        MyAssert.ContainsExactly(result, "Sword of Honour");
    }
}