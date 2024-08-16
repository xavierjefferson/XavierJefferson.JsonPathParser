using Newtonsoft.Json;
using XavierJefferson.JsonPathParser.UnitTests.TestData;

namespace XavierJefferson.JsonPathParser.UnitTests;

public class ReadContextTest : TestUtils
{
    public class Rootobject
    {
        public string category { get; set; }
        public string author { get; set; }
        public string title { get; set; }
        [JsonProperty("display-price")]
        public double displayprice { get; set; }
    }

    [Theory]

    [ClassData(typeof(ProviderTypeTestCases))]
    public void json_can_be_fetched_as_string(IProviderTypeTestCase testCase)
    {
        var jsonString1 = JsonPath.Using(testCase.Configuration)
            .Parse((object)JsonTestData.JsonBookDocument).JsonString;
        Assert.StartsWith("{", jsonString1);

        var p = JsonConvert.DeserializeObject<Rootobject>(jsonString1);
        Assert.Equal("reference", p.category);
        Assert.Equal("Nigel Rees", p.author);
        Assert.Equal("Sayings of the Century", p.title);
        Assert.Equal(8.95, p.displayprice);

    }
}