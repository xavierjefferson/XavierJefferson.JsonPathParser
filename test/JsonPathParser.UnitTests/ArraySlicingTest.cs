using XavierJefferson.JsonPathParser.UnitTests.Extensions;

namespace XavierJefferson.JsonPathParser.UnitTests;

/**
 * If you have a list
 * nums = [1, 3, 5, 7, 8, 13, 20]
 * then it is possible to slice by JsonPath.Using a notation similar to element retrieval:
 * <p>
 *     nums[3]   #equals 7, no slicing
 *     nums[:3]  #equals [1, 3, 5], from index 0 (inclusive) until index 3 (exclusive)
 *     nums[1:5] #equals [3, 5, 7, 8]
 *     nums[-3:] #equals [8, 13, 20]
 *     nums[3:] #equals [8, 13, 20]
 *     <p>
 *         Note that Python allows negative list indices. The index -1 represents the last element, -2 the penultimate
 *         element, etc.
 *         Python also allows a step property by appending an extra colon and a value. For example:
 *         <p>
 *             nums[3::]  #equals [7, 8, 13, 20], same as nums[3:]
 *             nums[::3]  #equals [1, 7, 20] (starting at index 0 and getting every third element)
 *             nums[1:5:2] #equals [3, 7] (from index 1 until index 5 and getting every second element)
 */
public class ArraySlicingTest
{
    public static string JsonArray = "[1, 3, 5, 7, 8, 13, 20]";

    [Fact]
    public void get_by_position()
    {
        var result = JsonPath.Read(JsonArray, "$[3]");
        Assert.Equal(7d, result);
    }

    [Fact]
    public void get_from_index()
    {
        var result = JsonPath.Read(JsonArray, "$[:3]").AsList();
        MyAssert.ContainsAll(result, 1d, 3d, 5d);
    }

    [Fact]
    public void get_between_index()
    {
        var result = JsonPath.Read(JsonArray, "$[1:5]").AsList();
        MyAssert.ContainsAll(result, 3d, 5d, 7d, 8d);
    }


    [Fact]
    public void get_between_index_2()
    {
        var result = JsonPath.Read(JsonArray, "$[0:1]").AsList();
        MyAssert.ContainsAll(result, 1d);
    }

    [Fact]
    public void get_between_index_3()
    {
        var result = JsonPath.Read(JsonArray, "$[0:2]").AsList();
        MyAssert.ContainsAll(result, 1d, 3d);
    }

    [Fact]
    public void get_between_index_out_of_bounds()
    {
        var result = JsonPath.Read(JsonArray, "$[1:15]").AsList();
        MyAssert.ContainsAll(result, 3d, 5d, 7d, 8d, 13d, 20d);
    }

    [Fact]
    public void get_from_tail_index()
    {
        var result = JsonPath.Read(JsonArray, "$[-3:]").AsList();
        MyAssert.ContainsAll(result, 8d, 13d, 20d);
    }

    [Fact]
    public void get_from_tail()
    {
        var result = JsonPath.Read(JsonArray, "$[3:]").AsList();
        MyAssert.ContainsAll(result, 7d, 8d, 13d, 20d);
    }

    [Fact]
    public void get_indexes()
    {
        var result = JsonPath.Read(JsonArray, "$[0,1,2]").AsList();
        MyAssert.ContainsAll(result, 1d, 3d, 5d);
    }
}