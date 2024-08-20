using XavierJefferson.JsonPathParser.Enums;
using XavierJefferson.JsonPathParser.Exceptions;
using XavierJefferson.JsonPathParser.Interfaces;
using XavierJefferson.JsonPathParser.Mapper;
using XavierJefferson.JsonPathParser.Provider;
using XavierJefferson.JsonPathParser.UnitTests.Extensions;
using XavierJefferson.JsonPathParser.UnitTests.TestData;

namespace XavierJefferson.JsonPathParser.UnitTests;

public class IssuesTest : TestUtils
{
    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void issue_143(IProviderTypeTestCase testCase)
    {
        var json = "{ \"foo\": { \"bar\" : \"val\" }, \"moo\": { \"cow\" : \"val\" } }";

        var configuration = testCase.Configuration.SetOptions(ConfigurationOptionEnum.AsPathList);

        var pathList = JsonPath.Using(configuration).Parse(json).Read(JsonPath.Compile("$.*.bar")).AsList();


        MyAssert.ContainsExactly(pathList, "$['foo']['bar']");
    }


    [Fact]
    public void issue_114_a()
    {
        var json = "{ \"p\":{\n" +
                   "\"s\": { \"u\": \"su\" }, \n" +
                   "\"t\": { \"u\": \"tu\" }\n" +
                   "}}";

        var result = JsonPath.Read(json, "$.p.['s', 't'].u").AsList();
        MyAssert.ContainsExactly(result, "su", "tu");
    }

    [Fact]
    public void issue_114_b()
    {
        var json = "{ \"p\": [\"valp\", \"valq\", \"valr\"] }";

        var result = JsonPath.Read(json, "$.p[?(@ == 'valp')]").AsList();
        MyAssert.ContainsExactly(result, "valp");
    }

    [Fact]
    public void issue_114_c()
    {
        var json = "{ \"p\": [\"valp\", \"valq\", \"valr\"] }";

        var result = JsonPath.Read(json, "$.p[?(@[0] == 'valp')]").AsList();
        Assert.Empty(result);
    }

    [Fact]
    public void issue_114_d()
    {
        Assert.Throws<InvalidPathException>(
            () => JsonPath.Read(JsonTestData.JsonBookDocument, "$..book[(@.length-1)] "));
    }


    [Fact]
    public void issue_151()
    {
        var json = "{\n" +
                   "\"datas\": {\n" +
                   "    \"selling\": {\n" +
                   "        \"3\": [\n" +
                   "            26452067,\n" +
                   "            31625950\n" +
                   "        ],\n" +
                   "        \"206\": [\n" +
                   "            32381852,\n" +
                   "            32489262\n" +
                   "        ],\n" +
                   "        \"208\": [\n" +
                   "            458\n" +
                   "        ],\n" +
                   "        \"217\": [\n" +
                   "            27364892\n" +
                   "        ],\n" +
                   "        \"226\": [\n" +
                   "            30474109\n" +
                   "        ]\n" +
                   "    }\n" +
                   "},\n" +
                   "\"status\": 0\n" +
                   "}";

        var result = JsonPath.Read(json, "$.datas.selling['3','206'].*").AsList();

        MyAssert.ContainsExactly(result, 26452067d, 31625950d, 32381852d, 32489262d);
    }

