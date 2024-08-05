using XavierJefferson.JsonPathParser.Filtering.ValueNodes;
using XavierJefferson.JsonPathParser.Helpers;
using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Filtering;

public class LogicalExpressionNode : ExpressionNode
{
    public LogicalOperator Operator { get; }
    protected SerializingList<ExpressionNode?> Chain = new();

    private LogicalExpressionNode(ExpressionNode left, LogicalOperator @operator, ExpressionNode? right)
    {
        Chain.Add(left);
        Chain.Add(right);
        this.Operator = @operator;
    }

    private LogicalExpressionNode(LogicalOperator @operator, ICollection<ExpressionNode> operands)
    {
        Chain.AddRange(operands);
        this.Operator = @operator;
    }

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


    public override string ToString()
    {
        string delimiter = " " + Operator.OperatorString + " ";
        return "(" + string.Join(delimiter, Chain.Cast<object>()) + ")";
    }


    public override bool Apply(IPredicateContext ctx)
    {
        if (Operator == LogicalOperator.Or)
        {
            foreach (var expression in Chain)
                if (expression.Apply(ctx))
                    return true;
            return false;
        }

        if (Operator == LogicalOperator.And)
        {
            foreach (var expression in Chain)
                if (!expression.Apply(ctx))
                    return false;
            return true;
        }

        {
            var expression = Chain[0];
            return !expression.Apply(ctx);
        }
    }
}