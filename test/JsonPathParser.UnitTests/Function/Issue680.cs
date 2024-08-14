namespace XavierJefferson.JsonPathParser.UnitTests.Function;

public class Issue680
{
    [Fact]
    public void TestIssue680Concat()
    {
        var json = "{ \"key\": \"first\"}";
        var value = JsonPath.Read(json, "concat(\"/\", $.key)");
        Assert.Equal("/first", value);
        json = "{ \"key\": \"second\"}";
        value = JsonPath.Read(json, "concat(\"/\", $.key)");
        Assert.Equal("/second", value);
    }

    [Fact]
    public void TestIssue680Min()
    {
        var json = "{ \"key\": 1}";
        var value = JsonPath.Read(json, "min($.key)");
        Assert.Equal(1d, value);
        json = "{ \"key\": 2}";
        value = JsonPath.Read(json, "min($.key)");
        Assert.Equal(2d, value);
    }

    [Fact]
    public void testIssue680concat_2()
    {
        var context = new Dictionary<string, object?>();
        context["key"] = "first";
        var value = JsonPath.Read(context, "concat(\"/\", $.key)");
        Assert.Equal("/first", value);
        var context2 = new Dictionary<string, object?>();
        context2["key"] = "second";
        value = JsonPath.Read(context2, "concat(\"/\", $.key)");
        Assert.Equal("/second", value);
    }
}