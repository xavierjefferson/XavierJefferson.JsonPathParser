using XavierJefferson.JsonPathParser.UnitTests.TestData;

namespace XavierJefferson.JsonPathParser.UnitTests;

public class ReadContextTest : TestUtils
{
    [Fact]
    public void json_can_be_fetched_as_string()
    {
        //String expected = "{\"category\":\"reference\",\"author\":\"Nigel Rees\",\"title\":\"Sayings of the Century\",\"display-price\":8.95}";
        const string expected =
            "{    \"category\" : \"reference\",\n   \"author\" : \"Nigel Rees\",\n   \"title\" : \"Sayings of the Century\",\n   \"display-price\" : 8.95\n}";
        var jsonString1 = JsonPath.Using(ConfigurationData.NewtonsoftJsonConfiguration)
            .Parse((object)JsonTestData.JsonBookDocument).JsonString;
        var jsonString2 = JsonPath.Using(ConfigurationData.SystemTextJsonConfiguration)
            .Parse((object)JsonTestData.JsonBookDocument).JsonString;
        Assert.StartsWith("{", jsonString1);
        Assert.StartsWith("{", jsonString2);

        //Xunit.Assert.Equal(expected, jsonString1);
        //Xunit.Assert.Equal(expected, jsonString2);
    }
}