using XavierJefferson.JsonPathParser.Filtering.ValueNodes;
using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Filtering;

public class LogicalExpressionNode : ExpressionNode
{
    protected List<ExpressionNode?> Chain = new();

    private LogicalExpressionNode(ExpressionNode left, LogicalOperator @operator, ExpressionNode? right)
    {
        Chain.Add(left);
        Chain.Add(right);
        Operator = @operator;
    }

    private LogicalExpressionNode(LogicalOperator @operator, ICollection<ExpressionNode> operands)
    {
        Chain.AddRange(operands);
        Operator = @operator;
    }

    public LogicalOperator Operator { get; }

    public static ExpressionNode CreateLogicalNot(ExpressionNode op)
    {
        return new LogicalExpressionNode(op, LogicalOperator.Not, null);
    }

    public static LogicalExpressionNode CreateLogicalOr(ExpressionNode left, ExpressionNode right)
    {
        return new LogicalExpressionNode(left, LogicalOperator.Or, right);
    }

    public static LogicalExpressionNode CreateLogicalOr(ICollection<ExpressionNode> operands)
    {
        return new LogicalExpressionNode(LogicalOperator.Or, operands);
    }

    public static LogicalExpressionNode CreateLogicalAnd(ExpressionNode left, ExpressionNode right)
    {
        return new LogicalExpressionNode(left, LogicalOperator.And, right);
    }

    public static LogicalExpressionNode CreateLogicalAnd(ICollection<ExpressionNode> operands)
    {
        return new LogicalExpressionNode(LogicalOperator.And, operands);
    }

    public LogicalExpressionNode And(LogicalExpressionNode other)
    {
        return CreateLogicalAnd(this, other);
    }

    public LogicalExpressionNode Or(LogicalExpressionNode other)
    {
        return CreateLogicalOr(this, other);
    }

    public LogicalOperator GetOperator()
    {
        return Operator;
    }

    public LogicalExpressionNode Append(ExpressionNode expressionNode)
    {
        Chain.Insert(0, expressionNode);
        return this;
    }

    public override string ToUnenclosedString()
    {
        var delimiter = " " + Operator.OperatorString + " ";
        return string.Join(delimiter, Chain);
    }

    public override string ToString()
    {
        return "(" + ToUnenclosedString() + ")";
    }


    public override bool Apply(IPredicateContext context)
    {
        if (Operator == LogicalOperator.Or) return Chain.Any(expression => expression.Apply(context));

        if (Operator == LogicalOperator.And) return Chain.All(expression => expression.Apply(context));


        var expression = Chain[0];
        return !expression.Apply(context);
    }
}