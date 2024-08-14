using System.Diagnostics;
using XavierJefferson.JsonPathParser.Exceptions;
using XavierJefferson.JsonPathParser.Function;
using XavierJefferson.JsonPathParser.Helpers;
using XavierJefferson.JsonPathParser.Interfaces;
using XavierJefferson.JsonPathParser.PathRefs;

namespace XavierJefferson.JsonPathParser.Path;

public abstract class PathToken
{
    private PathToken? _next;

    private PathToken? _prev;
    private bool? _definite;
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

    public void HandleObjectProperty(string currentPath, object? model, EvaluationContextImpl ctx,
        SerializingList<string> properties)
    {
        if (properties.Count() == 1)
        {
            var property = properties[0];
            var evalPath = $"{currentPath}['{property}']";
            var propertyVal = ReadObjectProperty(property, model, ctx);
            if (propertyVal == IJsonProvider.Undefined)
            {
                // Conditions below heavily depend on current token type (and its logic) and are not "universal",
                // so this code is quite dangerous (I'd rather rewrite it & move to PropertyPathToken and implemented
                // WildcardPathToken as a dynamic multi prop case of PropertyPathToken).
                // Better safe than sorry.
                Debug.Assert(this is PropertyPathToken, "only PropertyPathToken is supported");

                if (IsLeaf())
                {
                    if (ctx.Options.Contains(Option.DefaultPathLeafToNull))
                    {
                        propertyVal = null;
                    }
                    else
                    {
                        if (ctx.Options.Contains(Option.SuppressExceptions) ||
                            !ctx.Options.Contains(Option.RequireProperties))
                            return;
                        throw new PathNotFoundException("No results for path: " + evalPath);
                    }
                }
                else
                {
                    if ((!(IsUpstreamDefinite() && IsTokenDefinite()) &&
                         !ctx.Options.Contains(Option.RequireProperties)) ||
                        ctx.Options.Contains(Option.SuppressExceptions))
                        // If there is some indefiniteness in the path and properties are not required - we'll ignore
                        // absent property. And also in case of exception suppression - so that other path evaluation
                        // branches could be examined.
                        return;
                    throw new PathNotFoundException("Missing property in path " + evalPath);
                }
            }

            var pathRef = ctx.ForUpdate() ? PathRef.Create(model, property) : PathRef.NoOp;
            if (IsLeaf())
            {
                var idx = $"[{_upstreamArrayIndex}]";
                if (_upstreamArrayIndex == -1 || ctx.GetRoot().GetTail().Prev().GetPathFragment().Equals(idx))
                    ctx.AddResult(evalPath, pathRef, propertyVal);
            }
            else
            {
                Next().Evaluate(evalPath, pathRef, propertyVal, ctx);
            }
        }
        else
        {
            var evalPath = currentPath + "[" + StringHelper.Join(", ", "'", properties) + "]";

            Debug.Assert(IsLeaf(), "non-leaf multi props handled elsewhere");

            var merged = ctx.JsonProvider.CreateMap();
            foreach (var property in properties)
            {
                object? propertyVal;
                if (HasProperty(property, model, ctx))
                {
                    propertyVal = ReadObjectProperty(property, model, ctx);
                    if (propertyVal == IJsonProvider.Undefined)
                    {
                        if (ctx.Options.Contains(Option.DefaultPathLeafToNull))
                            propertyVal = null;
                        else
                            continue;
                    }
                }
                else
                {
                    if (ctx.Options.Contains(Option.DefaultPathLeafToNull))
                        propertyVal = null;
                    else if (ctx.Options.Contains(Option.RequireProperties))
                        throw new PathNotFoundException("Missing property in path " + evalPath);
                    else
                        continue;
                }

                ctx.JsonProvider.SetProperty(merged, property, propertyVal);
            }

            var pathRef = ctx.ForUpdate() ? PathRef.Create(model, properties) : PathRef.NoOp;
            ctx.AddResult(evalPath, pathRef, merged);
        }
    }

    private static bool HasProperty(string property, object? model, EvaluationContextImpl ctx)
    {
        return ctx.JsonProvider.GetPropertyKeys(model).Contains(property);
    }

    private static object? ReadObjectProperty(string property, object? model, EvaluationContextImpl ctx)
    {
        return ctx.JsonProvider.GetMapValue(model, property);
    }


    protected void HandleArrayIndex(int index, string currentPath, object? model, EvaluationContextImpl ctx)
    {
        var evalPath = $"{currentPath}[{index}]";
        var pathRef = ctx.ForUpdate() ? PathRef.Create(model, index) : PathRef.NoOp;
        var effectiveIndex = index < 0 ? ctx.JsonProvider.Length(model) + index : index;
        try
        {
            var evalHit = ctx.JsonProvider.GetArrayIndex(model, effectiveIndex);
            if (IsLeaf())
                ctx.AddResult(evalPath, pathRef, evalHit);
            else
                Next().Evaluate(evalPath, pathRef, evalHit, ctx);
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
        EvaluationContextImpl ctx)
    {
        ctx.AddResult(currentPath, parent, pathFunction.Invoke(currentPath, parent, model, ctx, null));
    }

    public abstract void Evaluate(string currentPath, PathRef parent, object? model, EvaluationContextImpl ctx);

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