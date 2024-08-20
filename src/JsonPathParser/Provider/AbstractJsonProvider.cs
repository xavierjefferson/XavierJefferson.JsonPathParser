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
        if (TryConvertToIList(obj, out var list)) return list[idx];
        throw new ArgumentException("The argument is not an array");
    }

    [Obsolete]
    public virtual object? GetArrayIndex(object? obj, int idx, bool unwrap)
    {
        return GetArrayIndex(obj, idx);
    }

    public virtual void SetArrayIndex(object? array, int index, object? newValue)
    {
        if (!TryConvertToIList(array, out var list)) throw new ArgumentException("The argument is not an array");
        if (list.IsFixedSize) throw new ArgumentException("The argument is a fixed size");
        if (list.IsReadOnly) throw new ArgumentException("The argument is read only");

        if (index == list.Count)
            list.Add(newValue);
        else
            list[index] = newValue;
    }


    /// <summary>
    ///     Extracts a value from an map
    /// </summary>
    /// <param name="obj">a map</param>
    /// <param name="key">property key</param>
    /// <returns> the map entry or {@link com.jayway.jsonpath.spi.json.JsonProvider#UNDEFINED} for missing properties</returns>
    public virtual object? GetMapValue(object? obj, string key)
    {
        if (TryConvertToIDictionary(obj, out var dictionary))
        {
            foreach (var currentKey in dictionary.Keys.Cast<string>())
                if (currentKey == key)
                    return dictionary[key];
        }
        else
        {
            throw new ArgumentException($"The argument does not implement {nameof(IDictionary)}");
        }

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
        if (TryConvertToIDictionary(obj, out var dictionary))
            dictionary[key] = value;
        else
            throw new JsonPathException(
                $"{nameof(SetProperty)} operation cannot be used with {SerializeTypeName(obj)}");
    }


    /// <summary>
    ///     Removes a value in an object or array
    /// </summary>
    /// <param name="obj">an array or an object</param>
    /// <param name="key">a string key or a numerical index to remove</param>
    public virtual void RemoveProperty(object? obj, object? key)
    {
        if (TryConvertToIDictionary(obj, out var dictionary))
        {
            dictionary.Remove(key);
            return;
        }

        if (TryConvertToIList(obj, out var list))
        {
            var index = key is int ? (int)key : int.Parse(key.ToString());
            list.RemoveAt(index);
            return;
        }

        throw new JsonPathException(
            $"{nameof(RemoveProperty)} operation cannot be used with {SerializeTypeName(obj)}");
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
        if (TryConvertToIDictionary(obj, out var dictionary)) return dictionary.Keys.Cast<string>().ToList();
        throw new ArgumentException($"The argument does not implement {nameof(IDictionary)}");
    }

    /// <summary>
    ///     Get the length of an array or object
    /// </summary>
    /// <param name="obj">an array or an object</param>
    /// <returns> the number of entries in the array or object</returns>
    public virtual int Length(object? obj)
    {
        if (obj is string stringInstance) return stringInstance.Length;
        if (TryConvertToIDictionary(obj, out var dictionary)) return dictionary.Count;
        if (TryConvertToIList(obj, out var list)) return list.Count;


        throw new JsonPathException($"{nameof(Length)} operation cannot be applied to {SerializeTypeName(obj)}");
    }

    /// <summary>
    ///     Converts given array to an <see cref=""/>
    /// </summary>
    /// <param name="obj">an array</param>
    /// <returns> an IEnumerable that iterates over the entries of an array</returns>
    public virtual List<object?> AsEnumerable(object? obj)
    {
        if (obj is IEnumerable enumerable) return enumerable.Cast<object?>().ToList();
        throw new JsonPathException($"Cannot iterate over {SerializeTypeName(obj)}");
    }


    public virtual object? Unwrap(object? obj)
    {
        if (obj.TryConvertDouble(out var test)) return test;
        return obj;
    }

    public abstract T Deserialize<T>(string obj);
    public abstract string Serialize(object? obj);

    public object? Parse(string json)
    {
        try
        {
            var cleanJson = StringExtensions.Beautify(json);
            return Cleanup(Deserialize<object?>(cleanJson));
        }
        catch (Exception e)
        {
            throw new InvalidJsonException(e);
        }
    }

    public object? Parse(Stream jsonStream, Encoding encoding)
    {
        try
        {
            using (var sr = new StreamReader(jsonStream, encoding))
            {
                return Parse(sr.ReadToEnd());
            }
        }
        catch (Exception e)
        {
            throw new InvalidJsonException(e);
        }
    }

    public string ToJson(object? obj)
    {
        return Serialize(obj);
    }

    public object? CreateArray()
    {
        return new List<object?>();
    }


    public object? CreateMap()
    {
        return new Dictionary<string, object?>();
    }


    public abstract object? Deserialize(string obj, Type type);

    protected abstract object? Cleanup(object? obj);

    private static bool TryConvertToIList(object? obj, out IList list)
    {
        if (obj is IDictionary)
        {
            list = null;
            return false;
        }

        list = obj as IList;
        return list != null;
    }

    private static bool TryConvertToIDictionary(object? obj, out IDictionary list)
    {
        list = obj as IDictionary;
        return list != null;
    }

    private static string SerializeTypeName(object? value)
    {
        if (value == null) return "null";
        return value.GetType().FullName;
    }
}