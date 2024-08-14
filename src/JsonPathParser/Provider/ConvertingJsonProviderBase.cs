using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using XavierJefferson.JsonPathParser.Exceptions;
using XavierJefferson.JsonPathParser.Extensions;
using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Provider;

public abstract class ConvertingJsonProviderBase : AbstractJsonProvider
{
    public override object? Unwrap(object? obj)
    {
        if (obj.TryConvertDouble(out var test)) return test;
        return obj;
    }

    public override object? Parse(Stream jsonStream, Encoding encoding)
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


    public override object? CreateArray()
    {
        return new JpObjectList();
    }


    public override object? CreateMap()
    {
        return new JpDictionary();
    }


    //public override bool IsArray(object? obj)
    //{
    //    return obj is JpObjectList;
    //}


    //public override object? GetArrayIndex(object? obj, int idx)
    //{
    //    try
    //    {
    //        return ToJsonArray(obj)[idx];
    //    }
    //    catch (ArgumentOutOfRangeException)
    //    {
    //        throw;
    //    }
    //    catch (Exception e)
    //    {
    //        throw new JsonPathException(e);
    //    }
    //}


    //public override void SetArrayIndex(object? array, int index, object? newValue)
    //{
    //    try
    //    {
    //        if (!IsArray(array))
    //            throw new NotImplementedException();
    //        var z = ToJsonArray(array);
            
    //        if (index == z.Count())
    //        {
    //            z.Add(newValue);
    //        }
    //        else
    //        {
    //            z[index] = newValue;
    //        }
            
    //    }
    //    catch (Exception e)
    //    {
    //        throw new JsonPathException(e);
    //    }
    //}


    //public override object? GetMapValue(object? obj, string key)
    //{
    //    try
    //    {
    //        var jsonObject = ToJsonObject(obj);
    //        if (jsonObject.ContainsKey(key)) return jsonObject[key];
    //        return IJsonProvider.Undefined;
    //    }
    //    catch (Exception e)
    //    {
    //        throw new JsonPathException(e);
    //    }
    //}


    //public override void SetProperty(object? obj, object? key, object? value)
    //{
    //    try
    //    {
    //        var dict = ToJsonObject(obj);


    //        if (dict != null)
    //        {
    //            dict[key.ToString()] = value;
    //        }
    //        else
    //        {
    //            var c = ToJsonArray(obj);
    //            if (c != null)
    //            {
    //                var array = c;
    //                int index;
    //                if (key == null)
    //                {
    //                    array.Add(value);
    //                    index = array.Count;
    //                }
    //                else
    //                {
    //                    index = key is int ? (int)key : int.Parse(key.ToString());
    //                }

    //                if (index == array.Count)
    //                    array.Add(value);
    //                else
    //                    array.Insert(index, value);
    //            }
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        throw new JsonPathException(e);
    //    }
    //}

    //public override void RemoveProperty(object? obj, object? key)
    //{
    //    if (IsMap(obj))
    //    {
    //        ToJsonObject(obj).Remove(key.ToString());
    //    }
    //    else
    //    {
    //        var array = ToJsonArray(obj);
    //        var index = key is int ? (int)key : int.Parse(key.ToString());
    //        array.RemoveAt(index);
    //    }
    //}


    //public override bool IsMap(object? obj)
    //{
    //    return obj is JObject || obj is JpDictionary || obj is List<KeyValuePair<string, object>>;
    //}


    //public override ICollection<string> GetPropertyKeys(object? obj)
    //{
    //    var jsonObject = ToJsonObject(obj);
    //    try
    //    {
    //        return jsonObject.Keys.ToSerializingList();
    //    }
    //    catch (Exception e)
    //    {
    //        throw new JsonPathException(e);
    //    }
    //}


    //public int Length(object? obj)
    //{
    //    var dictionary = ToJsonObject(obj);
    //    if (dictionary != null) return dictionary.Count;
    //    var list = obj as List<string>;
    //    if (list != null) return list.Count;

    //    if (obj is string) return ((string)obj).Length;

    //    throw new JsonPathException("length operation can not applied to " + (obj != null
    //        ? obj.GetType().FullName
    //        : "null"));
    //}


    //public override IEnumerable AsEnumerable(object? obj)
    //{
    //    try
    //    {
    //        var list = ToJsonArray(obj);
    //        if (list != null) return list.ToSerializingList();

    //        var dictionary = ToJsonObject(obj);
    //        return dictionary.Values.ToSerializingList();
    //    }
    //    catch (Exception e)
    //    {
    //        throw new JsonPathException(e);
    //    }
    //}


    //private JpObjectList ToJsonArray(object? o)
    //{
    //    return o as JpObjectList;
    //}

    //private JpDictionary ToJsonObject(object? o)
    //{
    //    return o as JpDictionary;
    //}
}