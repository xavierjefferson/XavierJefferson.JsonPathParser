using XavierJefferson.JsonPathParser.Enums;
using XavierJefferson.JsonPathParser.Function;
using XavierJefferson.JsonPathParser.Function.LateBinding;
using XavierJefferson.JsonPathParser.PathRefs;

namespace XavierJefferson.JsonPathParser.Path;

/// <summary>
///     Token representing a Function call to one of the functions produced via the FunctionFactory
/// </summary>
public class FunctionPathToken : PathToken
{
    private readonly string? _pathFragment;

    public SerializingList<Parameter>? Parameters { get; set; }

    public string? FunctionName { get; }

    public FunctionPathToken(string pathFragment, SerializingList<Parameter>? parameters)
    {
        _pathFragment = $"{pathFragment}{(parameters != null && parameters.Count() > 0 ? "(...)" : "()")}";
        if (null != pathFragment)
        {
            FunctionName = pathFragment;
            Parameters = parameters;
        }
        else
        {
            FunctionName = null;
            Parameters = null;
        }
    }


    public override void Evaluate(string currentPath, PathRef parent, object? model, EvaluationContextImpl context)
    {
        var pathFunction = PathFunctionFactory.NewFunction(FunctionName);
        EvaluateParameters(currentPath, parent, model, context);
        var result = pathFunction.Invoke(currentPath, parent, model, context, Parameters);
        context.AddResult($"{currentPath}.{FunctionName}", parent, result);
        CleanWildcardPathToken();
        if (!IsLeaf()) Next()?.Evaluate(currentPath, parent, result, context);
    }

    private void CleanWildcardPathToken()
    {
        if (null != Parameters && Parameters.Count() > 0)
        {
            var path = Parameters[0].Path;
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

    private void EvaluateParameters(string currentPath, PathRef parent, object? model, EvaluationContextImpl context)
    {
        if (null != Parameters)
            foreach (var param in Parameters)
                switch (param.ParameterType)
                {
                    case ParameterTypeEnum.Path:
                        var pathLateBindingValue =
                            new PathLateBindingValue(param.Path, context.RootDocument, context.Configuration);
                        if (!param.Evaluated || !pathLateBindingValue.Equals(param.LateBinding))
                        {
                            param.LateBinding = pathLateBindingValue;
                            param.Evaluated = true;
                        }

                        break;
                    case ParameterTypeEnum.Json:
                        if (!param.Evaluated)
                        {
                            param.LateBinding = new JsonLateBindingValue(context.Configuration.JsonProvider, param);
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
     
}