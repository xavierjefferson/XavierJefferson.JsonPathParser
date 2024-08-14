using XavierJefferson.JsonPathParser.UnitTests.TestData;

namespace XavierJefferson.JsonPathParser.UnitTests.Internal;

public class JsonContextTest : TestUtils
{
    [Fact]
    public void cached_path_with_predicates()
    {
        var feq = Filter.Create(Criteria.Where("category").Eq("reference"));
        var fne = Filter.Create(Criteria.Where("category").Ne("reference"));

        var jsonDoc = JsonPath.Parse(JsonTestData.JsonDocument);

        var eq = jsonDoc.Read<List<object?>>("$.store.book[?].category", feq);
        var ne = jsonDoc.Read<List<object?>>("$.store.book[?].category", fne);

        Assert.Contains("reference", eq);
        Assert.DoesNotContain("reference", ne);
    }
}