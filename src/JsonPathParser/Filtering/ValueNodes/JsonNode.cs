using System.Collections;
using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Filtering.ValueNodes;

public class JsonNode : TypedValueNode<object?>
{
    private static readonly HashSet<Type> ReturnedTypes = new()
    {
        typeof(string), typeof(double), typeof(DateTime), typeof(DateTimeOffset), typeof(bool)
    };

    private readonly bool _parsed;
    private readonly object? _value;

    public JsonNode(string jsonString)
    {
        _value = jsonString;
        _parsed = false;
    }

    public JsonNode(object? parsedJson)
    {
        _value = parsedJson;
        _parsed = true;
    }

    public override object? Value => _value;

    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }

    public override Type Type(IPredicateContext context)
    {
        if (IsArray(context)) return TypeConstants.ListType;

        if (IsMap(context)) return TypeConstants.DictionaryType;

        var parsed = Parse(context);
        if (parsed != null && ReturnedTypes.Contains(parsed.GetType())) return parsed.GetType();

        return typeof(void);
    }

    public override JsonNode AsJsonNode()
    {
        return this;
    }

    public ValueNode AsValueListNode(IPredicateContext context)
    {
        if (TryCastAsArray(context, out var list))
            return new ValueListNode(context.Configuration.JsonProvider, list.Cast<object?>().ToList());
        return ValueNodeConstants.Undefined;
    }

    public object? Parse(IPredicateContext context)
    {
        if (_parsed) return _value;
        var jsonProvider = context.Configuration.JsonProvider;
        return jsonProvider.Parse(_value.ToString());
    }

    public bool IsParsed()
    {
        return _parsed;
    }

    [Obsolete]
    public object? GetJson()
    {
        return _value;
    }

    public bool IsArray(IPredicateContext context)
    {
        return TryCastAsArray(context, out _);
    }

    public bool IsMap(IPredicateContext context)
    {
        return TryCastAsDictionary(context, out _);
    }

    public int Length(IPredicateContext context)
    {
        if (TryCastAsArray(context, out var list)) return list.Count;

        return -1;
    }

    private bool TryCastAsArray(IPredicateContext context, out IList list)
    {
        list = default;
        if (Parse(context) is IList tmp)
        {
            list = tmp;
            return true;
        }

        return false;
    }

    private bool TryCastAsDictionary(IPredicateContext context, out IDictionary dictionary)
    {
        dictionary = default;
        if (Parse(context) is IDictionary tmp)
        {
            dictionary = tmp;
            return true;
        }

        return false;
    }

    public bool IsEmpty(IPredicateContext context)
    {
        if (TryCastAsArray(context, out var list)) return list.Count == 0;
        if (TryCastAsDictionary(context, out var dictionary)) return dictionary.Count == 0;
        if (Parse(context) is string stringInstance) return string.IsNullOrEmpty(stringInstance);
        return true;
    }


    public override string ToString()
    {
        return _value.ToString();
    }

    public bool Equals(JsonNode jsonNode, IPredicateContext context)
    {
        if (this == jsonNode) return true;
        if (_value != null)
        {
            var leftNode = jsonNode.Parse(context);
            if (leftNode == null) return false;
            if (leftNode is IList leftList && _value is IList rightList)
            {
                if (leftList.Count != rightList.Count) return false;
                for (var i = 0; i < leftList.Count; i++)
                {
                    var left = leftList[i] as ValueNode ?? ToValueNode(context.Configuration.JsonProvider, leftList[i]);
                    var right = rightList[i] as ValueNode ??
                                ToValueNode(context.Configuration.JsonProvider, rightList[i]);

                    if (!left.Equals(right)) return false;
                }

                return true;
            }

            return _value.Equals(leftNode);
        }

        return jsonNode._value == null;
    }


    public override bool Equals(object? o)
    {
        if (this == o) return true;
        if (o is JsonNode jsonNode) return _value?.Equals(jsonNode._value) ?? jsonNode._value == null;
        return false;
    }
}