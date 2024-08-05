namespace XavierJefferson.JsonPathParser.UnitTests.Function;

public class Issue629
{
    [Fact]
    public void TestUncloseParenthesis()
    {
        var e = Assert.ThrowsAny<Exception>(() =>
        {
            var jsonPath = JsonPath.Compile("$.A.B.C.D(");
        });

        Assert.StartsWith("Arguments to function:", e.Message);
    }

    [Fact]
    public void TestUncloseParenthesisWithNestedCall()
    {
        var e = Assert.ThrowsAny<Exception>(() =>
        {
            var jsonPath = JsonPath.Compile("$.A.B.C.sum(D()");
        });
        Assert.StartsWith("Arguments to function:", e.Message);
    }
}