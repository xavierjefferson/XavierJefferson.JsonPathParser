using System.Collections;

namespace XavierJefferson.JsonPathParser;

public class JpDictionary : Dictionary<string, object?>
{
    public JpDictionary()
    {
    }

    public JpDictionary(IEqualityComparer<string> comparer) : base(comparer)
    {
    }

    public JpDictionary(Dictionary<string, object?> dictionary) : base(dictionary)
    {
    }

    public JpDictionary(Dictionary<string, object?> dictionary, IEqualityComparer<string> comparer) : base(dictionary,
        comparer)
    {
    }

    public JpDictionary(IEnumerable<KeyValuePair<string, object?>> kvp) : base(kvp)
    {
    }

    public override bool Equals(object? value)
    {
        if (value == null) return false;
        if (value == this) return true;
        var other = value as IDictionary;
        if (other == null) return false;
        if (other.Count != Count) return false;
        if (!other.Keys.Cast<object>().All(Keys.Contains)) return false;
        if (!Keys.All(other.Keys.Cast<object>().Contains)) return false;
        foreach (var key in Keys)
        {
            var thisValue = this[key];
            var otherValue = other[key];
            if (thisValue == otherValue) continue;
            if (thisValue == null || otherValue == null)
            {
                return false;
            }
            if (!thisValue.Equals(otherValue)) return false;
        }

        return true;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}