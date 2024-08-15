using XavierJefferson.JsonPathParser.Extensions;
using XavierJefferson.JsonPathParser.Interfaces;
using XavierJefferson.JsonPathParser.PathRefs;

namespace XavierJefferson.JsonPathParser.Path;


public class ScanPathToken : PathToken
{
    private static readonly IPredicate FalsePredicate = new FalsePredicateImpl();


    public override void Evaluate(string currentPath, PathRef parent, object? model, EvaluationContextImpl context)
    {
        var pt = Next();

        Walk(pt, currentPath, parent, model, context, CreateScanPredicate(pt, context));
    }

    public static void Walk(PathToken pt, string currentPath, PathRef parent, object? model, EvaluationContextImpl context,
        IPredicate predicate)
    {
        if (context.JsonProvider.IsMap(model))
            WalkObject(pt, currentPath, parent, model, context, predicate);
        else if (context.JsonProvider.IsArray(model)) WalkArray(pt, currentPath, parent, model, context, predicate);
    }

    public static void WalkArray(PathToken pt, string currentPath, PathRef parent, object? model,
        EvaluationContextImpl context, IPredicate predicate)
    {
        if (predicate.Matches(model))
        {
            if (pt.IsLeaf())
            {
                pt.Evaluate(currentPath, parent, model, context);
            }
            else
            {
                var next = pt.Next();
                var models1 = context.JsonProvider.AsEnumerable(model);
                foreach (var evalModel in models1.ToIndexedEnumerable())
                {
                    var evalPath = currentPath + $"[{evalModel.Index}]";
                    next.SetUpstreamArrayIndex(evalModel.Index);
                    next.Evaluate(evalPath, parent, evalModel.Value, context);
                }
            }
        }

        var models = context.JsonProvider.AsEnumerable(model);

        foreach (var evalModel in models.ToIndexedEnumerable())
        {
            var evalPath = $"{currentPath}[{evalModel.Index}]";
            Walk(pt, evalPath, PathRef.Create(model, evalModel.Index), evalModel.Value, context, predicate);
        }
    }

    public static void WalkObject(PathToken pathToken, string currentPath, PathRef parent, object? model,
        EvaluationContextImpl context, IPredicate predicate)
    {
        if (predicate.Matches(model)) pathToken.Evaluate(currentPath, parent, model, context);
        var properties = context.JsonProvider.GetPropertyKeys(model);

        foreach (var property in properties)
        {
            var evalPath = $"{currentPath}['{property}']";
            var propertyModel = context.JsonProvider.GetMapValue(model, property);
            if (propertyModel != IJsonProvider.Undefined)
                Walk(pathToken, evalPath, PathRef.Create(model, property), propertyModel, context, predicate);
        }
    }

    private static IPredicate CreateScanPredicate(PathToken target, EvaluationContextImpl context)
    {
        switch (target)
        {
            case PropertyPathToken:
                return new PropertyPathTokenPredicate(target, context);
            case ArrayPathToken:
                return new ArrayPathTokenPredicate(context);
            case WildcardPathToken:
                return new WildcardPathTokenPredicate();
            case PredicatePathToken:
                return new FilterPathTokenPredicate(target, context);
            default:
                return FalsePredicate;
        }
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
        private readonly EvaluationContextImpl _context;
        private readonly PredicatePathToken _predicatePathToken;

        public FilterPathTokenPredicate(PathToken target, EvaluationContextImpl context)
        {
            _context = context;
            _predicatePathToken = (PredicatePathToken)target;
        }


        public bool Matches(object? model)
        {
            return _predicatePathToken.Accept(model, _context.RootDocument, _context.Configuration, _context);
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
        private readonly EvaluationContextImpl _context;

        public ArrayPathTokenPredicate(EvaluationContextImpl context)
        {
            _context = context;
        }


        public bool Matches(object? model)
        {
            return _context.JsonProvider.IsArray(model);
        }
    }

    public class PropertyPathTokenPredicate : IPredicate
    {
        private readonly EvaluationContextImpl _context;
        private readonly PropertyPathToken _propertyPathToken;

        public PropertyPathTokenPredicate(PathToken target, EvaluationContextImpl context)
        {
            _context = context;
            _propertyPathToken = (PropertyPathToken)target;
        }


        public bool Matches(object? model)
        {
            if (!_context.JsonProvider.IsMap(model)) return false;

            //
            // The commented code below makes it really hard understand, use and predict the result
            // of deep scanning operations. It might be correct but was decided to be
            // left out until the behavior of REQUIRE_PROPERTIES is more strictly defined
            // in a deep scanning scenario. For details read conversation in commit
            // https://github.com/jayway/JsonPath/commit/1a72fc078deb16995e323442bfb681bd715ce45a#commitcomment-14616092
            //
            //            if (context.Options.Contains(Option.REQUIRE_PROPERTIES)) {
            //                // Have to require properties defined in path when an indefinite path is evaluated,
            //                // so have to go there and search for it.
            //                return true;
            //            }

            if (!_propertyPathToken.IsTokenDefinite())
                // It's responsibility of PropertyPathToken code to handle indefinite scenario of properties,
                // so we'll allow it to do its job.
                return true;

            if (_propertyPathToken.IsLeaf() && _context.Options.Contains(Option.DefaultPathLeafToNull))
                // In case of DEFAULT_PATH_LEAF_TO_NULL missing properties is not a problem.
                return true;

            var keys = _context.JsonProvider.GetPropertyKeys(model);
            return keys.ContainsAll(_propertyPathToken.GetProperties());
        }
    }
}