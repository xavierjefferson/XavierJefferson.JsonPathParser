using System.Runtime.Caching;

namespace XavierJefferson.JsonPathParser;

public class CacheManager
{
    private static readonly MemoryCache Mc = new MemoryCache(typeof(CacheManager).FullName);
    public static CacheManager Instance { get; } = new();

    public object? Get(string key)
    {
        return Mc.Get(key);
    }

    public void Add(string key, object? value)
    {
        Mc.Add(key, value, DateTimeOffset.Now.AddMinutes(1));
    }
}