using XavierJefferson.JsonPathParser.Function;
using XavierJefferson.JsonPathParser.Function.LateBinding;
using XavierJefferson.JsonPathParser.PathRefs;

namespace XavierJefferson.JsonPathParser.Path;

/// <summary>
///     Token representing a Function call to one of the functions produced via the FunctionFactory
/// </summary>
public class FunctionPathToken : PathToken
{
    private readonly string? _functionName;
    private readonly string? _pathFragment;
    private SerializingList<Parameter>? _functionParams;

    public FunctionPathToken(string pathFragment, SerializingList<Parameter>? parameters)
    {
        this._pathFragment = pathFragment + (parameters != null && parameters.Count() > 0 ? "(...)" : "()");
        if (null != pathFragment)
        {
            _functionName = pathFragment;
            _functionParams = parameters;
        }
        else
        {
            _functionName = null;
            _functionParams = null;
        }
    }


    public override void Evaluate(string currentPath, PathRef parent, object? model, EvaluationContextImpl ctx)
    {
        var pathFunction = PathFunctionFactory.NewFunction(_functionName);
        EvaluateParameters(currentPath, parent, model, ctx);
        var result = pathFunction.Invoke(currentPath, parent, model, ctx, _functionParams);
        ctx.AddResult(currentPath + $".{_functionName}", parent, result);
        CleanWildcardPathToken();
        if (!IsLeaf()) Next()?.Evaluate(currentPath, parent, result, ctx);
    }

    private void CleanWildcardPathToken()
    {
        if (null != _functionParams && _functionParams.Count() > 0)
        {
            var path = _functionParams[0].Path;
            if (null != path && !path.IsFunctionPath() && path is CompiledPath)
            {
                var root = ((CompiledPath)path).GetRoot();
                var tail = root?.GetNext();
                while (null != tail && null != tail.GetNext())
                {
                    if (tail.GetNext() is WildcardPathToken)
                    {
                        tail.SetNext(tail.GetNext().GetNext());
                        break;
                    }

                    tail = tail.GetNext();
                }
            }
        }
    }

    private void EvaluateParameters(string currentPath, PathRef parent, object? model, EvaluationContextImpl ctx)
    {
        if (null != _functionParams)
            foreach (var param in _functionParams)
                switch (param.ParameterType)
                {
                    case ParamType.Path:
                        var pathLateBindingValue =
                            new PathLateBindingValue(param.Path, ctx.RootDocument, ctx.Configuration);
                        if (!param.Evaluated || !pathLateBindingValue.Equals(param.LateBinding))
                        {
                            param.LateBinding = pathLateBindingValue;
                            param.Evaluated = true;
                        }

                        break;
                    case ParamType.Json:
                        if (!param.Evaluated)
                        {
                            param.LateBinding = new JsonLateBindingValue(ctx.Configuration.JsonProvider, param);
                            param.Evaluated = true;
                        }

                        break;
                }
    }

    /// <summary>
    ///     Return the actual value by indicating true. If this return was false then we'd return the value in an array which
    ///     isn't what is desired - true indicates the raw value is returned.
    /// </summary>
    /// <returns> true if token is definite</returns>
    public override bool IsTokenDefinite()
    {
        return true;
    }


    public override string GetPathFragment()
    {
        return $".{_pathFragment}";
    }


    public void SetParameters(SerializingList<Parameter>? parameters)
    {
        _functionParams = parameters;
    }

    public SerializingList<Parameter>? GetParameters()
    {
        return _functionParams;
    }

    public string? GetFunctionName()
    {
        return _functionName;
    }
}