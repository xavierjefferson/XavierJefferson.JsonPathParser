using XavierJefferson.JsonPathParser.Filtering.Evaluation;
using XavierJefferson.JsonPathParser.Filtering.ValueNodes;
using XavierJefferson.JsonPathParser.Interfaces;
using XavierJefferson.JsonPathParser.Logging;

namespace XavierJefferson.JsonPathParser.Filtering;

public class RelationalExpressionNode : ExpressionNode
{
    private static readonly ILog Logger = LoggerFactory.GetLogger(typeof(RelationalExpressionNode));

    private readonly ValueNode? _left;
    private readonly RelationalOperator? _relationalOperator;
    private readonly ValueNode? _right;

    public RelationalExpressionNode(ValueNode? left, RelationalOperator? relationalOperator, ValueNode? right)
    {
        _left = left;
        _relationalOperator = relationalOperator;
        _right = right;

        Logger.Trace($"ExpressionNode {ToString()}");
    }

    public override string ToUnenclosedString()
    {
        if (_relationalOperator == RelationalOperator.Exists)
            return _left.ToString();
        return _left + " " + _relationalOperator + " " + _right;
    }

    public override string ToString()
    {
        return ToUnenclosedString();
    }


    public override bool Apply(IPredicateContext context)
    {
        var l = _left;
        var r = _right;

        if (_left is PathNode) l = _left.AsPathNode().Evaluate(context);
        if (_right is PathNode) r = _right.AsPathNode().Evaluate(context);
        var evaluator = EvaluatorFactory.CreateEvaluator(_relationalOperator);
        if (evaluator != null) return evaluator.Evaluate(l, r, context);
        return false;
    }
}