using System.Text.RegularExpressions;
using Newtonsoft.Json;
using XavierJefferson.JsonPathParser.Exceptions;
using XavierJefferson.JsonPathParser.Interfaces;
using XavierJefferson.JsonPathParser.Path;

namespace XavierJefferson.JsonPathParser.Filtering.ValueNodes;

public abstract class ValueNode


{
    public abstract Type Type(IPredicateContext ctx);

    public virtual PatternNode AsPatternNode()
    {
        throw new InvalidPathException($"Expected {nameof(PatternNode)}");
    }

    public virtual PathNode AsPathNode()
    {
        throw new InvalidPathException($"Expected {nameof(PathNode)}");
    }

    public virtual NumberNode AsNumberNode()
    {
        throw new InvalidPathException($"Expected {nameof(NumberNode)}");
    }

    public virtual StringNode AsStringNode()
    {
        throw new InvalidPathException($"Expected {nameof(StringNode)}");
    }

    public virtual BooleanNode AsBooleanNode()
    {
        throw new InvalidPathException($"Expected {nameof(BooleanNode)}");
    }

    public virtual JsonNode AsJsonNode()
    {
        throw new InvalidPathException($"Expected {nameof(JsonNode)}");
    }

    public virtual PredicateNode AsPredicateNode()
    {
        throw new InvalidPathException($"Expected {nameof(PredicateNode)}");
    }

    public virtual ValueListNode AsValueListNode()
    {
        throw new InvalidPathException($"Expected {nameof(ValueListNode)}");
    }

    public virtual NullNode AsNullNode()
    {
        throw new InvalidPathException($"Expected {nameof(NullNode)}");
    }

    public virtual UndefinedNode AsUndefinedNode()
    {
        throw new InvalidPathException($"Expected {nameof(UndefinedNode)}");
    }

    public virtual ClassNode AsClassNode()
    {
        throw new InvalidPathException($"Expected {nameof(ClassNode)}");
    }

    //workaround for issue: https://github.com/json-path/JsonPath/issues/613

    public virtual DateTimeOffsetNode AsDateTimeOffsetNode()
    {
        throw new InvalidPathException($"Expected {nameof(DateTimeOffsetNode)}");
    }

    private static bool TryCreatePath(object? o, out IPath? path)
    {
        path = null;
        if (o != null && o is string stringValue)
        {
            var str = stringValue.Trim();
            if (str.Length <= 0) return false;
            var c0 = str[0];
            if (c0 == '@' || c0 == '$')
                try
                {
                    path = PathCompiler.Compile(str);
                    return true;
                }
                catch
                {
                    return false;
                }
        }

        return false;
    }

    private static bool IsJson(object? o)
    {
        if (o != null && o is string stringValue)
        {
            var str = stringValue.Trim();
            if (str.Length <= 1) return false;
            var c0 = str.First();
            var c1 = str.Last();
            if ((c0 == '[' && c1 == ']') || (c0 == '{' && c1 == '}'))
                try
                {
                    JsonConvert.DeserializeObject(str);
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
        }

        return false;
    }

    public static bool IsNumeric(object? o)
    {
        return o is int || o is byte || o is float || o is decimal || o is double || o is long;
    }

    //----------------------------------------------------
    //
    // Factory methods
    //
    //----------------------------------------------------
    public static ValueNode ToValueNode(object? o)
    {
        if (o == null) return ValueNodeConstants.NullNode;
        if (o is ValueNode) return (ValueNode)o;
        if (o is ICollection<object?> z) return new ValueListNode(z);
        if (o is Type) return CreateClassNode((Type)o);

        if (TryCreatePath(o, out var newPath)) return new PathNode(newPath);

        if (IsJson(o)) return CreateJsonNode(o.ToString());

        if (o is string stringValue) return CreateStringNode(stringValue, true);

        if (o is char) return CreateStringNode(o.ToString(), false);

        if (IsNumeric(o))
        {
            if (o is double nn) return new NumberNode(nn);
            return new NumberNode(Convert.ToDouble(o));
        }

        if (o is bool booleanValue)
            return CreateBooleanNode(booleanValue);
        if (o is Regex regexValue)
            return CreatePatternNode(regexValue);
        if (o is DateTimeOffset dx)
            return CreateDateTimeOffsetNode(
                dx); //workaround for issue: https://github.com/json-path/JsonPath/issues/613
        throw new JsonPathException("Could not determine value type");
    }

    public static StringNode CreateStringNode(string charSequence, bool escape)
    {
        return new StringNode(charSequence, escape);
    }

    public static ClassNode CreateClassNode(Type type)
    {
        return new ClassNode(type);
    }

    public static NumberNode CreateNumberNode(double charSequence)
    {
        return new NumberNode(charSequence);
    }

    public static NumberNode CreateNumberNode(string charSequence)
    {
        return new NumberNode(charSequence);
    }

    public static BooleanNode CreateBooleanNode(string charSequence)
    {
        return bool.Parse(charSequence) ? ValueNodeConstants.True : ValueNodeConstants.False;
    }

    public static BooleanNode CreateBooleanNode(bool input)
    {
        return input ? ValueNodeConstants.True : ValueNodeConstants.False;
    }

    public static NullNode CreateNullNode()
    {
        return ValueNodeConstants.NullNode;
    }

    public static JsonNode CreateJsonNode(string json)
    {
        return new JsonNode(json);
    }

    public static JsonNode CreateJsonNode(object? parsedJson)
    {
        return new JsonNode(parsedJson);
    }

    public static PatternNode CreatePatternNode(string pattern)
    {
        return new PatternNode(pattern);
    }

    public static PatternNode CreatePatternNode(Regex pattern)
    {
        return new PatternNode(pattern);
    }

    //workaround for issue: https://github.com/json-path/JsonPath/issues/613
    public static DateTimeOffsetNode CreateDateTimeOffsetNode(string charSequence)
    {
        return new DateTimeOffsetNode(charSequence);
    }

    public static DateTimeOffsetNode CreateDateTimeOffsetNode(DateTimeOffset charSequence)
    {
        return new DateTimeOffsetNode(charSequence);
    }

    public static UndefinedNode CreateUndefinedNode()
    {
        return ValueNodeConstants.Undefined;
    }

    public static PathNode CreatePathNode(string path, bool existsCheck, bool shouldExists)
    {
        return new PathNode(path, existsCheck, shouldExists);
    }

    public static ValueNode CreatePathNode(IPath path)
    {
        return new PathNode(path);
    }
}