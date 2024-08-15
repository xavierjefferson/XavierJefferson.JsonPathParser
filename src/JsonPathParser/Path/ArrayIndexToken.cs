using XavierJefferson.JsonPathParser.PathRefs;

namespace XavierJefferson.JsonPathParser.Path;

public class ArrayIndexToken : ArrayPathToken
{
    private readonly ArrayIndexOperation _arrayIndexOperation;

    internal ArrayIndexToken(ArrayIndexOperation arrayIndexOperation)
    {
        _arrayIndexOperation = arrayIndexOperation;
    }


    public override void Evaluate(string currentPath, PathRef parent, object? model, EvaluationContextImpl context)
    {
        if (!CheckArrayModel(currentPath, model, context))
            return;
        if (_arrayIndexOperation.IsSingleIndexOperation())
            HandleArrayIndex(_arrayIndexOperation.Indexes()[0], currentPath, model, context);
        else
            foreach (var index in _arrayIndexOperation.Indexes())
                HandleArrayIndex(index, currentPath, model, context);
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