using System.Text;
using XavierJefferson.JsonPathParser.Exceptions;
using XavierJefferson.JsonPathParser.Extensions;

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
        return new List<object?>();
    }


    public override object? CreateMap()
    {
        return new Dictionary<string, object?>();
    }
}