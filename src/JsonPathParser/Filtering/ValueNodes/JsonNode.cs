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
        return new ValueListNode(Parse(context) as ICollection<object?>, context.Configuration.JsonProvider);
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
                    var right = rightList[i] as ValueNode ?? ToValueNode(context.Configuration.JsonProvider, rightList[i]);

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
        if (!(o is JsonNode jsonNode)) return false;

        return _value?.Equals(jsonNode._value) ?? jsonNode._value == null;
    }
}