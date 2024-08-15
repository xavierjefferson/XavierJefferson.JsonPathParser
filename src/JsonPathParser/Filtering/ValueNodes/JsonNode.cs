using System.Collections;
using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Filtering.ValueNodes;

public class JsonNode : TypedValueNode<object?>
{
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
        if (parsed is double) return typeof(double);
        if (parsed is string) return typeof(string);
        if (parsed is bool) return typeof(bool);
        return typeof(void);
    }

    public override JsonNode AsJsonNode()
    {
        return this;
    }

    public ValueNode AsValueListNode(IPredicateContext context)
    {
        if (!IsArray(context))
            return ValueNodeConstants.Undefined;
        return new ValueListNode(Parse(context) as ICollection<object?>);
    }

    public object? Parse(IPredicateContext context)
    {
        if (_parsed) return _value;
        var p = context.Configuration.JsonProvider;
        return p.Parse(_value.ToString());
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
        return Parse(context) is IList;
    }

    public bool IsMap(IPredicateContext context)
    {
        return Parse(context) is IDictionary;
    }

    public int Length(IPredicateContext context)
    {
        return IsArray(context) ? ((ICollection<object>)Parse(context)).Count() : -1;
    }

    public bool IsEmpty(IPredicateContext context)
    {
        if (IsArray(context) || IsMap(context)) return ((ICollection<object>)Parse(context)).Count() == 0;
        if (Parse(context) is string) return ((string)Parse(context)).Length == 0;
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
            var c = jsonNode.Parse(context);
            if (c == null) return false;
            if (c is IList a && _value is IList b)
            {
                if (a.Count != b.Count) return false;
                for (var i = 0; i < a.Count; i++)
                {                   
                    var left = a[i] is ValueNode valueNodeA ? valueNodeA : ToValueNode(a[i]);
                    var right = b[i] is ValueNode valueNodeB ? valueNodeB: ToValueNode(b[i]);

                    if (!left.Equals(right)) return false;
                }

                return true;
            }

            return _value.Equals(c);
        }

        return jsonNode._value == null;
    }


    public override bool Equals(object? o)
    {
        if (this == o) return true;
        if (!(o is JsonNode)) return false;

        var jsonNode = (JsonNode)o;

        return !(_value != null ? !_value.Equals(jsonNode._value) : jsonNode._value != null);
    }
}