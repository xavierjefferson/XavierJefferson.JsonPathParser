using XavierJefferson.JsonPathParser.Enums;
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

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void json_can_be_parsed(IProviderTypeTestCase testCase)
    {
        var node = JsonPath.Using(testCase.Configuration).Parse(JsonTestData.JsonDocument)
            .Read<IDictionary<string, object?>>("$");
        Assert.Equal("string-value", node["string-property"]);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void strings_are_unwrapped(IProviderTypeTestCase testCase)
    {
        var node = JsonPath.Using(testCase.Configuration).Parse(JsonTestData.JsonDocument)
            .Read("$.string-property");
        var unwrapped = JsonPath.Using(testCase.Configuration).Parse(JsonTestData.JsonDocument)
            .Read<string>("$.string-property");

        Assert.Equal("string-value", unwrapped);
        Assert.Equal(node, unwrapped);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void ints_are_unwrapped(IProviderTypeTestCase testCase)
    {
        var node = JsonPath.Using(testCase.Configuration).Parse(JsonTestData.JsonDocument)
            .Read("$.int-max-property");
        var unwrapped = JsonPath.Using(testCase.Configuration).Parse(JsonTestData.JsonDocument)
            .Read<int>("$.int-max-property");

        Assert.Equal(int.MaxValue, unwrapped);
        Assert.Equal(Convert.ToInt32(node), unwrapped);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void longs_are_unwrapped(IProviderTypeTestCase testCase)
    {
        var node = JsonPath.Using(testCase.Configuration).Parse(JsonTestData.JsonDocument)
            .Read("$.long-max-property");
        var val = JsonPath.Using(testCase.Configuration).Parse(JsonTestData.JsonDocument)
            .Read<double>("$.long-max-property");

        Assert.Equal(long.MaxValue, val);
        Assert.Equal(node, val);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void doubles_are_unwrapped(IProviderTypeTestCase testCase)
    {
        var json = "{'double-property' : 56.78}";

        var node = JsonPath.Using(testCase.Configuration).Parse(json).Read("$.double-property");
        var val = JsonPath.Using(testCase.Configuration).Parse(json)
            .Read<double>("$.double-property");

        Assert.Equal(56.78d, val);
        Assert.Equal(node, val);
    }

    //[Theory][ClassData(typeof(ProviderTypeTestCases))]
    //public void bigdecimals_are_unwrapped()
    //{
    //    decimal bd = BigDecimal.valueOf(long.MaxValue).Add(BigDecimal.valueOf(10.5));
    //    string json = "{bd-property = " + bd.ToString() + "}";

    //    var node = JsonPath.Using(testCase.Configuration).Parse(json).Read("$.bd-property");
    //    BigDecimal val = JsonPath.Using(testCase.Configuration).Parse(json).Read("$.bd-property", typeof(BigDecimal));

    //    Assert.Equal(bd, val);
    //    Assert.Equal(node.getAsBigDecimal(), val);
    //}

    //[Theory][ClassData(typeof(ProviderTypeTestCases))]
    //public void small_bigdecimals_are_unwrapped()
    //{
    //    decimal bd = 10.5m;
    //    string json = "{bd-property = " + bd.ToString() + "}";

    //    var node = JsonPath.Using(testCase.Configuration).Parse(json).Read("$.bd-property");
    //    var val = JsonPath.Using(testCase.Configuration).Parse(json).Read("$.bd-property", typeof(BigDecimal));

    //    Assert.Equal(bd, val);
    //    Assert.Equal(node.getAsBigDecimal(), val);
    //}

    //[Theory][ClassData(typeof(ProviderTypeTestCases))]
    //public void bigintegers_are_unwrapped()
    //{
    //      BigInteger bi = BigInteger.valueOf(long.MaxValue).Add(BigInteger.TEN);
    //    string json = "{bi-property = " + bi.ToString() + "}";

    //    var node = JsonPath.Using(testCase.Configuration).Parse(json).Read("$.bi-property");
    //    BigInteger val = JsonPath.Using(testCase.Configuration).Parse(json).Read("$.bi-property", typeof(BigInteger));

    //    Assert.Equal(bi, val);
    //    Assert.Equal(node.getAsBigInteger(), val);
    //}

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void small_bigintegers_are_unwrapped(IProviderTypeTestCase testCase)
    {
        var json = "{'bi-property' : " + long.MaxValue + "}";

        var node = JsonPath.Using(testCase.Configuration).Parse(json).Read("$.bi-property");
        var val = JsonPath.Using(testCase.Configuration).Parse(json)
            .Read<double>("$.bi-property");

        Assert.Equal(long.MaxValue, val);
        Assert.Equal(node, val);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void int_to_long_mapping(IProviderTypeTestCase testCase)
    {
        Assert.Equal(1L,
            JsonPath.Using(testCase.Configuration).Parse("{\"val\": 1}")
                .Read("val", typeof(long)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void an_Integer_can_be_converted_to_a_Double(IProviderTypeTestCase testCase)
    {
        Assert.Equal(1D,
            JsonPath.Using(testCase.Configuration).Parse("{\"val\": 1}")
                .Read("val", typeof(double)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void list_of_numbers(IProviderTypeTestCase testCase)
    {
        var objs = JsonPath.Using(testCase.Configuration).Parse(JsonTestData.JsonDocument)
            .Read("$.store.book[*].display-price").AsList();

        MyAssert.ContainsExactly(objs, 8.95D, 12.99D, 8.99D, 22.99D);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void an_object_can_be_mapped_to_pojo(IProviderTypeTestCase testCase)
    {
        var json = "{\n" +
                   "   \"foo\" : \"foo\",\n" +
                   "   \"bar\" : 10,\n" +
                   "   \"baz\" : true\n" +
                   "}";


        var testClazz = JsonPath.Using(testCase.Configuration)
            .Parse(json).Read<TestClazz>("$");

        Assert.Equal("foo", testClazz.Foo);
        Assert.Equal(10L, testClazz.Bar);
        Assert.True(testClazz.Baz);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void test_type_ref(IProviderTypeTestCase testCase)
    {
        var list = JsonPath.Using(testCase.Configuration).Parse(Json)
            .Read<List<FooBarBaz<Gen>>>("$");
        Assert.NotNull(list);
        Assert.Equal("yepp", list[0].Gen.Eric);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void test_type_ref_fail(IProviderTypeTestCase testCase)
    {
        var typeRef = typeof(IList<FooBarBaz<int>>);


        Assert.Throws<MappingException>(() =>
            JsonPath.Using(testCase.Configuration).Parse(Json).Read("$", typeRef));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    // https://github.com/json-path/JsonPath/issues/351
    public void no_error_when_mapping_null(IProviderTypeTestCase testCase)
    {
        var configuration = testCase.Configuration.SetOptions(ConfigurationOptionEnum.DefaultPathLeafToNull, ConfigurationOptionEnum.SuppressExceptions);

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