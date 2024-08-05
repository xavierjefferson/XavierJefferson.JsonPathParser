using XavierJefferson.JsonPathParser.Mapper;
using XavierJefferson.JsonPathParser.Provider;
using XavierJefferson.JsonPathParser.UnitTests.Extensions;

namespace XavierJefferson.JsonPathParser.UnitTests;

public class TestSuppressExceptions
{
    [Fact]
    public void TestSuppressExceptionsIsRespected()
    {
        var parseContext = JsonPath.Using(
            new ConfigurationBuilder().WithJsonProvider(new NewtonsoftJsonProvider())
                .WithMappingProvider(new NewtonsoftJsonMappingProvider()).WithOptions(Option.SuppressExceptions)
                .Build());
        var json = "{}";
        Assert.Null(parseContext.Parse(json).Read(JsonPath.Compile("$.missing")));
    }

    [Fact]
    public void TestSuppressExceptionsIsRespectedPath()
    {
        var parseContext = JsonPath.Using(
            new ConfigurationBuilder().WithJsonProvider(new NewtonsoftJsonProvider())
                .WithMappingProvider(new NewtonsoftJsonMappingProvider())
                .WithOptions(Option.SuppressExceptions, Option.AsPathList)
                .Build());
        var json = "{}";

        var result = parseContext.Parse(json).Read(JsonPath.Compile("$.missing")).AsList();
        Assert.Empty(result);
    }
}