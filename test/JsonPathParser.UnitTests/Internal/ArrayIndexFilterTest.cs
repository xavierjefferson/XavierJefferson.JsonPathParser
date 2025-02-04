using XavierJefferson.JsonPathParser.UnitTests.Extensions;

namespace XavierJefferson.JsonPathParser.UnitTests.Internal;

public class ArrayIndexFilterTest
{
    private static readonly string Json = "[1, 3, 5, 7, 8, 13, 20]";

    [Fact]
    public void TailDoesNotThrowWhenIndexOutOfBounds()
    {
        var result = JsonPath.Parse(Json).Read("$[-10:]").AsList();
        MyAssert.ContainsAll(result, 1d, 3d, 5d, 7d, 8d, 13d, 20d);
    }

    [Fact]
    public void HeadDoesNotThrowWhenIndexOutOfBounds()
    {
        var result = JsonPath.Parse(Json).Read("$[:10]").AsList();
        MyAssert.ContainsAll(result, 1d, 3d, 5d, 7d, 8d, 13d, 20d);
    }

    [Fact]
    public void HeadGrabsCorrect()
    {
        var result = JsonPath.Parse(Json).Read("$[:3]").AsList();
        MyAssert.ContainsAll(result, 1d, 3d, 5d);
    }


    [Fact]
    public void TailGrabsCorrect()
    {
        var result = JsonPath.Parse(Json).Read("$[-3:]").AsList();
        MyAssert.ContainsAll(result, 8d, 13d, 20d);
    }

    [Fact]
    public void HeadTailGrabsCorrect()
    {
        var result = JsonPath.Parse(Json).Read("$[0:3]").AsList();
        MyAssert.ContainsAll(result, 1d, 3d, 5d);
    }

    [Fact]
    public void CanAccessItemsFromEndWithNegativeIndex()
    {
        var result = JsonPath.Parse(Json).Read("$[-3]");
        Assert.Equal(8d, result);
    }
}