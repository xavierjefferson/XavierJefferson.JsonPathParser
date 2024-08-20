using XavierJefferson.JsonPathParser.Exceptions;
using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Caching;

public class CacheProvider
{
    private bool _accessed;
    private ICache _cache = new SystemRuntimeCachingCache();
    private bool _registered;
    public static CacheProvider Instance { get; } = new();

    public ICache Cache
    {
        get
        {
            _accessed = true;
            return _cache;
        }
        set
        {
            if (_registered || _accessed)
                throw new JsonPathException(
                    "Cache provider must be configured before cache is accessed and must not be registered twice.");


            _cache = value ?? throw new ArgumentNullException(nameof(Cache));
            _registered = true;
        }
    }
}