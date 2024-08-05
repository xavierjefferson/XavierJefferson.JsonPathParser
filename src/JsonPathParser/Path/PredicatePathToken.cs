using System.Text;
using XavierJefferson.JsonPathParser.Exceptions;
using XavierJefferson.JsonPathParser.Interfaces;
using XavierJefferson.JsonPathParser.PathRefs;

namespace XavierJefferson.JsonPathParser.Path;

/// <summary>
///     */
public class PredicatePathToken : PathToken
{
    private readonly ICollection<IPredicate> _predicates;

    public PredicatePathToken(IPredicate filter)
    {
        _predicates = new SerializingList<IPredicate> { filter };
    }

    public PredicatePathToken(ICollection<IPredicate> predicates)
    {
        this._predicates = predicates;
    }


    public override void Evaluate(string currentPath, PathRef @ref, object? model, EvaluationContextImpl ctx)
    {
        if (ctx.JsonProvider.IsMap(model))
        {
            if (Accept(model, ctx.RootDocument, ctx.Configuration, ctx))
            {
                var op = ctx.ForUpdate() ? @ref : PathRef.NoOp;
                if (IsLeaf())
                    ctx.AddResult(currentPath, op, model);
                else
                    Next().Evaluate(currentPath, op, model, ctx);
            }
        }
        else if (ctx.JsonProvider.IsArray(model))
        {
            var idx = 0;
            var objects = ctx.JsonProvider.AsEnumerable(model);

            foreach (var idxModel in objects)
            {
                if (Accept(idxModel, ctx.RootDocument, ctx.Configuration, ctx))
                    HandleArrayIndex(idx, currentPath, model, ctx);
                idx++;
            }
        }
        else
        {
            if (IsUpstreamDefinite())
                throw new InvalidPathException(
                    $"Filter: {ToString()} can not be applied to primitives. Current context is: {model}");
        }
    }

    public bool Accept(object? obj, object? root, Configuration configuration, EvaluationContextImpl evaluationContext)
    {
        IPredicateContext ctx =
            new PredicateContextImpl(obj, root, configuration, evaluationContext.DocumentEvalCache());

        foreach (var predicate in _predicates)
            try
            {
                if (!predicate.Apply(ctx)) return false;
            }
            catch (InvalidPathException e)
            {
                return false;
            }

        return true;
    }


    public override string GetPathFragment()
    {
        var sb = new StringBuilder();
        sb.Append("[");
        for (var i = 0; i < _predicates.Count(); i++)
        {
            if (i != 0) sb.Append(",");
            sb.Append("?");
        }

        sb.Append("]");
        return sb.ToString();
    }


    public override bool IsTokenDefinite()
    {
        return false;
    }
}