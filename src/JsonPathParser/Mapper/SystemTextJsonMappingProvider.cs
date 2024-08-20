using System.Text.Json.Nodes;

namespace XavierJefferson.JsonPathParser.Mapper;

public class SystemTextJsonMappingProvider : MappingProviderBase
{
    protected override object? MapToObject(object? source)
    {
        if (source is JsonArray array) return array.Select(MapToObject).ToList();

        if (source is JsonObject jObjectSource)
        {
            var mapped = new Dictionary<string, object?>();
            foreach (var m in jObjectSource) mapped.Add(m.Key, MapToObject(m.Value));
            return mapped;
        }

        if (source == null)
            return null;
        return source;
    }
}