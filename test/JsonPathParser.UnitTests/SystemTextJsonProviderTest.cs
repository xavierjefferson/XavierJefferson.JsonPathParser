using XavierJefferson.JsonPathParser.Exceptions;
using XavierJefferson.JsonPathParser.Mapper;
using XavierJefferson.JsonPathParser.Provider;
using XavierJefferson.JsonPathParser.UnitTests.Extensions;
using XavierJefferson.JsonPathParser.UnitTests.TestData;

namespace XavierJefferson.JsonPathParser.UnitTests;

public class SystemTextJsonProviderTest : TestUtils
{
    private static readonly string Json =
        "[" +
        "{\n" +
        "   \"foo\" : \"foo0\",\n" +
        "   \"bar\" : 0,\n" +
        "   \"baz\" : true,\n" +
        "   \"gen\" : {\"eric\" : \"yepp\"}" +
        "}," +
        "{\n" +
        "   \"foo\" : \"foo1\",\n" +
        "   \"bar\" : 1,\n" +
        "   \"baz\" : true,\n" +
        "   \"gen\" : {\"eric\" : \"yepp\"}" +
        "}," +
        "{\n" +
        "   \"foo\" : \"foo2\",\n" +
        "   \"bar\" : 2,\n" +
        "   \"baz\" : true,\n" +
        "   \"gen\" : {\"eric\" : \"yepp\"}" +
        "}" +
        "]";

    [Fact]
    public void json_can_be_parsed()
    {
        var node = JsonPath.Using(ConfigurationData.SystemTextJsonConfiguration).Parse(JsonTestData.JsonDocument)
            .Read<JpDictionary>("$");
        Assert.Equal("string-value", node["string-property"]);
    }

    [Fact]
    public void strings_are_unwrapped()
    {
        var node = JsonPath.Using(ConfigurationData.SystemTextJsonConfiguration).Parse(JsonTestData.JsonDocument)
            .Read("$.string-property");
        var unwrapped = JsonPath.Using(ConfigurationData.SystemTextJsonConfiguration).Parse(JsonTestData.JsonDocument)
            .Read<string>("$.string-property");

        Assert.Equal("string-value", unwrapped);
        Assert.Equal(node, unwrapped);
    }

    [Fact]
    public void ints_are_unwrapped()
    {
        var node = JsonPath.Using(ConfigurationData.SystemTextJsonConfiguration).Parse(JsonTestData.JsonDocument)
            .Read("$.int-max-property");
        var unwrapped = JsonPath.Using(ConfigurationData.SystemTextJsonConfiguration).Parse(JsonTestData.JsonDocument)
            .Read<int>("$.int-max-property");

        Assert.Equal(int.MaxValue, unwrapped);
        Assert.Equal(Convert.ToInt32(node), unwrapped);
    }

    [Fact]
    public void longs_are_unwrapped()
    {
        var node = JsonPath.Using(ConfigurationData.SystemTextJsonConfiguration).Parse(JsonTestData.JsonDocument)
            .Read("$.long-max-property");
        var val = JsonPath.Using(ConfigurationData.SystemTextJsonConfiguration).Parse(JsonTestData.JsonDocument)
            .Read<double>("$.long-max-property");

        Assert.Equal(long.MaxValue, val);
        Assert.Equal(node, val);
    }

    [Fact]
    public void doubles_are_unwrapped()
    {
        var json = "{'double-property' : 56.78}";

        var node = JsonPath.Using(ConfigurationData.SystemTextJsonConfiguration).Parse(json).Read("$.double-property");
        var val = JsonPath.Using(ConfigurationData.SystemTextJsonConfiguration).Parse(json)
            .Read<double>("$.double-property");

        Assert.Equal(56.78d, val);
        Assert.Equal(node, val);
    }

    //[Fact]
    //public void bigdecimals_are_unwrapped()
    //{
    //    decimal bd = BigDecimal.valueOf(long.MaxValue).Add(BigDecimal.valueOf(10.5));
    //    string json = "{bd-property = " + bd.ToString() + "}";

    //    var node = JsonPath.Using(ConfigurationData.SystemTextJsonConfiguration).Parse(json).Read("$.bd-property");
    //    BigDecimal val = JsonPath.Using(ConfigurationData.SystemTextJsonConfiguration).Parse(json).Read("$.bd-property", typeof(BigDecimal));

    //    Assert.Equal(bd, val);
    //    Assert.Equal(node.getAsBigDecimal(), val);
    //}

    //[Fact]
    //public void small_bigdecimals_are_unwrapped()
    //{
    //    decimal bd = 10.5m;
    //    string json = "{bd-property = " + bd.ToString() + "}";

