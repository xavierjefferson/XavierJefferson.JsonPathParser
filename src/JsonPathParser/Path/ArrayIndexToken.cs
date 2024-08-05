using XavierJefferson.JsonPathParser.PathRefs;

namespace XavierJefferson.JsonPathParser.Path;

public class ArrayIndexToken : ArrayPathToken
{
    private readonly ArrayIndexOperation _arrayIndexOperation;

    internal ArrayIndexToken(ArrayIndexOperation arrayIndexOperation)
    {
        this._arrayIndexOperation = arrayIndexOperation;
    }


    public override void Evaluate(string currentPath, PathRef parent, object? model, EvaluationContextImpl ctx)
    {
        if (!CheckArrayModel(currentPath, model, ctx))
            return;
        if (_arrayIndexOperation.IsSingleIndexOperation())
            HandleArrayIndex(_arrayIndexOperation.Indexes()[0], currentPath, model, ctx);
        else
            foreach (var index in _arrayIndexOperation.Indexes())
                HandleArrayIndex(index, currentPath, model, ctx);
    }


    public override string GetPathFragment()
    {
        return _arrayIndexOperation.ToString();
    }


    public override bool IsTokenDefinite()
    {
        return _arrayIndexOperation.IsSingleIndexOperation();
    }
}