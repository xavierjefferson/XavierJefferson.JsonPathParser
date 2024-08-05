using XavierJefferson.JsonPathParser.Helpers;
using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Filtering.ValueNodes;

public class StringNode : TypedValueNode<string>
{
    private readonly string? _value;
    private readonly bool _useSingleQuote = true;

    public override string Value => _value;

    public StringNode(string? value, bool escape)
    {
        if (escape && value!=null && value.Length > 1)
        {
            var open = value.First();
            var close = value.Last();
            if (open == '\'' && close == '\'')
            {
                value = value.Subsequence(1, value.Length - 1);
            }
            else if (open == '"' && close == '"')
            {
                value = value.Subsequence(1, value.Length - 1);
                _useSingleQuote = false;
            }

            _value = StringHelper.Unescape(value);
        }
        else
        {
            _value = value;
        }
    }

    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }

    public override NumberNode AsNumberNode()
    {
        if (double.TryParse(_value, out var number)) return new NumberNode(number);
        return NumberNode.Nan;
    }

    public int Length()
    {
        return _value.Length;
    }

    public bool IsEmpty()
    {
        return string.IsNullOrWhiteSpace(_value);
    }

    public bool Contains(string str)
    {
        return _value.Contains(str);
    }


    public override StringNode AsStringNode()
    {
        return this;
    }


    public override string ToString()
    {
        var quote = _useSingleQuote ? "'" : "\"";
        return quote + StringHelper.Escape(_value, true) + quote;
    }


    public override bool Equals(object? o)
    {
        if (this == o) return true;
        if (o is ValueNode valueNode)
        {
            if (valueNode is StringNode || valueNode is NumberNode)
            {
                var that = valueNode.AsStringNode();

                if (that._value == null && _value == null)
                {
                    return true;
                }
                else
                {
                    return _value.Equals(that.Value);
                }
            }
        }

        return false;
    }
}