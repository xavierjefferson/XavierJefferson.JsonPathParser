using System.Text.Json;

namespace XavierJefferson.JsonPathParser.UnitTests;

public class MapperTest : TestUtils
{
    [Fact]
    public void an_Integer_can_be_converted_to_a_Long()
    {
        Assert.Equal(1L, JsonPath.Parse("{\"val\": 1}").Read<long>("val"));
    }

    [Fact]
    public void an_String_can_be_converted_to_a_Long()
    {
        Assert.Equal(1L, JsonPath.Parse("{\"val\": 1}").Read<long>("val"));
    }

    [Fact]
    public void an_Integer_can_be_converted_to_a_String()
    {
        Assert.Equal("1", JsonPath.Parse("{\"val\": 1}").Read<string>("val"));
    }

    [Fact]
    public void an_Integer_can_be_converted_to_a_Double()
    {
        Assert.Equal(1D, JsonPath.Parse("{\"val\": 1}").Read<double>("val"));
    }

    [Fact]
    public void a_BigDecimal_can_be_converted_to_a_Long()
    {
        Assert.Equal(2L, JsonPath.Parse("{\"val\": 1.5}").Read<long>("val"));
    }

    [Fact]
    public void a_Long_can_be_converted_to_a_Date()
    {
        var now = DateTime.Now;
        Assert.Equal(now, JsonPath.Parse(JsonSerializer.Serialize(new { val = now })).Read<DateTime>("val"));
    }

    [Fact]
    public void a_String_can_be_converted_to_a_BigInteger()
    {
        Assert.Equal(1, JsonPath.Parse("{\"val\": \"1\"}").Read<long>("val"));
    }

    [Fact]
    public void a_String_can_be_converted_to_a_BigDecimal()
    {
        Assert.Equal(1.5m, JsonPath.Parse("{\"val\": \"1.5\"}").Read<decimal>("val"));
    }

    [Fact]
    public void a_Boolean_can_be_converted_to_a_primitive_boolean()
    {
        Assert.True(JsonPath.Parse("{\"val\": true}").Read<bool>("val"));
        Assert.False(JsonPath.Parse("{\"val\": false}").Read<bool>("val"));
    }
}