    [Fact]
    public void full_ones_can_be_filtered()
    {
        var json = "[\n" +
                   " {\"kind\" : \"full\"},\n" +
                   " {\"kind\" : \"empty\"}\n" +
                   "]";

        var fullOnes = JsonPath.Read(json, "$[?(@.kind == 'full')]").AsListOfMap();

        Assert.Single(fullOnes);
        Assert.Equal("full", fullOnes[0]["kind"]);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void issue_36(IProviderTypeTestCase testCase)
    {
        var json = "{\n" +
                   "\n" +
                   " \"arrayOfObjectsAndArrays\" : [ { \"k\" : [\"json\"] }, { \"k\":[\"path\"] }, { \"k\" : [\"is\"] }, { \"k\" : [\"cool\"] } ],\n" +
                   "\n" +
                   "  \"arrayOfObjects\" : [{\"k\" : \"json\"}, {\"k\":\"path\"}, {\"k\" : \"is\"}, {\"k\" : \"cool\"}]\n" +
                   "\n" +
                   " }";

        var o1 = JsonPath.Read(json, "$.arrayOfObjectsAndArrays..k ");
        var o2 = JsonPath.Read(json, "$.arrayOfObjects..k ");

        Assert.Equal("[[\"json\"],[\"path\"],[\"is\"],[\"cool\"]]", testCase.Configuration.JsonProvider.ToJson(o1));
        Assert.Equal("[\"json\",\"path\",\"is\",\"cool\"]", testCase.Configuration.JsonProvider.ToJson(o2));
    }

    [Fact]
    public void issue_11()
    {
        var json = "{ \"foo\" : [] }";
        var result = JsonPath.Read(json, "$.foo[?(@.rel == 'item')][0].uri").AsList();
        Assert.Empty(result);
    }

    [Fact]
    public void issue_11b()
    {
        var json = "{ \"foo\" : [] }";
        Assert.Throws<PathNotFoundException>(() => JsonPath.Read(json, "$.foo[0].uri"));
    }

    [Fact]
    public void issue_15()
    {
        var json = "{ \"store\": {\n" +
                   "    \"book\": [ \n" +
                   "      { \"category\": \"reference\",\n" +
                   "        \"author\": \"Nigel Rees\",\n" +
                   "        \"title\": \"Sayings of the Century\",\n" +
                   "        \"price\": 8.95\n" +
                   "      },\n" +
                   "      { \"category\": \"fiction\",\n" +
                   "        \"author\": \"Herman Melville\",\n" +
                   "        \"title\": \"Moby Dick\",\n" +
                   "        \"isbn\": \"0-553-21311-3\",\n" +
                   "        \"price\": 8.99,\n" +
                   "        \"retailer\": null, \n" +
                   "        \"children\": true,\n" +
                   "        \"number\": -2.99\n" +
                   "      },\n" +
                   "      { \"category\": \"fiction\",\n" +
                   "        \"author\": \"J. R. R. Tolkien\",\n" +
                   "        \"title\": \"The Lord of the Rings\",\n" +
                   "        \"isbn\": \"0-395-19395-8\",\n" +
                   "        \"price\": 22.99,\n" +
                   "        \"number\":0,\n" +
                   "        \"children\": false\n" +
                   "      }\n" +
                   "    ]\n" +
                   "  }\n" +
                   "}";

        var titles = JsonPath.Read(json, "$.store.book[?(@.children==true)].title").AsList();

        Assert.Contains("Moby Dick", titles);
        Assert.Single(titles);
    }


    [Fact]
    public void issue_24()
    {
        using (var stream = GetResourceAsStream("issue_24.json"))
        {
            //Object o = JsonPath.Read(is, "$.project[?(@.template.@key == 'foo')].field[*].@key");
            var o = JsonPath.Read(stream, "$.project.field[*].@key");
            //Object o = JsonPath.Read(is, "$.project.template[?(@.@key == 'foo')].field[*].@key");
        }
    }

    [Fact]
    public void issue_28_string()
    {
        var json = "{\"contents\": [\"one\",\"two\",\"three\"]}";

        var result = JsonPath.Read(json, "$.contents[?(@  == 'two')]").AsList();

        Assert.Single(result);
        Assert.Equal("two", result.First());
    }

    [Fact]
    public void issue_37()
    {
        var json = "[\n" +
                   "    {\n" +
                   "        \"id\": \"9\",\n" +
                   "        \"sku\": \"SKU-001\",\n" +
                   "        \"compatible\": false\n" +
                   "    },\n" +
                   "    {\n" +
                   "        \"id\": \"13\",\n" +
                   "        \"sku\": \"SKU-005\",\n" +
                   "        \"compatible\": true\n" +
                   "    },\n" +
                   "    {\n" +
                   "        \"id\": \"11\",\n" +
                   "        \"sku\": \"SKU-003\",\n" +
                   "        \"compatible\": true\n" +
                   "    }\n" +
                   "]";

        var result = JsonPath.Read(json, "$[?(@.compatible == true)].sku").AsList();

        MyAssert.ContainsExactly(result, "SKU-005", "SKU-003");
    }


    [Fact]
    public void issue_38()
    {
        var json = "{\n" +
                   "   \"datapoints\":[\n" +
                   "      [\n" +
                   "         10.1,\n" +
                   "         13.0\n" +
                   "      ],\n" +
                   "      [\n" +
                   "         21.0,\n" +
                   "         22.0\n" +
                   "      ]\n" +
                   "   ]\n" +
                   "}";

        var result = JsonPath.Read(json, "$.datapoints.[*].[0]").AsList();

        Assert.Equal(10.1d, result[0]);
        Assert.Equal(21d, result[1]);
    }

    [Fact]
    public void issue_39()
    {
        var json = "{\n" +
                   "    \"obj1\": {\n" +
                   "        \"arr\": [\"1\", \"2\"]\n" +
                   "    },\n" +
                   "    \"obj2\": {\n" +
                   "       \"arr\": [\"3\", \"4\"]\n" +
                   "    }\n" +
                   "}\n";

        var result = JsonPath.Read(json, "$..arr").AsList();
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void issue_28_int()
    {
        var json = "{\"contents\": [1,2,3]}";

        var result = JsonPath.Read(json, "$.contents[?(@ == 2)]").AsList();

        Assert.Contains(2d, result);
        Assert.Single(result);
    }

    [Fact]
    public void issue_28_boolean()
    {
        var json = "{\"contents\": [true, true, false]}";

        var result = JsonPath.Read(json, "$.contents[?(@  == true)]").AsList();

        MyAssert.ContainsExactly(result, true, true);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void issue_22(IProviderTypeTestCase testCase)
    {
        var configuration = testCase.Configuration;

        var json = "{\"a\":{\"b\":1,\"c\":2}}";
        Assert.Throws<PathNotFoundException>(() => JsonPath.Parse(json, configuration).Read("a.d"));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void issue_22c(IProviderTypeTestCase testCase)
    {
        //Configuration configuration = Configuration.CreateBuilder().Build();
        var configuration = testCase.Configuration.SetOptions(ConfigurationOptionEnum.SuppressExceptions);

        var json = "{\"a\":{\"b\":1,\"c\":2}}";
        Assert.Null(JsonPath.Parse(json, configuration).Read("a.d"));
    }


    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void issue_22b(IProviderTypeTestCase testCase)
    {
        var json = "{\"a\":[{\"b\":1,\"c\":2},{\"b\":5,\"c\":2}]}";
        var res = JsonPath.Using(testCase.Configuration.SetOptions(ConfigurationOptionEnum.DefaultPathLeafToNull))
            .Parse(json).Read("a[?(@.b==5)].d").AsList();
        MyAssert.ContainsExactly(res, new object[] { null });
    }

    [Fact]
    public void issue_26()
    {
        var json = "[{\"a\":[{\"b\":1,\"c\":2}]}]";
        Assert.Throws<PathNotFoundException>(() => JsonPath.Read(json, "$.a"));
    }

    [Fact]
    public void issue_29_a()
    {
        var json =
            "{\"list\": [ { \"a\":\"atext\", \"b.b-a\":\"batext2\", \"b\":{ \"b-a\":\"batext\", \"b-b\":\"bbtext\" } }, { \"a\":\"atext2\", \"b\":{ \"b-a\":\"batext2\", \"b-b\":\"bbtext2\" } } ] }";

        var result = JsonPath.Read(json, "$.list[?(@['b.b-a']=='batext2')]").AsListOfMap();
        Assert.Single(result);
        var a = result[0]["a"];
        Assert.Equal("atext", a);

        result = JsonPath.Read(json, "$.list[?(@.b.b-a=='batext2')]").AsListOfMap();
        Assert.Single(result);
        Assert.Equal("atext2", result[0]["a"]);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void issue_29_b(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var json =
            "{\"list\": [ { \"a\":\"atext\", \"b\":{ \"b-a\":\"batext\", \"b-b\":\"bbtext\" } }, { \"a\":\"atext2\", \"b\":{ \"b-a\":\"batext2\", \"b-b\":\"bbtext2\" } } ] }";
        var result = JsonPath.Read(json, "$.list[?]", Filter.Create(Criteria.Where(jsonProvider, "b.b-a").Eq(jsonProvider, "batext2"))).AsList();

        Assert.Single(result);
    }

    [Fact]
    public void issue_30()
    {
        var json = "{\"foo\" : {\"@id\" : \"123\", \"$\" : \"hello\"}}";

        Assert.Equal("123", JsonPath.Read(json, "foo.@id"));
        Assert.Equal("hello", JsonPath.Read(json, "foo.$"));
    }

    [Fact]
    public void issue_32()
    {
        var json = "{\"text\" : \"skill: \\\"Heuristic Evaluation\\\"\", \"country\" : \"\"}";
        Assert.Equal("skill: \"Heuristic Evaluation\"", JsonPath.Read(json, "$.text"));
    }

    [Fact]
    public void issue_33()
    {
        var json = "{ \"store\": {\n" +
                   "    \"book\": [ \n" +
                   "      { \"category\": \"reference\",\n" +
                   "        \"author\": {\n" +
                   "          \"name\": \"Author Name\",\n" +
                   "          \"age\": 36\n" +
                   "        },\n" +
                   "        \"title\": \"Sayings of the Century\",\n" +
                   "        \"price\": 8.95\n" +
                   "      },\n" +
                   "      { \"category\": \"fiction\",\n" +
                   "        \"author\": \"Evelyn Waugh\",\n" +
                   "        \"title\": \"Sword of Honour\",\n" +
                   "        \"price\": 12.99,\n" +
                   "        \"isbn\": \"0-553-21311-3\"\n" +
                   "      }\n" +
                   "    ],\n" +
                   "    \"bicycle\": {\n" +
                   "      \"color\": \"red\",\n" +
                   "      \"price\": 19.95\n" +
                   "    }\n" +
                   "  }\n" +
                   "}";

        var result = JsonPath.Read(json, "$.store.book[?(@.author.age == 36)]").AsListOfMap();

        Assert.Single(result);
        MyAssert.ContainsEntry(result[0], "title", "Sayings of the Century");
    }

    [Fact]
    public void array_root()
    {
        var json = "[\n" +
                   "    {\n" +
                   "        \"a\": 1,\n" +
                   "        \"b\": 2,\n" +
                   "        \"c\": 3\n" +
                   "    }\n" +
                   "]";


        Assert.Equal(1d, JsonPath.Read(json, "$[0].a"));
    }

    [Fact]
    public void a_test()
    {
        var json = "{\n" +
                   "  \"success\": true,\n" +
                   "  \"data\": {\n" +
                   "    \"user\": 3,\n" +
                   "    \"own\": null,\n" +
                   "    \"passes\": null,\n" +
                   "    \"completed\": null\n" +
                   "  },\n" +
                   "  \"version\": 1371160528774\n" +
                   "}";

        Assert.Throws<PathNotFoundException>(() => JsonPath.Read(json, "$.data.passes[0].id"));
    }


    [Fact]
    public void issue_42()
    {
        var json = "{" +
                   "        \"list\": [{" +
                   "            \"name\": \"My (String)\" " +
                   "        }] " +
                   "    }";

        var result = JsonPath.Read(json, "$.list[?(@.name == 'My (String)')]").AsListOfMap();

        MyAssert.ContainsExactly(result, GetSingletonMap("name", "My (String)"));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void issue_43(IProviderTypeTestCase testCase)
    {
        var json = "{\"test\":null}";

        Assert.Null(JsonPath.Read(json, "test"));

        Assert.Null(JsonPath.Using(testCase.Configuration.SetOptions(ConfigurationOptionEnum.SuppressExceptions))
            .Parse(json).Read("nonExistingProperty"));

        Assert.Throws<PathNotFoundException>(() => { JsonPath.Read(json, "nonExistingProperty"); });

        Assert.Throws<PathNotFoundException>(() => { JsonPath.Read(json, "nonExisting.property"); });
    }


    [Fact]
    public void issue_45()
    {
        var json = "{\"rootkey\":{\"sub.key\":\"value\"}}";

        Assert.Equal("value", (string)JsonPath.Read(json, "rootkey['sub.key']"));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void issue_46(IProviderTypeTestCase testCase)
    {
        var json = "{\"a\": {}}";

        var configuration = testCase.Configuration.SetOptions(ConfigurationOptionEnum.SuppressExceptions);
        Assert.Null(JsonPath.Using(configuration).Parse(json).Read("a.x"));

        var e = Assert.Throws<PathNotFoundException>(() => { JsonPath.Read(json, "a.x"); });

        Assert.Equal("No results for path: $['a']['x']", e.Message);
    }


    [Fact]
    public void issue_x()
    {
        var json = "{\n" +
                   " \"a\" : [\n" +
                   "   {},\n" +
                   "   { \"b\" : [ { \"c\" : \"foo\"} ] }\n" +
                   " ]\n" +
                   "}\n";

        var result = JsonPath.Read(json, "$.a.*.b.*.c").AsList();

        MyAssert.ContainsExactly(result, "foo");
    }

    [Fact]
    public void issue_60()
    {
        var json = "[\n" +
                   "{\n" +
                   "  \"mpTransactionId\": \"542986eae4b001fd500fdc5b-coreDisc_50-title\",\n" +
                   "  \"resultType\": \"FAIL\",\n" +
                   "  \"narratives\": [\n" +
                   "    {\n" +
                   "      \"ruleProcessingDate\": \"Nov 2, 2014 7:30:20 AM\",\n" +
                   "      \"area\": \"Discovery\",\n" +
                   "      \"phase\": \"Validation\",\n" +
                   "      \"message\": \"Chain does not have a discovery event. Possible it was cut by the date that was picked\",\n" +
                   "      \"ruleName\": \"Validate chain\\u0027s discovery event existence\",\n" +
                   "      \"lastRule\": true\n" +
                   "    }\n" +
                   "  ]\n" +
                   "},\n" +
                   "{\n" +
                   "  \"mpTransactionId\": \"54298649e4b001fd500fda3e-fixCoreDiscovery_3-title\",\n" +
                   "  \"resultType\": \"FAIL\",\n" +
                   "  \"narratives\": [\n" +
                   "    {\n" +
                   "      \"ruleProcessingDate\": \"Nov 2, 2014 7:30:20 AM\",\n" +
                   "      \"area\": \"Discovery\",\n" +
                   "      \"phase\": \"Validation\",\n" +
                   "      \"message\": \"There is one and only discovery event ContentDiscoveredEvent(230) found.\",\n" +
                   "      \"ruleName\": \"Marks existence of discovery event (230)\",\n" +
                   "      \"lastRule\": false\n" +
                   "    },\n" +
                   "    {\n" +
                   "      \"ruleProcessingDate\": \"Nov 2, 2014 7:30:20 AM\",\n" +
                   "      \"area\": \"Discovery/Processing\",\n" +
                   "      \"phase\": \"Validation\",\n" +
                   "      \"message\": \"Chain does not have SLA start event (204) in Discovery or Processing. \",\n" +
                   "      \"ruleName\": \"Check if SLA start event is not present (204). \",\n" +
                   "      \"lastRule\": false\n" +
                   "    },\n" +
                   "    {\n" +
                   "      \"ruleProcessingDate\": \"Nov 2, 2014 7:30:20 AM\",\n" +
                   "      \"area\": \"Processing\",\n" +
                   "      \"phase\": \"Transcode\",\n" +
                   "      \"message\": \"No start transcoding events found\",\n" +
                   "      \"ruleName\": \"Start transcoding events missing (240)\",\n" +
                   "      \"lastRule\": true\n" +
                   "    }\n" +
                   "  ]\n" +
                   "}]";

        var problems = JsonPath.Read(json, "$..narratives[?(@.lastRule==true)].message").AsList();

        Assert.True(problems.ContainsExactly(
            "Chain does not have a discovery event. Possible it was cut by the date that was picked",
            "No start transcoding events found"));
    }

    //http://stackoverflow.com/questions/28596324/jsonpath-filtering-api
    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void stack_overflow_question_1(IProviderTypeTestCase testCase)
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

        var read = JsonPath.Parse(json).Read("$.store.book[?]", filter);
    }

    [Fact]
    public void issue_71()
    {
        var json = "{\n"
                   + "    \"logs\": [\n"
                   + "        {\n"
                   + "            \"message\": \"it's here\",\n"
                   + "            \"id\": 2\n"
                   + "        }\n"
                   + "    ]\n"
                   + "}";

        var result = JsonPath.Read(json, "$.logs[?(@.message == 'it\\'s here')].message").AsList();

        MyAssert.ContainsExactly(result, "it's here");
    }

    //[Fact]
    //public void issue_76()
    //{

    //    string json = "{\n" +
    //            "    \"cpus\": -8.88178419700125e-16,\n" +
    //            "    \"disk\": 0,\n" +
    //            "    \"mem\": 0\n" +
    //            "}";

    //    JSONParser parser = new JSONParser(JSONParser.MODE_PERMISSIVE);
    //    JSONAware jsonModel = (JSONAware)parser.Parse(json);

    //    jsonModel.toJSONString();
    //}

    [Fact]
    public void issue_79()
    {
        var json = "{ \n" +
                   "  \"c\": {\n" +
                   "    \"d1\": {\n" +
                   "      \"url\": [ \"url1\", \"url2\" ]\n" +
                   "    },\n" +
                   "    \"d2\": {\n" +
                   "      \"url\": [ \"url3\", \"url4\",\"url5\" ]\n" +
                   "    }\n" +
                   "  }\n" +
                   "}";

        var res = JsonPath.Read(json, "$.c.*.url[2]").AsList();

        MyAssert.ContainsExactly(res, "url5");
    }
    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void issue_97(IProviderTypeTestCase testCase)
    {
        var json = "{ \"books\": [ " +
                   "{ \"category\": \"fiction\" }, " +
                   "{ \"category\": \"reference\" }, " +
                   "{ \"category\": \"fiction\" }, " +
                   "{ \"category\": \"fiction\" }, " +
                   "{ \"category\": \"reference\" }, " +
                   "{ \"category\": \"fiction\" }, " +
                   "{ \"category\": \"reference\" }, " +
                   "{ \"category\": \"reference\" }, " +
                   "{ \"category\": \"reference\" }, " +
                   "{ \"category\": \"reference\" }, " +
                   "{ \"category\": \"reference\" } ]  }";

        var conf = testCase.Configuration;

        var context = JsonPath.Using(conf).Parse(json);
        context.Delete("$.books[?(@.category == 'reference')]");

        var categories = context.Read("$..category", TypeConstants.ListType).AsList();

        MyAssert.ContainsOnly(categories, "fiction");
    }


    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void issue_99(IProviderTypeTestCase testCase)
    {
        var json = "{\n" +
                   "    \"array1\": [\n" +
                   "        {\n" +
                   "            \"array2\": []\n" +
                   "        },\n" +
                   "        {\n" +
                   "            \"array2\": [\n" +
                   "                {\n" +
                   "                    \"key\": \"test_key\"\n" +
                   "                }\n" +
                   "            ]\n" +
                   "        }\n" +
                   "    ]\n" +
                   "}";

        var configuration = testCase.Configuration.AddOptions(ConfigurationOptionEnum.DefaultPathLeafToNull);

        var keys = JsonPath.Using(configuration).Parse(json).Read("$.array1[*].array2[0].key").AsList();
    }


    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void issue_129(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var match = new Dictionary<string, object?>();
        match["a"] = 1;
        match["b"] = 2;

        var noMatch = new Dictionary<string, object?>();
        noMatch["a"] = -1;
        noMatch["b"] = -2;

        var orig = Filter.Create(Criteria.Where(jsonProvider, "a").Eq(jsonProvider, 1).And(jsonProvider, "b").Eq(jsonProvider, 2));

        var filterAsString = orig.ToString();

        var parsed = Filter.Parse(filterAsString);

        Assert.True(orig.Apply(CreatePredicateContext(match, testCase)));
        Assert.True(parsed.Apply(CreatePredicateContext(match, testCase)));
        Assert.False(orig.Apply(CreatePredicateContext(noMatch, testCase)));
        Assert.False(parsed.Apply(CreatePredicateContext(noMatch, testCase)));
    }

    private IPredicateContext CreatePredicateContext(IDictionary<string, object?> map, IProviderTypeTestCase testCase)
    {
        return new FakePredicateContext(map, testCase);
    }

    [Fact]
    public void issue_131()
    {
        var json = "[\n" +
                   "    {\n" +
                   "        \"foo\": \"1\"\n" +
                   "    },\n" +
                   "    {\n" +
                   "        \"foo\": null\n" +
                   "    },\n" +
                   "    {\n" +
                   "        \"xxx\": null\n" +
                   "    }\n" +
                   "]";

        var result = JsonPath.Read(json, "$[?(@.foo)]").AsListOfMap();

        MyAssert.ContainsExactly(result.Select(i => i["foo"]), "1", null);
    }


    [Fact]
    public void issue_131_2()
    {
        var json = "[\n" +
                   "    {\n" +
                   "        \"foo\": { \"bar\" : \"0\"}\n" +
                   "    },\n" +
                   "    {\n" +
                   "        \"foo\": null\n" +
                   "    },\n" +
                   "    {\n" +
                   "        \"xxx\": null\n" +
                   "    }\n" +
                   "]";

        var result = JsonPath.Read(json, "$[?(@.foo != null)].foo.bar").AsList();

        MyAssert.ContainsExactly(result, "0");


        result = JsonPath.Read(json, "$[?(@.foo.bar)].foo.bar").AsList();

        MyAssert.ContainsExactly(result, "0");
    }


    [Fact]
    public void issue_131_3()
    {
        var json = "[\n" +
                   "    1,\n" +
                   "    2,\n" +
                   "    {\n" +
                   "        \"d\": {\n" +
                   "            \"random\": null,\n" +
                   "            \"date\": 1234\n" +
                   "        },\n" +
                   "        \"l\": \"filler\"\n" +
                   "    }\n" +
                   "]";

        var result = JsonPath.Read(json, "$[2]['d'][?(@.random)]['date']").AsList();

        MyAssert.ContainsExactly(result, 1234d);
    }


    //https://groups.google.com/forum/#!topic/jsonpath/Ojv8XF6LgqM
    [Fact]
    public void using_square_bracket_literal_path()
    {
        var json = "{ \"valid key[@num = 2]\" : \"value\" }";

        var result = JsonPath.Read(json, "$['valid key[@num = 2]']");

        Assert.Equal("value", result);
    }

    [Fact]
    public void issue_90()
    {
        var json = "{\n" +
                   "    \"store\": {\n" +
                   "        \"book\": [\n" +
                   "            {\n" +
                   "                \"price\": \"120\"\n" +
                   "            },\n" +
                   "            {\n" +
                   "                \"price\": 8.95\n" +
                   "            },\n" +
                   "            {\n" +
                   "                \"price\": 12.99\n" +
                   "            },\n" +
                   "            {\n" +
                   "                \"price\": 8.99\n" +
                   "            },\n" +
                   "            {\n" +
                   "                \"price\": 22.99\n" +
                   "            }\n" +
                   "        ]\n" +
                   "    },\n" +
                   "    \"expensive\": 10\n" +
                   "}";

        var numbers = JsonPath.Read(json, "$.store.book[?(@.price <= 90)].price").AsList();

        MyAssert.ContainsExactly(numbers, 8.95D, 12.99D, 8.99D, 22.99D);
    }

    //[Fact]
    //public void github_89()
    //{
    //    var json = new Dictionary<string, object?> { { "foo", "bar" } };

    //    var path = JsonPath.Compile("$.foo");
    //    Assert.Throws<PathNotFoundException>(() =>
    //    {
    //        var tmp = path.Read(json);

    //    });
    //}

    [Theory]

    [ClassData(typeof(ProviderTypeTestCases))]
    public void issue_170(IProviderTypeTestCase testCase)
    {
        var json = "{\n" +
                   "  \"array\": [\n" +
                   "    0,\n" +
                   "    1,\n" +
                   "    2\n" +
                   "  ]\n" +
                   "}";


        var context = JsonPath.Using(testCase.Configuration).Parse(json);
        context = context.Set("$.array[0]", null);
        context = context.Set("$.array[2]", null);

        var list = context.Read("$.array", TypeConstants.ListType).AsList();

        MyAssert.ContainsExactly(list, null, 1d, null);
    }

    //[Fact]
    //public void issue_171()
    //{

    //    string json = "{\n" +
    //            "  \"can delete\": \"this\",\n" +
    //            "  \"can't delete\": \"this\"\n" +
    //            "}";

    //    DocumentContext context = JsonPath.Using(JACKSON_JSON_NODE_CONFIGURATION).Parse(json);
    //    context.set("$.['can delete']", null);
    //    context.set("$.['can\\'t delete']", null);

    //    ObjectNode objectNode = context.read("$");

    //    Assert.True(objectNode["can delete"].isNull());
    //    Assert.True(objectNode["can't delete"].isNull());
    //}

    [Fact]
    public void issue_309()
    {
        var json = "{\n" +
                   "\"jsonArr\": [\n" +
                   "   {\n" +
                   "       \"name\":\"nOne\"\n" +
                   "   },\n" +
                   "   {\n" +
                   "       \"name\":\"nTwo\"\n" +
                   "   }\n" +
                   "   ]\n" +
                   "}";

        var doc = JsonPath.Parse(json).Set("$.jsonArr[1].name", "Jayway");

        Assert.Equal("nOne", doc.Read("$.jsonArr[0].name"));
        Assert.Equal("Jayway", doc.Read("$.jsonArr[1].name"));
    }

    //[Fact]
    //public void issue_378()
    //{

    //    string json = "{\n" +
    //            "    \"nodes\": {\n" +
    //            "        \"unnamed1\": {\n" +
    //            "            \"ntpServers\": [\n" +
    //            "                \"1.2.3.4\"\n" +
    //            "            ]\n" +
    //            "        }\n" +
    //            "    }\n" +
    //            "}";

    //    Configuration configuration = Configuration.CreateBuilder()
    //            .jsonProvider(new JacksonJsonNodeJsonProvider())
    //            .mappingProvider(new JacksonMappingProvider())
    //            .Build();

    //    DocumentContext context = JsonPath.Using(configuration).Parse(json);

    //    string path = "$.nodes[*][?(!([\"1.2.3.4\"] subsetof @.ntpServers))].ntpServers";
    //    JsonPath jsonPath = JsonPath.Compile(path);

    //    context.read(jsonPath);
    //}

    //CS304 (manually written) Issue link: https://github.com/json-path/JsonPath/issues/620
    [Fact]
    public void issue_620_1()
    {
        var json = "{\n" +
                   "  \"complexText\": {\n" +
                   "    \"nestedFields\": [\n" +
                   "      {\n" +
                   "        \"index\": \"0\",\n" +
                   "        \"name\": \"A\"\n" +
                   "      },\n" +
                   "      {\n" +
                   "        \"index\": \"1\",\n" +
                   "        \"name\": \"B\"\n" +
                   "      },\n" +
                   "      {\n" +
                   "        \"index\": \"2\",\n" +
                   "        \"name\": \"C\"\n" +
                   "      }\n" +
                   "    ]\n" +
                   "  }\n" +
                   "}";

        var path1 = "$.concat($.complexText.nestedFields[?(@.index == '2')].name," +
                    "$.complexText.nestedFields[?(@.index == '1')].name," +
                    "$.complexText.nestedFields[?(@.index == '0')].name)";
        var path2 = "$.concat($.complexText.nestedFields[2].name," +
                    "$.complexText.nestedFields[1].name," +
                    "$.complexText.nestedFields[0].name)";

        Assert.Equal("CBA", JsonPath.Read(json, path1));
        Assert.Equal("CBA", JsonPath.Read(json, path2));
    }

    //CS304 (manually written) Issue link: https://github.com/json-path/JsonPath/issues/620
    [Fact]
    public void issue_620_2()
    {
        var json = "{\n" +
                   "  \"complexText\": {\n" +
                   "    \"nestedFields\": [\n" +
                   "      {\n" +
                   "        \"index\": \"0\",\n" +
                   "        \"name\": \"A\"\n" +
                   "      },\n" +
                   "      {\n" +
                   "        \"index\": \"1\",\n" +
                   "        \"name\": \"B\"\n" +
                   "      },\n" +
                   "      {\n" +
                   "        \"index\": \"2\",\n" +
                   "        \"name\": \"C\"\n" +
                   "      }\n" +
                   "    ]\n" +
                   "  }\n" +
                   "}";

        var path1 = "$.concat($.complexText.nestedFields[?(@.index == '2')].name," +
                    "$.complexText.nestedFields[?((@.index == '1')].name," +
                    "$.complexText.nestedFields[?(@.index == '0')].name)";

        var e = Assert.ThrowsAny<Exception>(() =>
        {
            var result = JsonPath.Read(json, path1);
        });
    }

    public class FakePredicateContext : IPredicateContext
    {
        private readonly IDictionary<string, object?> _map;
        private readonly IProviderTypeTestCase _testCase;

        public FakePredicateContext(IDictionary<string, object?> map, IProviderTypeTestCase testCase)
        {
            _map = map;
            _testCase = testCase;
        }

        public object? Item => _map;

        public object? Root => _map;

        public Configuration Configuration => _testCase.Configuration;

        public object? GetItem(Type type)
        {
            var map = _map;
            if (map is null) return null;
            if (map.GetType() == type || map.GetType().IsAssignableFrom(type)) return map;
            return Convert.ChangeType(type, typeof(Type));
        }

        public T? GetItem<T>()
        {
            throw new NotImplementedException();
        }
    }
}