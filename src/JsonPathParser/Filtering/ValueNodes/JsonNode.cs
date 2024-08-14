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

    public override Type Type(IPredicateContext ctx)
    {
        if (IsArray(ctx)) return TypeConstants.ListType;

        if (IsMap(ctx)) return TypeConstants.DictionaryType;

        var parsed = Parse(ctx);
        if (parsed is double) return typeof(double);
        if (parsed is string) return typeof(string);
        if (parsed is bool) return typeof(bool);
        return typeof(void);
    }

    public override JsonNode AsJsonNode()
    {
        return this;
    }

    public ValueNode AsValueListNode(IPredicateContext ctx)
    {
        if (!IsArray(ctx))
            return ValueNodeConstants.Undefined;
        return new ValueListNode(Parse(ctx) as ICollection<object?>);
    }

    public object? Parse(IPredicateContext ctx)
    {
        if (_parsed) return _value;
        var p = ctx.Configuration.JsonProvider;
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

    public bool IsArray(IPredicateContext ctx)
    {
        return Parse(ctx) is IList;
    }

    public bool IsMap(IPredicateContext ctx)
    {
        return Parse(ctx) is IDictionary;
    }

    public int Length(IPredicateContext ctx)
    {
        return IsArray(ctx) ? ((ICollection<object>)Parse(ctx)).Count() : -1;
    }

    public bool IsEmpty(IPredicateContext ctx)
    {
        if (IsArray(ctx) || IsMap(ctx)) return ((ICollection<object>)Parse(ctx)).Count() == 0;
        if (Parse(ctx) is string) return ((string)Parse(ctx)).Length == 0;
        return true;
    }


    public override string ToString()
    {
        return _value.ToString();
    }

    public bool Equals(JsonNode jsonNode, IPredicateContext ctx)
    {
        if (this == jsonNode) return true;
        if (_value != null)
        {
            var c = jsonNode.Parse(ctx);
            if (c == null) return false;
            if (c is IList a && _value is IList b)
            {
                if (a.Count != b.Count) return false;
                for (var i = 0; i < a.Count; i++)
                {
                    var left = a is ValueNode ? (ValueNode)a[i] : ToValueNode(a[i]);
                    var right = b is ValueNode ? (ValueNode)b[i] : ToValueNode(b[i]);


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