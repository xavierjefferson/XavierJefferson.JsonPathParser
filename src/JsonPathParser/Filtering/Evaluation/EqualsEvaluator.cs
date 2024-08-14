namespace XavierJefferson.JsonPathParser.Filtering.Evaluation;

public class EqualsEvaluator : CompareEvaluator
{
    protected override List<int> CompareValues => new() { 0 };
}