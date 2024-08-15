namespace XavierJefferson.JsonPathParser.Filtering.Evaluation;

public class LessThanEqualsEvaluator : CompareEvaluator
{
    protected override List<int> CompareValues => new() { 0, -1 };

}