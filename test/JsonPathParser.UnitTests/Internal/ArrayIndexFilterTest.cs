using XavierJefferson.JsonPathParser.UnitTests.Extensions;

namespace XavierJefferson.JsonPathParser.UnitTests.Internal;

public class ArrayIndexFilterTest
{
    private static readonly string Json = "[1, 3, 5, 7, 8, 13, 20]";

    [Fact]
    public void tail_does_not_throw_when_index_out_of_bounds()
    {
        var result = JsonPath.Parse(Json).Read("$[-10:]").AsList();
        MyAssert.ContainsAll(result, 1d, 3d, 5d, 7d, 8d, 13d, 20d);
    }

    [Fact]
    public void head_does_not_throw_when_index_out_of_bounds()
    {
        var result = JsonPath.Parse(Json).Read("$[:10]").AsList();
        MyAssert.ContainsAll(result, 1d, 3d, 5d, 7d, 8d, 13d, 20d);
    }

    [Fact]
    public void head_grabs_correct()
    {
        var result = JsonPath.Parse(Json).Read("$[:3]").AsList();
        MyAssert.ContainsAll(result, 1d, 3d, 5d);
    }


    [Fact]
    public void tail_grabs_correct()
    {
        var result = JsonPath.Parse(Json).Read("$[-3:]").AsList();
        MyAssert.ContainsAll(result, 8d, 13d, 20d);
    }

    [Fact]
    public void head_tail_grabs_correct()
    {
        var result = JsonPath.Parse(Json).Read("$[0:3]").AsList();
        MyAssert.ContainsAll(result, 1d, 3d, 5d);
    }

    [Fact]
    public void can_access_items_from_end_with_negative_index()
    {
        var result = JsonPath.Parse(Json).Read("$[-3]");
        Assert.Equal(8d, result);
    }
}