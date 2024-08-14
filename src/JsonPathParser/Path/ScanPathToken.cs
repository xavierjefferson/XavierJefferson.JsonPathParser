using XavierJefferson.JsonPathParser.Extensions;
using XavierJefferson.JsonPathParser.Interfaces;
using XavierJefferson.JsonPathParser.PathRefs;

namespace XavierJefferson.JsonPathParser.Path;

/// <summary>
///     */
public class ScanPathToken : PathToken
{
    private static readonly IPredicate FalsePredicate = new FalsePredicateImpl();


    public override void Evaluate(string currentPath, PathRef parent, object? model, EvaluationContextImpl ctx)
    {
        var pt = Next();

        Walk(pt, currentPath, parent, model, ctx, CreateScanPredicate(pt, ctx));
    }

    public static void Walk(PathToken pt, string currentPath, PathRef parent, object? model, EvaluationContextImpl ctx,
        IPredicate predicate)
    {
        if (ctx.JsonProvider.IsMap(model))
            WalkObject(pt, currentPath, parent, model, ctx, predicate);
        else if (ctx.JsonProvider.IsArray(model)) WalkArray(pt, currentPath, parent, model, ctx, predicate);
    }

    public static void WalkArray(PathToken pt, string currentPath, PathRef parent, object? model,
        EvaluationContextImpl ctx, IPredicate predicate)
    {
        if (predicate.Matches(model))
        {
            if (pt.IsLeaf())
            {
                pt.Evaluate(currentPath, parent, model, ctx);
            }
            else
            {
                var next = pt.Next();
                var models1 = ctx.JsonProvider.AsEnumerable(model);
                foreach (var evalModel in models1.Cast<object?>().ToIndexedEnumerable())
                {
                    var evalPath = currentPath + $"[{evalModel.Index}]";
                    next.SetUpstreamArrayIndex(evalModel.Index);
                    next.Evaluate(evalPath, parent, evalModel.Value, ctx);
                }
            }
        }

        var models = ctx.JsonProvider.AsEnumerable(model).Cast<object?>();
        var idx = 0;
        foreach (var evalModel in models)
        {
            var evalPath = currentPath + "[" + idx + "]";
            Walk(pt, evalPath, PathRef.Create(model, idx), evalModel, ctx, predicate);
            idx++;
        }
    }

    public static void WalkObject(PathToken pt, string currentPath, PathRef parent, object? model,
        EvaluationContextImpl ctx, IPredicate predicate)
    {
        if (predicate.Matches(model)) pt.Evaluate(currentPath, parent, model, ctx);
        var properties = ctx.JsonProvider.GetPropertyKeys(model);

        foreach (var property in properties)
        {
            var evalPath = currentPath + "['" + property + "']";
            var propertyModel = ctx.JsonProvider.GetMapValue(model, property);
            if (propertyModel != IJsonProvider.Undefined)
                Walk(pt, evalPath, PathRef.Create(model, property), propertyModel, ctx, predicate);
        }
    }

    private static IPredicate CreateScanPredicate(PathToken target, EvaluationContextImpl ctx)
    {
        if (target is PropertyPathToken)
            return new PropertyPathTokenPredicate(target, ctx);
        if (target is ArrayPathToken)
            return new ArrayPathTokenPredicate(ctx);
        if (target is WildcardPathToken)
            return new WildcardPathTokenPredicate();
        if (target is PredicatePathToken)
            return new FilterPathTokenPredicate(target, ctx);
        return FalsePredicate;
    }


    public override bool IsTokenDefinite()
    {
        return false;
    }


    public override string GetPathFragment()
    {
        return "..";
    }

    public interface IPredicate
    {
        bool Matches(object? model);
    }

    private class FalsePredicateImpl : IPredicate
    {
        public bool Matches(object? model)
        {
            return false;
        }
    }

    public class FilterPathTokenPredicate : IPredicate
    {
        private readonly EvaluationContextImpl _ctx;
        private readonly PredicatePathToken _predicatePathToken;

        public FilterPathTokenPredicate(PathToken target, EvaluationContextImpl ctx)
        {
            _ctx = ctx;
            _predicatePathToken = (PredicatePathToken)target;
        }


        public bool Matches(object? model)
        {
            return _predicatePathToken.Accept(model, _ctx.RootDocument, _ctx.Configuration, _ctx);
        }
    }

    public class WildcardPathTokenPredicate : IPredicate
    {
        public bool Matches(object? model)
        {
            return true;
        }
    }

    public class ArrayPathTokenPredicate : IPredicate
    {
        private readonly EvaluationContextImpl _ctx;

        public ArrayPathTokenPredicate(EvaluationContextImpl ctx)
        {
            _ctx = ctx;
        }


        public bool Matches(object? model)
        {
            return _ctx.JsonProvider.IsArray(model);
        }
    }

    public class PropertyPathTokenPredicate : IPredicate
    {
        private readonly EvaluationContextImpl _ctx;
        private readonly PropertyPathToken _propertyPathToken;

        public PropertyPathTokenPredicate(PathToken target, EvaluationContextImpl ctx)
        {
            _ctx = ctx;
            _propertyPathToken = (PropertyPathToken)target;
        }


        public bool Matches(object? model)
        {
            if (!_ctx.JsonProvider.IsMap(model)) return false;

            //
            // The commented code below makes it really hard understand, use and predict the result
            // of deep scanning operations. It might be correct but was decided to be
            // left out until the behavior of REQUIRE_PROPERTIES is more strictly defined
            // in a deep scanning scenario. For details read conversation in commit
            // https://github.com/jayway/JsonPath/commit/1a72fc078deb16995e323442bfb681bd715ce45a#commitcomment-14616092
            //
            //            if (ctx.Options.Contains(Option.REQUIRE_PROPERTIES)) {
            //                // Have to require properties defined in path when an indefinite path is evaluated,
            //                // so have to go there and search for it.
            //                return true;
            //            }

            if (!_propertyPathToken.IsTokenDefinite())
                // It's responsibility of PropertyPathToken code to handle indefinite scenario of properties,
                // so we'll allow it to do its job.
                return true;

            if (_propertyPathToken.IsLeaf() && _ctx.Options.Contains(Option.DefaultPathLeafToNull))
                // In case of DEFAULT_PATH_LEAF_TO_NULL missing properties is not a problem.
                return true;

            var keys = _ctx.JsonProvider.GetPropertyKeys(model);
            return keys.ContainsAll(_propertyPathToken.GetProperties());
        }
    }
}