    //    var node = JsonPath.Using(ConfigurationData.SystemTextJsonConfiguration).Parse(json).Read("$.bd-property");
    //    var val = JsonPath.Using(ConfigurationData.SystemTextJsonConfiguration).Parse(json).Read("$.bd-property", typeof(BigDecimal));

    //    Assert.Equal(bd, val);
    //    Assert.Equal(node.getAsBigDecimal(), val);
    //}

    //[Fact]
    //public void bigintegers_are_unwrapped()
    //{
    //      BigInteger bi = BigInteger.valueOf(long.MaxValue).Add(BigInteger.TEN);
    //    string json = "{bi-property = " + bi.ToString() + "}";

    //    var node = JsonPath.Using(ConfigurationData.SystemTextJsonConfiguration).Parse(json).Read("$.bi-property");
    //    BigInteger val = JsonPath.Using(ConfigurationData.SystemTextJsonConfiguration).Parse(json).Read("$.bi-property", typeof(BigInteger));

    //    Assert.Equal(bi, val);
    //    Assert.Equal(node.getAsBigInteger(), val);
    //}

    [Fact]
    public void small_bigintegers_are_unwrapped()
    {
        var json = "{'bi-property' : " + long.MaxValue + "}";

        var node = JsonPath.Using(ConfigurationData.SystemTextJsonConfiguration).Parse(json).Read("$.bi-property");
        var val = JsonPath.Using(ConfigurationData.SystemTextJsonConfiguration).Parse(json)
            .Read<double>("$.bi-property");

        Assert.Equal(long.MaxValue, val);
        Assert.Equal(node, val);
    }

    [Fact]
    public void int_to_long_mapping()
    {
        Assert.Equal(1L,
            JsonPath.Using(ConfigurationData.SystemTextJsonConfiguration).Parse("{\"val\": 1}")
                .Read("val", typeof(long)));
    }

    [Fact]
    public void an_Integer_can_be_converted_to_a_Double()
    {
        Assert.Equal(1D,
            JsonPath.Using(ConfigurationData.SystemTextJsonConfiguration).Parse("{\"val\": 1}")
                .Read("val", typeof(double)));
    }

    [Fact]
    public void list_of_numbers()
    {
        var objs = JsonPath.Using(ConfigurationData.SystemTextJsonConfiguration).Parse(JsonTestData.JsonDocument)
            .Read("$.store.book[*].display-price").AsList();

        MyAssert.ContainsExactly(objs, 8.95D, 12.99D, 8.99D, 22.99D);
    }

    [Fact]
    public void an_object_can_be_mapped_to_pojo()
    {
        var json = "{\n" +
                   "   \"foo\" : \"foo\",\n" +
                   "   \"bar\" : 10,\n" +
                   "   \"baz\" : true\n" +
                   "}";


        var testClazz = JsonPath.Using(ConfigurationData.SystemTextJsonConfiguration)
            .Parse(json).Read<TestClazz>("$");

        Assert.Equal("foo", testClazz.Foo);
        Assert.Equal(10L, testClazz.Bar);
        Assert.True(testClazz.Baz);
    }

    [Fact]
    public void test_type_ref()
    {
        var list = JsonPath.Using(ConfigurationData.SystemTextJsonConfiguration).Parse(Json)
            .Read<List<FooBarBaz<Gen>>>("$");
        Assert.NotNull(list);
        Assert.Equal("yepp", list[0].Gen.Eric);
    }

    [Fact]
    public void test_type_ref_fail()
    {
        var typeRef = typeof(IList<FooBarBaz<int>>);


        Assert.Throws<MappingException>(() =>
            JsonPath.Using(ConfigurationData.SystemTextJsonConfiguration).Parse(Json).Read("$", typeRef));
    }

    [Fact]
    // https://github.com/json-path/JsonPath/issues/351
    public void no_error_when_mapping_null()
    {
        var configuration = Configuration
            .CreateBuilder()
            .WithMappingProvider(new SystemTextJsonMappingProvider())
            .WithJsonProvider(new SystemTextJsonProvider())
            .WithOptions(Option.DefaultPathLeafToNull, Option.SuppressExceptions)
            .Build();

        var json = "{\"M\":[]}";

        var result = JsonPath.Using(configuration).Parse(json).Read<string>("$.M[0].A[0]");

        Assert.Null(result);
    }


    public class FooBarBaz<T>
    {
        public T Gen { get; set; }
        public string Foo { get; set; }
        public long Bar { get; set; }
        public bool Baz { get; set; }
    }


    public class Gen
    {
        public string Eric { get; set; }
    }

    public class TestClazz
    {
        public string Foo { get; set; }
        public long Bar { get; set; }
        public bool Baz { get; set; }
    }
}