using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace XavierJefferson.JsonPathParser.Provider;

public class NewtonsoftJsonProvider : AbstractJsonProvider
{
    private readonly JsonSerializerSettings _settings = new();

    public NewtonsoftJsonProvider()
    {
    }

    public NewtonsoftJsonProvider(JsonSerializerSettings settings)
    {
        _settings = settings;
    }

    public override T Deserialize<T>(string obj)
    {
        return JsonConvert.DeserializeObject<T>(obj, _settings);
    }

    public override string Serialize(object? obj)
    {
        return JsonConvert.SerializeObject(obj, _settings);
    }


    public override object? Deserialize(string obj, Type type)
    {
        return JsonConvert.DeserializeObject(obj, type, _settings);
    }

    protected override object? Cleanup(object? value)
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