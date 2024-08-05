using System.Collections;
using System.ComponentModel;
using XavierJefferson.JsonPathParser.Exceptions;
using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Mapper;

public abstract class MappingProviderBase : IMappingProvider
{
    protected abstract object? MapToObject(object? source);

    protected abstract string Serialize(object? source);
    protected abstract object? Deserialize(string s, Type t);
    public object? Map(object? source, Type targetType, Configuration configuration)
    {
        if (source == null) return null;

        //if (source is double b)
        //{
        //    if (b.TryConvertTo(targetType, out object zz))
        //    {
        //        return zz;
        //    }
        //}
        if (source.GetType() == targetType) return source;
        if (source is double || source is int || source is byte || source is long || source is float || source is decimal || source is string)
        {
            if (targetType == typeof(long))
            {
                return Convert.ToInt64(source);
            }
            else if (targetType == typeof(int))
            {
                return Convert.ToInt32(source);
            }
            else if (targetType == typeof(byte))
            {
                return Convert.ToByte(source);
            }
            else if (targetType == typeof(float))
            {
                return Convert.ToSingle(source);
            }
            else if (targetType == typeof(double))
            {
                return Convert.ToDouble(source);
            }
            else if (targetType == typeof(decimal))
            {
                return Convert.ToDecimal(source);
            }
        }
        if (targetType.Equals(typeof(object)) || targetType.Equals(typeof(IList)) ||
            targetType.Equals(typeof(IDictionary))) return MapToObject(source);
        var c = new TypeConverter();
        try
        {
            return c.ConvertTo(source, targetType);
        }
        catch (Exception ex)
        {
            try
            {
                try
                {
                    var a = Serialize(source); var b = Deserialize(a, targetType);
                    return b;
                }
                catch (NotSupportedException nes)
                {
                    return source;
                }

               
            }
            catch (Exception ey)
            {
                throw new MappingException(ey);
            }


            //return source;
        }
    }
    public object? Map<T>(object? source, Configuration configuration)
    {
        return Map(source, typeof(T), configuration);
    }
}