using System.Text.RegularExpressions;
using XavierJefferson.JsonPathParser.Exceptions;
using XavierJefferson.JsonPathParser.Interfaces;
using XavierJefferson.JsonPathParser.Path;

namespace XavierJefferson.JsonPathParser.Filtering.ValueNodes;

public abstract class ValueNode


{
    public abstract Type Type(IPredicateContext context);

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

    private static bool TryCreatePath(object? instance, out IPath? path)
    {
        path = default;
        if (instance is string stringValue)
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

    private static bool TryCreateJsonNode(object? instance, IJsonProvider jsonProvider, out JsonNode? jsonNode)
    {
        jsonNode = default;
        if (instance is string stringValue)
        {
            var str = stringValue.Trim();
            if (str.Length <= 1) return false;
            var c0 = str.First();
            var c1 = str.Last();
            if ((c0 == '[' && c1 == ']') || (c0 == '{' && c1 == '}'))
                try
                {
                    var tmp = jsonProvider.Parse(str);
                    jsonNode = new JsonNode(tmp);
                    return true;
                }
                catch
                {
                    return false;
                }
        }

        return false;
    }

    protected static bool TryQuickCastNode(object? instance, bool willEscapeStringInstance,
        Func<string, ValueNode?>? stringHandler, out ValueNode? valueNode)
    {
        valueNode = default;
        switch (instance)
        {
            case bool booleanInstance:
                valueNode = CreateBooleanNode(booleanInstance);
                return true;
            case Regex regexInstance:
                valueNode = CreatePatternNode(regexInstance);
                return true;
            case DateTimeOffset dateTimeOffsetInstance:
                //workaround for issue: https://github.com/json-path/JsonPath/issues/613
                valueNode = CreateDateTimeOffsetNode(dateTimeOffsetInstance);
                return true;
            case char charInstance:
                valueNode = CreateStringNode(charInstance.ToString(), false);
                return true;
            case double nn:
                valueNode = new NumberNode(nn);
                return true;
            case int:
            case byte:
            case float:
            case decimal:
            case long:
                valueNode = new NumberNode(Convert.ToDouble(instance));
                return true;
            case string stringInstance:
            {
                if (stringHandler != null)
                {
                    valueNode = stringHandler(stringInstance);
                    if (valueNode != null) return true;
                }

                valueNode = CreateStringNode(stringInstance, willEscapeStringInstance);
                return true;
            }
            default:
                return false;
        }
    }

    //----------------------------------------------------
    //
    // Factory methods
    //
    //----------------------------------------------------
    public static ValueNode ToValueNode(IJsonProvider jsonProvider, object? o)
    {
        ValueNode StringHandler(string stringValue)
        {
            if (TryCreatePath(stringValue, out var newPath)) return new PathNode(newPath);

            if (TryCreateJsonNode(stringValue, jsonProvider, out var jsonNode)) return jsonNode;

            return null;
        }

        switch (o)
        {
            case null:
                return ValueNodeConstants.NullNode;
            case ValueNode valueNodeInstance:
                return valueNodeInstance;
            case ICollection<object?> objectCollectionInstance:
                return new ValueListNode(objectCollectionInstance, jsonProvider);
            case Type typeInstance:
                return CreateClassNode(typeInstance);
        }


        if (TryQuickCastNode(o, true, StringHandler, out var result)) return result;

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