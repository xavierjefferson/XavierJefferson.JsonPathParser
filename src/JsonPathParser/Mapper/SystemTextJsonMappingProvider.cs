using System.Text.Json;
using System.Text.Json.Nodes;

namespace XavierJefferson.JsonPathParser.Mapper;

public class SystemTextJsonMappingProvider : MappingProviderBase
{
    protected override object? Deserialize(string s, Type t)
    {
        return JsonSerializer.Deserialize(s, t, new JsonSerializerOptions { AllowTrailingCommas = true, PropertyNameCaseInsensitive = true });
    }

    protected override object? MapToObject(object? source)
    {
        if (source is JsonArray array)
        {
            JpObjectList mapped;
            mapped = new JpObjectList(array.Select(MapToObject));
            return mapped;
        }

        if (source is JsonObject jObjectSource)
        {
            var mapped = new JpDictionary();
            foreach (var m in jObjectSource) mapped.Add(m.Key, MapToObject(m.Value));
            return mapped;
        }

        if (source == null)
            return null;
        return source;
    }

    protected override string Serialize(object? source)
    {
        return JsonSerializer.Serialize(source);
    }
}