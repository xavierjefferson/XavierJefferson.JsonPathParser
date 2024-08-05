using System.Collections;
using XavierJefferson.JsonPathParser.Exceptions;
using XavierJefferson.JsonPathParser.Extensions;
using XavierJefferson.JsonPathParser.Helpers;
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

    public override IPath Value => _path;

    public PathNode(string charSequence, bool existsCheck, bool shouldExist) : this(
        PathCompiler.Compile(charSequence), existsCheck, shouldExist)
    {
    }

    public PathNode(IPath path, bool existsCheck = false, bool shouldExist = false)
    {
        this._path = path;
        this._existsCheck = existsCheck;
        this._shouldExist = shouldExist;
        Logger.Trace($"PathNode {path} existsCheck: {existsCheck}");
    }

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


    public override Type Type(IPredicateContext ctx)
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
        return _existsCheck && !_shouldExist ? StringHelper.Concat("!", _path.ToString()) : _path.ToString();
    }

    public ValueNode Evaluate(IPredicateContext ctx)
    {
        if (IsExistsCheck())
            try
            {
                var c = Configuration.CreateBuilder().WithJsonProvider(ctx.Configuration.JsonProvider)
                    .WithOptions(Option.RequireProperties).Build();
                var result = _path.Evaluate(ctx.Item, ctx.Root, c).GetValue(false);
                return result == IJsonProvider.Undefined ? ValueNodeConstants.False : ValueNodeConstants.True;
            }
            catch (PathNotFoundException)
            {
                return ValueNodeConstants.False;
            }

        try
        {
            object? res;
            if (ctx is PredicateContextImpl predicateContextImpl)
            {
                //This will use cache for document ($) queries
                res = predicateContextImpl.Evaluate(_path);
            }
            else
            {
                var doc = _path.IsRootPath() ? ctx.Root : ctx.Item;
                res = _path.Evaluate(doc, ctx.Root, ctx.Configuration).GetValue();
            }

            res = ctx.Configuration.JsonProvider.Unwrap(res);
            if (res.TryConvertDouble(out var test))
                return CreateNumberNode(test);
            if (IsNumeric(res)) return CreateNumberNode(res.ToString());
            if (res is string) return CreateStringNode(res.ToString(), false);
            if (res is bool) return CreateBooleanNode(res.ToString());
            if (res is DateTimeOffset d1)
                return CreateDateTimeOffsetNode(
                    d1); //workaround for issue: https://github.com/json-path/JsonPath/issues/613
            if (res == null) return ValueNodeConstants.NullNode;
            if (ctx.Configuration.JsonProvider.IsArray(res))
                return CreateJsonNode(ctx.Configuration.MappingProvider.Map(res, typeof(IList), ctx.Configuration));
            if (ctx.Configuration.JsonProvider.IsMap(res))
                return CreateJsonNode(
                    ctx.Configuration.MappingProvider.Map(res, typeof(IDictionary), ctx.Configuration));
            throw new JsonPathException(
                $"Could not convert {res.GetType().FullName} '{res}' to a {nameof(ValueNode)}");
        }
        catch (PathNotFoundException)
        {
            return ValueNodeConstants.Undefined;
        }
    }
}