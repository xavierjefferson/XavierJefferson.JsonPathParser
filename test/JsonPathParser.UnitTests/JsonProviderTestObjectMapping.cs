using XavierJefferson.JsonPathParser.UnitTests.Extensions;
using XavierJefferson.JsonPathParser.UnitTests.TestData;

namespace XavierJefferson.JsonPathParser.UnitTests;

//
//
public class JsonProviderTestObjectMapping : TestUtils
{
    private static readonly string Json =
        "[" +
        "{\n" +
        "   \"foo\" : \"foo0\",\n" +
        "   \"bar\" : 0,\n" +
        "   \"baz\" : true,\n" +
        "   \"gen\" : {\"prop\" : \"yepp0\"}" +
        "}," +
        "{\n" +
        "   \"foo\" : \"foo1\",\n" +
        "   \"bar\" : 1,\n" +
        "   \"baz\" : true,\n" +
        "   \"gen\" : {\"prop\" : \"yepp1\"}" +
        "}," +
        "{\n" +
        "   \"foo\" : \"foo2\",\n" +
        "   \"bar\" : 2,\n" +
        "   \"baz\" : true,\n" +
        "   \"gen\" : {\"prop\" : \"yepp2\"}" +
        "}" +
        "]";

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void list_of_numbers(IProviderTypeTestCase testCase)
    {
        MyAssert.ContainsExactly(
            JsonPath.Using(testCase.Configuration).Parse(JsonTestData.JsonDocument)
                .Read("$.store.book[*].display-price").AsList(), 8.95D, 12.99D, 8.99D, 22.99D);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void test_type_ref(IProviderTypeTestCase testCase)
    {
        var items = JsonPath.Using(testCase.Configuration).Parse(Json).Read<List<FooBarBaz<Sub>>>("$")
            .Select(i => i.Foo).ToList();

        MyAssert.ContainsExactly(items, "foo0", "foo1", "foo2");
    }


    public class FooBarBaz<T>
    {
        public long Bar { get; set; }
        public bool Baz { get; set; }
        public string Foo { get; set; }
        public T Gen { get; set; }
    }

    public class Sub
    {
        public string Prop { get; set; }
    }
}