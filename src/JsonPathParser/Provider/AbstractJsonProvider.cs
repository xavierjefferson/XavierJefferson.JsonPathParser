using System.Collections;
using System.Text;
using XavierJefferson.JsonPathParser.Exceptions;
using XavierJefferson.JsonPathParser.Extensions;
using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Provider;

public abstract class AbstractJsonProvider : IJsonProvider
{
    /// <summary>
    ///     checks if object is an array
    /// </summary>
    /// <param name="obj">object to check</param>
    /// <returns> true if obj is an array</returns>
    public virtual bool IsArray(object? obj)
    {
        return obj is IList;
    }

    /// <summary>
    ///     Extracts a value from an array
    /// </summary>
    /// <param name="obj">an array</param>
    /// <param name="idx">index</param>
    /// <returns> the entry at the given index</returns>
    public virtual object? GetArrayIndex(object? obj, int idx)
    {
        return ((IList)obj)[idx];
    }

    [Obsolete]
    public virtual object? GetArrayIndex(object? obj, int idx, bool unwrap)
    {
        return GetArrayIndex(obj, idx);
    }

    public virtual void SetArrayIndex(object? array, int index, object? newValue)
    {
        if (!IsArray(array)) throw new NotImplementedException();

        var l = array as JpObjectList;
        if (index == l.Count)
            l.Add(newValue);
        else
            l[index] = newValue;
    }


    /// <summary>
    ///     Extracts a value from an map
    /// </summary>
    /// <param name="obj">a map</param>
    /// <param name="key">property key</param>
    /// <returns> the map entry or {@link com.jayway.jsonpath.spi.json.JsonProvider#UNDEFINED} for missing properties</returns>
    public virtual object? GetMapValue(object? obj, string key)
    {
        var m = (IDictionary)obj;
        foreach (var n in m.Keys.Cast<string>())
            if (n == key)
                return m[key];

        return IJsonProvider.Undefined;
    }

    /// <summary>
    ///     Sets a value in an object
    /// </summary>
    /// <param name="obj">an object</param>
    /// <param name="key">a string key</param>
    /// <param name="value">the value to set</param>
    public virtual void SetProperty(object? obj, object? key, object? value)
    {
        if (IsMap(obj))
            ((IDictionary)obj).Add(key?.ToString(), value);
        else
            throw new JsonPathException("setProperty operation cannot be used with " + obj != null
                ? obj.GetType().FullName
                : "null");
    }


    /// <summary>
    ///     Removes a value in an object or array
    /// </summary>
    /// <param name="obj">an array or an object</param>
    /// <param name="key">a string key or a numerical index to remove</param>
    public virtual void RemoveProperty(object? obj, object? key)
    {
        if (IsMap(obj))
        {
            ((IDictionary)obj).Remove(key);
        }
        else
        {
            var list = obj as JpObjectList;
            var index = key is int ? (int)key : int.Parse(key.ToString());
            list.Remove(index);
        }
    }


    /// <summary>
    ///     checks if object is a map (i.e. no array)
    /// </summary>
    /// <param name="obj">object to check</param>
    /// <returns> true if the object is a map</returns>
    public virtual bool IsMap(object? obj)
    {
        return obj is IDictionary;
    }

    /// <summary>
    ///     Returns the keys from the given object
    /// </summary>
    /// <param name="obj">an object</param>
    /// <returns> the keys for an object</returns>
    public virtual ICollection<string> GetPropertyKeys(object? obj)
    {
        if (IsArray(obj))
            throw new NotImplementedException();
        return ((IDictionary)obj).Keys.Cast<string>().ToSerializingList();
    }

    /// <summary>
    ///     Get the length of an array or object
    /// </summary>
    /// <param name="obj">an array or an object</param>
    /// <returns> the number of entries in the array or object</returns>
    public virtual int Length(object? obj)
    {
        if (IsArray(obj))
            return ((IList)obj).Count;
        if (IsMap(obj))
            return GetPropertyKeys(obj).Count();
        if (obj is string) return ((string)obj).Length;
        throw new JsonPathException("length operation cannot be applied to " + (obj != null
            ? obj.GetType().FullName
            : "null"));
    }

    /// <summary>
    ///     Converts given array to an {@link IEnumerable}
    /// </summary>
    /// <param name="obj">an array</param>
    /// <returns> an IEnumerable that iterates over the entries of an array</returns>
    public virtual IEnumerable AsEnumerable(object? obj)
    {
        if (IsArray(obj))
            return (IEnumerable)obj;
        throw new JsonPathException("Cannot iterate over " + obj != null ? obj.GetType().FullName : "null");
    }


    public virtual object? Unwrap(object? obj)
    {
        if (obj.TryConvertDouble(out var test)) return test;
        return obj;
    }

    public abstract object? Parse(string json);
    public abstract object? Parse(Stream jsonStream, Encoding charset);
    public abstract string ToJson(object? obj);
    public abstract object? CreateArray();
    public abstract object? CreateMap();
}