using XavierJefferson.JsonPathParser.Enums;
using XavierJefferson.JsonPathParser.Interfaces;
using XavierJefferson.JsonPathParser.UnitTests.Extensions;
using XavierJefferson.JsonPathParser.UnitTests.TestData;

namespace XavierJefferson.JsonPathParser.UnitTests;

public class EvaluationListenerTest : TestUtils
{
    [Fact]
    public void an_evaluation_listener_can_abort_after_one_result_using_fluent_api()
    {
        var title = JsonPath.Parse(JsonTestData.JsonDocument).WithListeners(_ => EvaluationContinuationEnum.Abort)
            .Read("$..title", TypeConstants.ListType).AsList();
        MyAssert.ContainsExactly(title, "Sayings of the Century");
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void an_evaluation_listener_can_abort_after_one_result_using_configuration(IProviderTypeTestCase testCase)
    {
        var configuration = testCase.Configuration.SetEvaluationCallbacks(_ => EvaluationContinuationEnum.Abort);

        var title = JsonPath.Using(configuration).Parse(JsonTestData.JsonDocument).Read("$..title", TypeConstants.ListType)
            .AsList();
        MyAssert.ContainsExactly(title, "Sayings of the Century");
    }

    [Fact]
    public void an_evaluation_lister_can_continue()
    {
        IList<int> idxs = new List<int>();

        EvaluationCallback firstResultListener = found =>
        {
            idxs.Add(found.Index);
            return EvaluationContinuationEnum.Continue;
        };

        var title = JsonPath.Parse(JsonTestData.JsonDocument).WithListeners(firstResultListener)
            .Read("$..title", TypeConstants.ListType).AsList();
        MyAssert.ContainsExactly(title, "Sayings of the Century", "Sword of Honour", "Moby Dick",
            "The Lord of the Rings");
        MyAssert.ContainsExactly(idxs, 0, 1, 2, 3);
    }


    [Fact]
    public void evaluation_results_can_be_limited()
    {
        var res = JsonPath.Parse(JsonTestData.JsonDocument).Limit(1).Read("$..title", TypeConstants.ListType).AsList();
        MyAssert.ContainsExactly(res, "Sayings of the Century");

        res = JsonPath.Parse(JsonTestData.JsonDocument).Limit(2).Read("$..title", TypeConstants.ListType).AsList();
        MyAssert.ContainsExactly(res, "Sayings of the Century", "Sword of Honour");
    }

    [Fact]
    public void multiple_evaluation_listeners_can_be_added()
    {
        var idxs1 = new List<int>();
        var idxs2 = new List<int>();


        var title = JsonPath.Parse(JsonTestData.JsonDocument).WithListeners(found =>
            {
                idxs1.Add(found.Index);
                return EvaluationContinuationEnum.Continue;
            }, found =>
            {
                idxs2.Add(found.Index);
                return EvaluationContinuationEnum.Continue;
            })
            .Read("$..title", TypeConstants.ListType).AsList();
        MyAssert.ContainsExactly(title, "Sayings of the Century", "Sword of Honour", "Moby Dick",
            "The Lord of the Rings");
        MyAssert.ContainsExactly(idxs1, 0, 1, 2, 3);
        MyAssert.ContainsExactly(idxs2, 0, 1, 2, 3);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void evaluation_listeners_can_be_cleared(IProviderTypeTestCase testCase)
    {
        var configuration1 = testCase.Configuration.SetEvaluationCallbacks(_ => EvaluationContinuationEnum.Continue);
        var configuration2 = configuration1.SetEvaluationCallbacks();

        Assert.Equal(1, configuration1.EvaluationCallbacks.Count());
        Assert.Equal(0, configuration2.EvaluationCallbacks.Count());
    }
}