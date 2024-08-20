using XavierJefferson.JsonPathParser.Filtering;
using XavierJefferson.JsonPathParser.UnitTests.TestData;

namespace XavierJefferson.JsonPathParser.UnitTests.Internal;

public class JsonContextTest : TestUtils
{
    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void cached_path_with_predicates(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var feq = Filter.Create(Criteria.Where(jsonProvider, "category").Eq(jsonProvider, "reference"));
        var fne = Filter.Create(Criteria.Where(jsonProvider, "category").Ne(jsonProvider, "reference"));

        var jsonDoc = JsonPath.Parse(JsonTestData.JsonDocument);

        var eq = jsonDoc.Read<List<object?>>("$.store.book[?].category", feq);
        var ne = jsonDoc.Read<List<object?>>("$.store.book[?].category", fne);

        Assert.Contains("reference", eq);
        Assert.DoesNotContain("reference", ne);
    }
}