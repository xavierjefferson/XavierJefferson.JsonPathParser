using XavierJefferson.JsonPathParser.Exceptions;

namespace XavierJefferson.JsonPathParser.UnitTests.Function;

public class Issue612
{
    [Fact]
    public void Test()
    {
        var config = Configuration.CreateBuilder()
            .WithOptions(Option.SuppressExceptions)
            .Build();
        var json = "{\"1\":{\"2\":null}}";
        var documentContext = JsonPath.Using(config).Parse(json);
        var query = JsonPath.Compile("$.1.2.a.b.c");
        Assert.Null(documentContext.Read(query));
        Assert.Null(documentContext.Map(query, (a, _) => a));
    }

    [Fact]
    public void Test2()
    {
        var config = Configuration.CreateBuilder()
            .Build();
        var json = "{\"1\":{\"2\":null}}";
        var documentContext = JsonPath.Using(config).Parse(json);
        var query = JsonPath.Compile("$.1.2.a.b.c");

        Assert.Throws<PathNotFoundException>(() => documentContext.Map(query, (a, _) => a));
    }
}