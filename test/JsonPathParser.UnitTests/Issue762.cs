namespace XavierJefferson.JsonPathParser.UnitTests;

/**
 * test for issue 762
 */
public class Issue762
{
    [Theory]
    [InlineData("5", 5)]
    [InlineData("5", 5.0)]
    [InlineData("true", true)]
    [InlineData("false", false)]
    public void TestParseJsonValue(string expected, object input)
    {
        Assert.Equal(expected, JsonPath.Parse(input).JsonString);
    }
}