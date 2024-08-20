using XavierJefferson.JsonPathParser.Filtering;
using XavierJefferson.JsonPathParser.UnitTests.TestData;

namespace XavierJefferson.JsonPathParser.UnitTests;

public abstract class Issue613Base<T> : TestUtils
{
    public const string MiddleValueKey = "middleValueKey";
    public abstract T SmallValue { get; }
    public abstract T MiddleValue { get; }
    public abstract T LargeValue { get; }

    protected Dictionary<string, object?> MapMiddle =>
        new()
        {
            { MiddleValueKey, MiddleValue }
        };

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void issue_613_eq_ne_test(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, MiddleValueKey).Eq(MiddleValue))
            .Apply(CreatePredicateContext(MapMiddle, testCase)));
        Assert.True(
            Filter.Create(Criteria.Where(jsonProvider, MiddleValueKey).Ne(LargeValue))
                .Apply(CreatePredicateContext(MapMiddle, testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void issue_613_lt_lte_test(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.True(
            Filter.Create(Criteria.Where(jsonProvider, MiddleValueKey).Lt(LargeValue))
                .Apply(CreatePredicateContext(MapMiddle, testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, MiddleValueKey).Lte(SmallValue))
            .Apply(CreatePredicateContext(MapMiddle, testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void issue_613_gt_gte_test(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, MiddleValueKey).Gt(LargeValue))
            .Apply(CreatePredicateContext(MapMiddle, testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, MiddleValueKey).Gte(SmallValue))
            .Apply(CreatePredicateContext(MapMiddle, testCase)));
    }
}