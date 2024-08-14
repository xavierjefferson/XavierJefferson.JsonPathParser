using XavierJefferson.JsonPathParser.Interfaces;
using XavierJefferson.JsonPathParser.Logging;

namespace XavierJefferson.JsonPathParser.Path;

public class PredicateContextImpl : IPredicateContext
{
    private static readonly ILog Logger = LoggerFactory.GetLogger(typeof(PredicateContextImpl));
    private readonly Dictionary<IPath, object?> _documentPathCache;

    public PredicateContextImpl(object? contextDocument, object? rootDocument, Configuration configuration,
        Dictionary<IPath, object?> documentPathCache)
    {
        Item = contextDocument;
        Root = rootDocument;
        Configuration = configuration;
        _documentPathCache = documentPathCache;
    }


    public object? Item { get; }


    public object? GetItem(Type type)
    {
        return Configuration?.MappingProvider?.Map(Item, type, Configuration);
    }


    public object? Root { get; }


    public Configuration? Configuration { get; }

    public T? GetItem<T>()
    {
        return (T?)GetItem(typeof(T));
    }

    public object? Evaluate(IPath path)
    {
        object? result;
        if (path.IsRootPath())
        {
            if (_documentPathCache.ContainsKey(path))
            {
                Logger.DebugFormat("Using cached result for root path: " + path);
                result = _documentPathCache[path];
            }
            else
            {
                result = path.Evaluate(Root, Root, Configuration).GetValue();
                _documentPathCache.Add(path, result);
            }
        }
        else
        {
            result = path.Evaluate(Item, Root, Configuration).GetValue();
        }

        return result;
    }

    public Dictionary<IPath, object?> DocumentPathCache()
    {
        return _documentPathCache;
    }
}