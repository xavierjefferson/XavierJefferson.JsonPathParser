using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using XavierJefferson.JsonPathParser.Exceptions;
using XavierJefferson.JsonPathParser.Extensions;

namespace XavierJefferson.JsonPathParser.Provider;

public class NewtonsoftJsonProvider : ConvertingJsonProviderBase
{
    private static readonly JsonSerializerSettings Settings = GetSettings();

    private static JsonSerializerSettings GetSettings()
    {
        var settings = new JsonSerializerSettings { Formatting = Formatting.None };
        return settings;
    }

    public override string ToJson(object? obj)
    {
        return JsonConvert.SerializeObject(obj, Settings);
    }

    public override object? Parse(string json)
    {
        try
        {
            var beauty = StringExtensions.Beautify(json);
            return Cleanup(JsonConvert.DeserializeObject(beauty, Settings));
        }
        catch (Exception e)
        {
            throw new InvalidJsonException(e);
        }
    }


    private object? Cleanup(object? value)
    {
        if (value == null) return null;

        if (value is JObject jObject)
        {
            var resultDictionary = new Dictionary<string, object?>();
            foreach (var jProperty in jObject.Properties())
                if (jObject.TryGetValue(jProperty.Name, out var jToken0))
                    resultDictionary[jProperty.Name] = Cleanup(jToken0);
            return resultDictionary;
        }

        if (value is JArray jArray) return jArray.Select(Cleanup).ToList();

        if (value is JToken jToken)
            switch (jToken.Type)
            {
                case JTokenType.Array:
                    return jToken.Value<JArray>();
                case JTokenType.Boolean:
                    return jToken.Value<bool>();
                case JTokenType.String:
                    return jToken.Value<string>();
                case JTokenType.Integer:
                    return Convert.ToDouble(jToken.Value<long>());
                //return new IntegerNumber(jToken.Value<long>());
                case JTokenType.Float:
                    return jToken.Value<double>();
                case JTokenType.Null:
                    return null;
                case JTokenType.Date:
                    return jToken.Value<DateTime>();
                case JTokenType.Bytes:
                    return jToken.Value<byte[]>();
                case JTokenType.Object:
                    return jToken.Value<JObject>();
                case JTokenType.TimeSpan:
                    return jToken.Value<TimeSpan>();
                default:
                    throw new NotImplementedException();
            }

        if (value.GetType().IsPrimitive || value is string || value is DateTime)
            return value;
        throw new NotImplementedException();
    }
}