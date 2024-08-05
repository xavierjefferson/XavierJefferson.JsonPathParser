using System.Collections;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Mapper;

public class NewtonsoftJsonMappingProvider : MappingProviderBase
{
    protected override object? Deserialize(string s, Type t)
    {
        return JsonConvert.DeserializeObject(s, t);
    }

    protected override object? MapToObject(object? source)
    {
        if (source is JArray array)
        {
            JpObjectList mapped;
            mapped = new JpObjectList(array.Select(i => MapToObject(i)));
            return mapped;
        }

        if (source is JObject jObjectSource)
        {
            var mapped = new JpDictionary();
            IDictionary<string, JToken?> obj = jObjectSource;

            foreach (var m in obj) mapped.Add(m.Key, MapToObject(m.Value));
            return mapped;
        }

        if (source == null)
            return null;
        return source;
    }

    protected override string Serialize(object? source)
    {
        
            return JsonConvert.SerializeObject(source);
         
    }
}