using XavierJefferson.JsonPathParser.UnitTests.TestData;

namespace XavierJefferson.JsonPathParser.UnitTests;

//test for issue: https://github.com/json-path/JsonPath/issues/613
public class Issue613 : TestUtils
{
    private static readonly DateTimeOffset OfdtSmall = new(1999, 2, 1, 1, 1, 1, 1, DateTimeOffset.Now.Offset);
    private static readonly DateTimeOffset OfdtMiddle = new(2000, 2, 1, 1, 1, 1, 1, DateTimeOffset.Now.Offset);
    private static readonly DateTimeOffset OfdtBig = new(2001, 3, 1, 1, 1, 1, 1, TimeSpan.FromHours(14));

    private static readonly Dictionary<string, object?> MapMiddle = new()
    {
        { "time", OfdtMiddle }
    };

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void issue_613_eq_ne_test(IProviderTypeTestCase testCase)
    {
        Assert.True(Filter.Create(Criteria.Where("time").Eq(OfdtMiddle))
            .Apply(CreatePredicateContext(MapMiddle, testCase)));
        Assert.True(
            Filter.Create(Criteria.Where("time").Ne(OfdtBig)).Apply(CreatePredicateContext(MapMiddle, testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void issue_613_lt_lte_test(IProviderTypeTestCase testCase)
    {
        Assert.True(
            Filter.Create(Criteria.Where("time").Lt(OfdtBig)).Apply(CreatePredicateContext(MapMiddle, testCase)));
        Assert.False(Filter.Create(Criteria.Where("time").Lte(OfdtSmall))
            .Apply(CreatePredicateContext(MapMiddle, testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void issue_613_gt_gte_test(IProviderTypeTestCase testCase)
    {
        Assert.False(Filter.Create(Criteria.Where("time").Gt(OfdtBig))
            .Apply(CreatePredicateContext(MapMiddle, testCase)));
        Assert.True(Filter.Create(Criteria.Where("time").Gte(OfdtSmall))
            .Apply(CreatePredicateContext(MapMiddle, testCase)));
    }
}