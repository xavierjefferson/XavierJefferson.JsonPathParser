using System.Diagnostics;
using XavierJefferson.JsonPathParser.Exceptions;
using XavierJefferson.JsonPathParser.Function;
using XavierJefferson.JsonPathParser.Helpers;
using XavierJefferson.JsonPathParser.Interfaces;
using XavierJefferson.JsonPathParser.PathRefs;

namespace XavierJefferson.JsonPathParser.Path;

public abstract class PathToken
{
    private bool? _definite;
    private PathToken? _next;

    private PathToken? _prev;
    private int _upstreamArrayIndex = -1;
    private bool? _upstreamDefinite;


    public void SetUpstreamArrayIndex(int idx)
    {
        _upstreamArrayIndex = idx;
    }

    public PathToken AppendTailToken(PathToken next)
    {
        _next = next;
        _next._prev = this;
        return next;
    }

    public void HandleObjectProperty(string currentPath, object? model, EvaluationContextImpl context,
        SerializingList<string> properties)
    {
        if (properties.Count() == 1)
        {
            var property = properties[0];
            var evalPath = $"{currentPath}['{property}']";
            var propertyVal = ReadObjectProperty(property, model, context);
            if (propertyVal == IJsonProvider.Undefined)
            {
                // Conditions below heavily depend on current token type (and its logic) and are not "universal",
                // so this code is quite dangerous (I'd rather rewrite it & move to PropertyPathToken and implemented
                // WildcardPathToken as a dynamic multi prop case of PropertyPathToken).
                // Better safe than sorry.
                Debug.Assert(this is PropertyPathToken, "only PropertyPathToken is supported");

                if (IsLeaf())
                {
                    if (context.Options.Contains(Option.DefaultPathLeafToNull))
                    {
                        propertyVal = null;
                    }
                    else
                    {
                        if (context.Options.Contains(Option.SuppressExceptions) ||
                            !context.Options.Contains(Option.RequireProperties))
                            return;
                        throw new PathNotFoundException($"No results for path: {evalPath}");
                    }
                }
                else
                {
                    if ((!(IsUpstreamDefinite() && IsTokenDefinite()) &&
                         !context.Options.Contains(Option.RequireProperties)) ||
                        context.Options.Contains(Option.SuppressExceptions))
                        // If there is some indefiniteness in the path and properties are not required - we'll ignore
                        // absent property. And also in case of exception suppression - so that other path evaluation
                        // branches could be examined.
                        return;
                    throw new PathNotFoundException($"Missing property in path {evalPath}");
                }
            }

            var pathRef = context.ForUpdate() ? PathRef.Create(model, property) : PathRef.NoOp;
            if (IsLeaf())
            {
                var idx = $"[{_upstreamArrayIndex}]";
                if (_upstreamArrayIndex == -1 || context.GetRoot().GetTail().Prev().GetPathFragment().Equals(idx))
                    context.AddResult(evalPath, pathRef, propertyVal);
            }
            else
            {
                Next().Evaluate(evalPath, pathRef, propertyVal, context);
            }
        }
        else
        {
            var evalPath = currentPath + "[" + StringHelper.Join(", ", "'", properties) + "]";

            Debug.Assert(IsLeaf(), "non-leaf multi props handled elsewhere");

            var merged = context.JsonProvider.CreateMap();
            foreach (var property in properties)
            {
                object? propertyVal;
                if (HasProperty(property, model, context))
                {
                    propertyVal = ReadObjectProperty(property, model, context);
                    if (propertyVal == IJsonProvider.Undefined)
                    {
                        if (context.Options.Contains(Option.DefaultPathLeafToNull))
                            propertyVal = null;
                        else
                            continue;
                    }
                }
                else
                {
                    if (context.Options.Contains(Option.DefaultPathLeafToNull))
                        propertyVal = null;
                    else if (context.Options.Contains(Option.RequireProperties))
                        throw new PathNotFoundException($"Missing property in path {evalPath}");
                    else
                        continue;
                }

                context.JsonProvider.SetProperty(merged, property, propertyVal);
            }

            var pathRef = context.ForUpdate() ? PathRef.Create(model, properties) : PathRef.NoOp;
            context.AddResult(evalPath, pathRef, merged);
        }
    }

    private static bool HasProperty(string property, object? model, EvaluationContextImpl context)
    {
        return context.JsonProvider.GetPropertyKeys(model).Contains(property);
    }

    private static object? ReadObjectProperty(string property, object? model, EvaluationContextImpl context)
    {
        return context.JsonProvider.GetMapValue(model, property);
    }


    protected void HandleArrayIndex(int index, string currentPath, object? model, EvaluationContextImpl context)
    {
        var evalPath = $"{currentPath}[{index}]";
        var pathRef = context.ForUpdate() ? PathRef.Create(model, index) : PathRef.NoOp;
        var effectiveIndex = index < 0 ? context.JsonProvider.Length(model) + index : index;
        try
        {
            var evalHit = context.JsonProvider.GetArrayIndex(model, effectiveIndex);
            if (IsLeaf())
                context.AddResult(evalPath, pathRef, evalHit);
            else
                Next().Evaluate(evalPath, pathRef, evalHit, context);
        }
        catch (ArgumentOutOfRangeException)
        {
            //do nothing
        }
    }

    private PathToken Prev()
    {
        return _prev;
    }

    public PathToken? Next()
    {
        if (IsLeaf()) throw new InvalidOperationException("Current path token is a leaf");
        return _next;
    }

    public bool IsLeaf()
    {
        return _next == null;
    }

    private bool IsRoot()
    {
        return _prev == null;
    }

    public bool IsUpstreamDefinite()
    {
        if (_upstreamDefinite == null)
        {
            var px = _prev?.IsUpstreamDefinite();
            _upstreamDefinite = IsRoot() || (_prev.IsTokenDefinite() && (px ?? false));
        }

        return _upstreamDefinite ?? false;
    }

    public virtual int GetTokenCount()
    {
        var cnt = 1;
        var token = this;

        while (!token.IsLeaf())
        {
            token = token.Next();
            cnt++;
        }

        return cnt;
    }

    public bool IsPathDefinite()
    {
        if (_definite != null) return _definite.Value;
        var isDefinite = IsTokenDefinite();
        if (isDefinite && !IsLeaf()) isDefinite = _next.IsPathDefinite();
        _definite = isDefinite;
        return isDefinite;
    }


    public override string ToString()
    {
        if (IsLeaf())
            return GetPathFragment();
        return GetPathFragment() + Next();
    }

    public override int GetHashCode()
    {
        return ToString().GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        return base.Equals(obj);
    }

    public void Invoke(IPathFunction pathFunction, string currentPath, PathRef parent, object? model,
        EvaluationContextImpl context)
    {
        context.AddResult(currentPath, parent, pathFunction.Invoke(currentPath, parent, model, context, null));
    }

    public abstract void Evaluate(string currentPath, PathRef parent, object? model, EvaluationContextImpl context);

    public abstract bool IsTokenDefinite();

    public abstract string GetPathFragment();

    public void SetNext(PathToken? next)
    {
        _next = next;
    }

    public PathToken? GetNext()
    {
        return _next;
    }
}