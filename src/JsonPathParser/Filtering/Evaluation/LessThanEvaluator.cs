namespace XavierJefferson.JsonPathParser.Filtering.Evaluation;

public class LessThanEvaluator : CompareEvaluator
{
    protected override List<int> CompareValues => new() { -1 };
}