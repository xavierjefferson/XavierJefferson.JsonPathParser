using System.Text.Json;
using System.Text.Json.Nodes;
using XavierJefferson.JsonPathParser.Exceptions;
using XavierJefferson.JsonPathParser.Extensions;

namespace XavierJefferson.JsonPathParser.Provider;

public class SystemTextJsonProvider : ConvertingJsonProviderBase
{
    public override string ToJson(object? obj)
    {
        return JsonSerializer.Serialize(obj);
    }

    private object? Cleanup(object? x)
    {
        if (x == null) return null;

        if (x is JsonObject jx)
        {
            var d1 = new JpDictionary();

            foreach (var nn in jx) d1[nn.Key] = Cleanup(nn.Value);
            return d1;
        }

        if (x is JsonArray jz)
        {
            var r = new JpObjectList();
            foreach (var nn in jz) r.Add(Cleanup(nn));
            return r;
        }

        if (x is JsonNode jToken)
            switch (jToken.GetValueKind())
            {
                case JsonValueKind.String:
                    return jToken.GetValue<string>();
                case JsonValueKind.Number:
                    return jToken.GetValue<double>();
                case JsonValueKind.True:
                    return true;
                case JsonValueKind.False:
                    return false;
                case JsonValueKind.Object:
                    return jToken;
                case JsonValueKind.Null:
                    return null;
                case JsonValueKind.Array:
                    return jToken;
                default:
                    throw new NotImplementedException();
            }

        if (x.GetType().IsPrimitive || x is string || x is DateTime)
            return x;
        throw new NotImplementedException();
    }

    public override object? Parse(string json)
    {
        try
        {
            string? json1 = StringExtensions.Beautify(json);
            return Cleanup(JsonNode.Parse(json1, new JsonNodeOptions { PropertyNameCaseInsensitive = true })); ;
        }
        catch (Exception e)
        {
            throw new InvalidJsonException(e);
        }
    }
}