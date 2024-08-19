using Newtonsoft.Json.Linq;

namespace XavierJefferson.JsonPathParser.Mapper;

public class NewtonsoftJsonMappingProvider : MappingProviderBase
{
    protected override object? MapToObject(object? source)
    {
        if (source is JArray array) return array.Select(MapToObject).ToList();

        if (source is JObject jObjectSource)
        {
            IDictionary<string, JToken?> obj = jObjectSource;

            return obj.ToDictionary(i => i.Key, i => MapToObject(i.Value));
        }

        return source;
    }
}