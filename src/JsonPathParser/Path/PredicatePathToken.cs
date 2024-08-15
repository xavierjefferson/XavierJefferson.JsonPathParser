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
        _predicates = predicates;
    }


    public override void Evaluate(string currentPath, PathRef @ref, object? model, EvaluationContextImpl context)
    {
        if (context.JsonProvider.IsMap(model))
        {
            if (Accept(model, context.RootDocument, context.Configuration, context))
            {
                var op = context.ForUpdate() ? @ref : PathRef.NoOp;
                if (IsLeaf())
                    context.AddResult(currentPath, op, model);
                else
                    Next().Evaluate(currentPath, op, model, context);
            }
        }
        else if (context.JsonProvider.IsArray(model))
        {
            var idx = 0;
            var objects = context.JsonProvider.AsEnumerable(model);

            foreach (var idxModel in objects)
            {
                if (Accept(idxModel, context.RootDocument, context.Configuration, context))
                    HandleArrayIndex(idx, currentPath, model, context);
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
        IPredicateContext context =
            new PredicateContextImpl(obj, root, configuration, evaluationContext.DocumentEvalCache());

        foreach (var predicate in _predicates)
            try
            {
                if (!predicate.Apply(context)) return false;
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