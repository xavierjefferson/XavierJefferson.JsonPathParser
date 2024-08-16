namespace XavierJefferson.JsonPathParser.Extensions;

public static class DictionaryExtensions
{
    public static bool DeepEquals(this IDictionary<string, object?> x, IDictionary<string, object?> y)
    {
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (ReferenceEquals(x, y)) return true;

        // Check whether the dictionaries are equal
        if (x.Count == y.Count)
        {
            if (!x.Keys.All(y.Keys.Contains)) return false;
            if (!y.Keys.All(x.Keys.Contains)) return false;
            foreach (var key in y.Keys)
            {
                var thisValue = y[key];
                var otherValue = x[key];
                if (thisValue == otherValue) continue;
                if (thisValue == null || otherValue == null) return false;

                if (thisValue is IDictionary<string, object?> a && otherValue is IDictionary<string, object?> b)
                    return a.DeepEquals(b);
                if (!thisValue.Equals(otherValue)) return false;
            }

            return true;
        }

        return false;
    }
}