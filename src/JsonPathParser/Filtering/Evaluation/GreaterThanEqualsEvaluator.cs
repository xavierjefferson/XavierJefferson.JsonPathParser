namespace XavierJefferson.JsonPathParser.Filtering.Evaluation;

public class GreaterThanEqualsEvaluator : CompareEvaluator
{
    protected override List<int> CompareValues => new() { 0, 1 };
}