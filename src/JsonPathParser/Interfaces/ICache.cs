namespace XavierJefferson.JsonPathParser.Interfaces;

public interface ICache
{
    /// <summary>
    ///     Get the Cached JsonPath
    ///     <param name="key">cache key to lookup the JsonPath</param>
    ///     <returns> JsonPath</returns>
    JsonPath? Get(string key);

    /// <summary>
    ///     Add JsonPath to the cache
    ///     <param name="key">cache key to store the JsonPath</param>
    ///     <param name="value">JsonPath to be cached</param>
    void Add(string key, JsonPath? value);
}