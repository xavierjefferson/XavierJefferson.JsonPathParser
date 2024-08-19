using System.Collections;
using XavierJefferson.JsonPathParser.Exceptions;
using XavierJefferson.JsonPathParser.Interfaces;
using XavierJefferson.JsonPathParser.Logging;
using XavierJefferson.JsonPathParser.Path;

namespace XavierJefferson.JsonPathParser.Filtering.ValueNodes;

public class PathNode : TypedValueNode<IPath>
{
    private static readonly ILog Logger = LoggerFactory.GetLogger(typeof(PathNode));
    private readonly bool _existsCheck;

    private readonly IPath _path;
    private readonly bool _shouldExist;

    public PathNode(string charSequence, bool existsCheck, bool shouldExist) : this(
        PathCompiler.Compile(charSequence), existsCheck, shouldExist)
    {
    }

    public PathNode(IPath path, bool existsCheck = false, bool shouldExist = false)
    {
        _path = path;
        _existsCheck = existsCheck;
        _shouldExist = shouldExist;
        Logger.Trace($"PathNode {path} existsCheck: {existsCheck}");
    }

    public override IPath Value => _path;

    public override int GetHashCode()
    {
        return _path.GetHashCode();
    }

    public IPath GetPath()
    {
        return _path;
    }

    public bool IsExistsCheck()
    {
        return _existsCheck;
    }

    public bool ShouldExists()
    {
        return _shouldExist;
    }


    public override Type Type(IPredicateContext context)
    {
        return typeof(void);
    }

    public override PathNode AsPathNode()
    {
        return this;
    }

    public PathNode AsExistsCheck(bool shouldExist)
    {
        return new PathNode(_path, true, shouldExist);
    }


    public override string ToString()
    {
        return $"{(_existsCheck && !_shouldExist ? "!" : "")}{_path}";
    }

    public ValueNode Evaluate(IPredicateContext context)
    {
        if (IsExistsCheck())
            try
            {
                var c = Configuration.CreateBuilder().WithJsonProvider(context.Configuration.JsonProvider)
                    .WithOptions(Option.RequireProperties).Build();
                var result = _path.Evaluate(context.Item, context.Root, c).GetValue(false);
                return result == IJsonProvider.Undefined ? ValueNodeConstants.False : ValueNodeConstants.True;
            }
            catch (PathNotFoundException)
            {
                return ValueNodeConstants.False;
            }

        try
        {
            object? res;
            if (context is PredicateContextImpl predicateContextImpl)
            {
                //This will use cache for document ($) queries
                res = predicateContextImpl.Evaluate(_path);
            }
            else
            {
                var doc = _path.IsRootPath() ? context.Root : context.Item;
                res = _path.Evaluate(doc, context.Root, context.Configuration).GetValue();
            }

            res = context.Configuration.JsonProvider.Unwrap(res);

            if (TryQuickCastNode(res, false, null, out var valueNode)) return valueNode;


            if (res == null) return ValueNodeConstants.NullNode;
            if (context.Configuration.JsonProvider.IsArray(res))
                return CreateJsonNode(
                    context.Configuration.MappingProvider.Map(res, typeof(IList), context.Configuration));
            if (context.Configuration.JsonProvider.IsMap(res))
                return CreateJsonNode(
                    context.Configuration.MappingProvider.Map(res, typeof(IDictionary), context.Configuration));
            throw new JsonPathException(
                $"Could not convert {res.GetType().FullName} '{res}' to a {nameof(ValueNode)}");
        }
        catch (PathNotFoundException)
        {
            return ValueNodeConstants.Undefined;
        }
    }
}