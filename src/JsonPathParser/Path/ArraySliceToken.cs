using XavierJefferson.JsonPathParser.Enums;
using XavierJefferson.JsonPathParser.Interfaces;
using XavierJefferson.JsonPathParser.Logging;
using XavierJefferson.JsonPathParser.PathRefs;

namespace XavierJefferson.JsonPathParser.Path;

public class ArraySliceToken : ArrayPathToken
{
    private static readonly ILog Logger = LoggerFactory.GetLogger(typeof(ArraySliceToken));

    private readonly ArraySliceOperation _operation;

    internal ArraySliceToken(ArraySliceOperation operation)
    {
        _operation = operation;
    }


    public override void Evaluate(string currentPath, PathRef parent, object? model, EvaluationContextImpl ctx)
    {
        if (!CheckArrayModel(currentPath, model, ctx))
            return;
        switch (_operation.Operation())
        {
            case ArraySliceOperationEnum.SliceFrom:
                SliceFrom(currentPath, parent, model, ctx);
                break;
            case ArraySliceOperationEnum.SliceBetween:
                SliceBetween(currentPath, parent, model, ctx);
                break;
            case ArraySliceOperationEnum.SliceTo:
                SliceTo(currentPath, parent, model, ctx);
                break;
        }
    }

    private void SliceFrom(string currentPath, PathRef parent, object? model, EvaluationContextImpl ctx)
    {
        var length = ctx.JsonProvider.Length(model);
        var from = _operation.From();
        if (from < 0)
            //calculate slice start from array length
            from = length + from;
        from = Math.Max(0, from.Value);

        Logger.Debug(
            $"Slice from index on array with length: {length}. From index: {from} to: {length - 1}. Input: {ToString()}");

        if (length == 0 || from >= length) return;
        for (var i = from.Value; i < length; i++) HandleArrayIndex(i, currentPath, model, ctx);
    }

    private void SliceBetween(string currentPath, PathRef parent, object? model, EvaluationContextImpl ctx)
    {
        var length = ctx.JsonProvider.Length(model);
        var from = _operation.From();
        var to = _operation.To();

        to = Math.Min(length, to.Value);

        if (from >= to || length == 0) return;

        Logger.DebugFormat("Slice between indexes on array with length: {0}. From index: {1} to: {2}. Input: {3}",
            length, from, to, ToString());

        for (var i = from.Value; i < to; i++) HandleArrayIndex(i, currentPath, model, ctx);
    }

    private void SliceTo(string currentPath, PathRef parent, object? model, EvaluationContextImpl ctx)
    {
        var length = ctx.JsonProvider.Length(model);
        if (length == 0) return;
        var to = _operation.To();
        if (to < 0)
            //calculate slice end from array length
            to = length + to;
        to = Math.Min(length, to.Value);

        Logger.DebugFormat("Slice to index on array with length: {0}. From index: 0 to: {1}. Input: {2}", length, to,
            ToString());

        for (var i = 0; i < to; i++) HandleArrayIndex(i, currentPath, model, ctx);
    }


    public override string GetPathFragment()
    {
        return _operation.ToString();
    }


    public override bool IsTokenDefinite()
    {
        return false;
    }
}