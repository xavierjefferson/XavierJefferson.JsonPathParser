using XavierJefferson.JsonPathParser.UnitTests.Extensions;
using XavierJefferson.JsonPathParser.UnitTests.TestData;

namespace XavierJefferson.JsonPathParser.UnitTests.Internal;

public class ArrayPathTokenTest : TestBase
{
    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void array_can_select_multiple_indexes(IProviderTypeTestCase testCase)
    {
        var result = JsonPath.Read(Array(testCase), "$[0,1]").AsList();

        Assert.True(result.ContainsOnly(
            GetSingletonMap("foo", "foo-val-0"),
            GetSingletonMap("foo", "foo-val-1")));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void array_can_be_sliced_to_2(IProviderTypeTestCase testCase)
    {
        var result = JsonPath.Read(Array(testCase), "$[:2]").AsListOfMap();

        Assert.True(result.ContainsOnly(
            GetSingletonMap("foo", "foo-val-0"),
            GetSingletonMap("foo", "foo-val-1")));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void array_can_be_sliced_to_2_from_tail(IProviderTypeTestCase testCase)
    {
        var result = JsonPath.Read(Array(testCase), "$[:-5]").AsListOfMap();

        Assert.True(result.ContainsOnly(
            GetSingletonMap("foo", "foo-val-0"),
            GetSingletonMap("foo", "foo-val-1")));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void array_can_be_sliced_from_2(IProviderTypeTestCase testCase)
    {
        var result = JsonPath.Read(Array(testCase), "$[5:]").AsListOfMap();

        Assert.True(result.ContainsOnly(
            GetSingletonMap("foo", "foo-val-5"),
            GetSingletonMap("foo", "foo-val-6")));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void array_can_be_sliced_from_2_from_tail(IProviderTypeTestCase testCase)
    {
        var result = JsonPath.Read(Array(testCase), "$[-2:]").AsListOfMap();

        Assert.True(result.ContainsOnly(GetSingletonMap("foo", "foo-val-5"),
            GetSingletonMap("foo", "foo-val-6")));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void array_can_be_sliced_between(IProviderTypeTestCase testCase)
    {
        var result = JsonPath.Read(Array(testCase), "$[2:4]").AsListOfMap();

        Assert.True(result.ContainsOnly(
            GetSingletonMap("foo", "foo-val-2"),
            GetSingletonMap("foo", "foo-val-3")));
    }
}