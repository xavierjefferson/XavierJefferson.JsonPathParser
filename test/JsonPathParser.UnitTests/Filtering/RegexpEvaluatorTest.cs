using XavierJefferson.JsonPathParser.Filtering;
using XavierJefferson.JsonPathParser.Filtering.Evaluation;
using XavierJefferson.JsonPathParser.Filtering.ValueNodes;
using XavierJefferson.JsonPathParser.Interfaces;
using XavierJefferson.JsonPathParser.UnitTests.TestData;

namespace XavierJefferson.JsonPathParser.UnitTests.Filtering;

public class RegexEvaluatorTest : TestUtils
{
    [Theory]
    [ClassData(typeof(RegexTestCases))]
    public void should_evaluate_regular_expression(RegexTestCase testCase)
    {
        var testCase0 = ProviderTypeTestCases.Cases.First().Value;
        //given
        var evaluator = EvaluatorFactory.CreateEvaluator(RelationalOperator.Regex);
        ValueNode patternNode = ValueNode.CreatePatternNode(testCase.Pattern);
        var ctx = CreatePredicateContext(testCase0);

        //when
        var result = evaluator.Evaluate(patternNode, testCase.ValueNode, ctx);

        Assert.Equal(testCase.ExpectedResult, result);
    }


    private IPredicateContext CreatePredicateContext(IProviderTypeTestCase testCase)
    {
        return base.CreatePredicateContext(new Dictionary<string, object?>(), testCase);
    }
}