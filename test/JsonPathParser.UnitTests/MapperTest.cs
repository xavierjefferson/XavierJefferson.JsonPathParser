using System.Text.Json;

namespace XavierJefferson.JsonPathParser.UnitTests;

public class MapperTest : TestUtils
{
    [Fact]
    public void AnIntegerCanBeConvertedToALong()
    {
        Assert.Equal(1L, JsonPath.Parse("{\"val\": 1}").Read<long>("val"));
    }

    [Fact]
    public void AnStringCanBeConvertedToALong()
    {
        Assert.Equal(1L, JsonPath.Parse("{\"val\": 1}").Read<long>("val"));
    }

    [Fact]
    public void AnIntegerCanBeConvertedToAString()
    {
        Assert.Equal("1", JsonPath.Parse("{\"val\": 1}").Read<string>("val"));
    }

    [Fact]
    public void AnIntegerCanBeConvertedToADouble()
    {
        Assert.Equal(1D, JsonPath.Parse("{\"val\": 1}").Read<double>("val"));
    }

    [Fact]
    public void ABigdecimalCanBeConvertedToALong()
    {
        Assert.Equal(2L, JsonPath.Parse("{\"val\": 1.5}").Read<long>("val"));
    }

    [Fact]
    public void ALongCanBeConvertedToADate()
    {
        var now = DateTime.Now;
        Assert.Equal(now, JsonPath.Parse(JsonSerializer.Serialize(new { val = now })).Read<DateTime>("val"));
    }

    [Fact]
    public void AStringCanBeConvertedToABiginteger()
    {
        Assert.Equal(1, JsonPath.Parse("{\"val\": \"1\"}").Read<long>("val"));
    }

    [Fact]
    public void AStringCanBeConvertedToABigdecimal()
    {
        Assert.Equal(1.5m, JsonPath.Parse("{\"val\": \"1.5\"}").Read<decimal>("val"));
    }

    [Fact]
    public void ABooleanCanBeConvertedToAPrimitiveBoolean()
    {
        Assert.True(JsonPath.Parse("{\"val\": true}").Read<bool>("val"));
        Assert.False(JsonPath.Parse("{\"val\": false}").Read<bool>("val"));
    }
}