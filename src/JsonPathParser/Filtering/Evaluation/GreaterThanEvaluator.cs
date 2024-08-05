namespace XavierJefferson.JsonPathParser.Filtering.Evaluation;

public class GreaterThanEvaluator : CompareEvaluator
{
    protected override List<int> CompareValues => new() { 1 };

}