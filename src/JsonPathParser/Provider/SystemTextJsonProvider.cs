using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Nodes;
using XavierJefferson.JsonPathParser.Exceptions;
using XavierJefferson.JsonPathParser.Extensions;

namespace XavierJefferson.JsonPathParser.Provider;

public class SystemTextJsonProvider : AbstractJsonProvider
{

    public override T Deserialize<T>(string obj)
    {
        return JsonSerializer.Deserialize<T>(obj, _options);
    }
    public override object? Deserialize(string obj, Type type)
    {
        return JsonSerializer.Deserialize(obj, type, _options); 
    }
    public override string Serialize(object? obj)
    {
        return JsonSerializer.Serialize(obj, _options);
    }

    protected override object? Cleanup(object? value)
    {
        if (value == null) return null;

        if (value is JsonObject jsonObject)
        {
            var tmp = jsonObject.ToDictionary(i => i.Key, i => Cleanup(i.Value));
            return tmp;
        }

        if (value is JsonElement jsonElement)
        {
            switch (jsonElement.ValueKind)
            {
                case JsonValueKind.Array:
                    return new List<object?>(jsonElement.EnumerateArray().Select(i => Cleanup(i))).ToList();
                    break;
                case JsonValueKind.False:
                    return false;
                    break;
                case JsonValueKind.Null:
                    return null;
                    break;
                case JsonValueKind.Object:
                    return jsonElement.EnumerateObject().ToDictionary(i => i.Name, i => Cleanup(i.Value));
                    break;
                case JsonValueKind.Number:
                    return jsonElement.GetDouble();
                    break;
                case JsonValueKind.String:
                    return jsonElement.GetString();
                    break;
                case JsonValueKind.True:
                    return true;
                    break;
                default:
                    throw new NotImplementedException();
            }
            

        }
        if (value is JsonArray jsonArray) return jsonArray.Select(Cleanup).ToList();

        if (value is JsonNode jToken)
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

        if (value.GetType().IsPrimitive || value is string || value is DateTime)
            return value;
        throw new NotImplementedException();
    }

    JsonSerializerOptions _options = new JsonSerializerOptions { AllowTrailingCommas = true, PropertyNameCaseInsensitive=true };
    public SystemTextJsonProvider()
    {

    }
    public SystemTextJsonProvider(JsonSerializerOptions options)
    {
        _options = options;
    }


    
}