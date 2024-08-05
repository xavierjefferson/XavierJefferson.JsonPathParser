namespace XavierJefferson.JsonPathParser.Filtering.Evaluation;

public class LessThanEqualsEvaluator : CompareEvaluator
{
    protected override List<int> CompareValues => new() { 0, -1 };

    //public bool Evaluate(ValueNode left, ValueNode right, Predicate.PredicateContext ctx)
    //{
    //    if (left.isNumberNode() && right.isNumberNode())
    //    {
    //        return left.asNumberNode().getNumber().CompareTo(right.asNumberNode().getNumber()) <= 0;
    //    }
    //    if (left.isStringNode() && right.isStringNode())
    //    {
    //        return left.asStringNode().getString().CompareTo(right.asStringNode().getString()) <= 0;
    //    }
    //    if (left.isDateTimeOffsetNode() && right.isDateTimeOffsetNode())
    //    { //workaround for issue: https://github.com/json-path/JsonPath/issues/613
    //        return left.asDateTimeOffsetNode().getDate().CompareTo(right.asDateTimeOffsetNode().getDate()) <= 0;
    //    }
    //    return false;
    //}
}