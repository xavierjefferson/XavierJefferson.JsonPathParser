using System.Collections;
using System.Text;

namespace XavierJefferson.JsonPathParser.Interfaces;

public interface IJsonProvider
{
    static readonly object Undefined = new();

    /// <summary>
    ///     Parse the given json string
    ///     <param name="json">json string to Parse</param>
    ///     <returns> object representation of json</returns>
    object? Parse(string json);

    /// <summary>
    ///     Parse the given json bytes in UTF-8 encoding
    ///     <param name="json">json bytes to Parse</param>
    ///     <returns> object representation of json</returns>
    object? Parse(byte[] json)
    {
        return Parse(Encoding.UTF8.GetString(json));
    }

    /// <summary>
    ///     Parse the given json string
    ///     <param name="jsonStream">input stream to Parse</param>
    ///     <param name="charset">charset to use</param>
    ///     <returns> object representation of json</returns>
    object? Parse(Stream jsonStream, Encoding charset);

    /// <summary>
    ///     Convert given json object to a json string
    ///     <param name="obj">object to transform</param>
    ///     <returns> json representation of object</returns>
    string ToJson(object? obj);

    /// <summary>
    ///     Creates a provider specific json array
    ///     <returns> new array</returns>
    object? CreateArray();

    /// <summary>
    ///     Creates a provider specific json object
    ///     <returns> new object</returns>
    object? CreateMap();

    /// <summary>
    ///     checks if object is an array
    /// </summary>
    /// <param name="obj">object to check</param>
    /// <returns> true if obj is an array</returns>
    bool IsArray(object? obj);

    /// <summary>
    ///     Get the length of an json array, json object or a json string
    /// </summary>
    /// <param name="obj">an array or object or a string</param>
    /// <returns> the number of entries in the array or object</returns>
    int Length(object? obj);

    /// <summary>
    ///     Converts given array to an {@link IEnumerable}
    /// </summary>
    /// <param name="obj">an array</param>
    /// <returns> an IEnumerable that iterates over the entries of an array</returns>
    IEnumerable AsEnumerable(object? obj);


    /// <summary>
    ///     Returns the keys from the given object
    /// </summary>
    /// <param name="obj">an object</param>
    /// <returns> the keys for an object</returns>
    ICollection<string> GetPropertyKeys(object? obj);

    /// <summary>
    ///     Extracts a value from an array anw unwraps provider specific data type
    /// </summary>
    /// <param name="obj">an array</param>
    /// <param name="idx">index</param>
    /// <returns> the entry at the given index</returns>
    object? GetArrayIndex(object? obj, int idx);

    /// <summary>
    ///     Extracts a value from an array
    /// </summary>
    /// <param name="obj">an array</param>
    /// <param name="idx">index</param>
    /// <param name="unwrap">should provider specific data type be unwrapped</param>
    /// <returns> the entry at the given index</returns>
    [Obsolete]
    object? GetArrayIndex(object? obj, int idx, bool unwrap);

    /// <summary>
    ///     Sets a value in an array. If the array is too small, the provider is supposed to enlarge it.
    /// </summary>
    /// <param name="array">an array</param>
    /// <param name="idx">index</param>
    /// <param name="newValue">the new value</param>
    void SetArrayIndex(object? array, int idx, object? newValue);

    /// <summary>
    ///     Extracts a value from an map
    /// </summary>
    /// <param name="obj">a map</param>
    /// <param name="key">property key</param>
    /// <returns> the map entry or {@link com.jayway.jsonpath.spi.json.JsonProvider#UNDEFINED} for missing properties</returns>
    object? GetMapValue(object? obj, string key);

    /// <summary>
    ///     Sets a value in an object
    /// </summary>
    /// <param name="obj">an object</param>
    /// <param name="key">a string key</param>
    /// <param name="value">the value to set</param>
    void SetProperty(object? obj, object? key, object? value);

    /// <summary>
    ///     Removes a value in an object or array
    /// </summary>
    /// <param name="obj">an array or an object</param>
    /// <param name="key">a string key or a numerical index to remove</param>
    void RemoveProperty(object? obj, object? key);

    /// <summary>
    ///     checks if object is a map (i.e. no array)
    /// </summary>
    /// <param name="obj">object to check</param>
    /// <returns> true if the object is a map</returns>
    bool IsMap(object? obj);

    /// <summary>
    ///     Extracts a value from a wrapper object. For JSON providers that to not wrap
    ///     values, this will usually be the object itself.
    /// </summary>
    /// <param name="obj">a value holder object</param>
    /// <returns> the unwrapped value.</returns>
    object? Unwrap(object? obj);
}