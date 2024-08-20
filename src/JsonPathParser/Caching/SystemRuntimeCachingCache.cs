using System.Runtime.Caching;
using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Caching;

public class SystemRuntimeCachingCache : ICache
{
    private static readonly MemoryCache Mc = new(typeof(SystemRuntimeCachingCache).FullName);

    public void Add(string key, JsonPath? value)
    {
        Mc.Add(key, value, DateTimeOffset.Now.AddMinutes(5));
    }

    public JsonPath? Get(string key)
    {
        return Mc.Get(key) as JsonPath;
    }
}