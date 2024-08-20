using XavierJefferson.JsonPathParser.Filtering;
using XavierJefferson.JsonPathParser.Interfaces;
using XavierJefferson.JsonPathParser.UnitTests.TestData;

namespace XavierJefferson.JsonPathParser.UnitTests;

public class PredicateTest : TestUtils
{
    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void predicates_filters_can_be_applied(IProviderTypeTestCase testCase)
    {
        IReadContext reader = JsonPath.Using(testCase.Configuration)
            .Parse(JsonTestData.JsonDocument);
        var predicate = SimplePredicate.Create(context =>
        {
            return context.GetItem<IDictionary<string, object?>>().ContainsKey("isbn");
        });


        var nn = reader.Read<List<object?>>("$.store.book[?].isbn", predicate);
        MyAssert.ContainsOnly(nn, "0-395-19395-8", "0-553-21311-3");
    }
}