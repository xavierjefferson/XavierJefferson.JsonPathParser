namespace XavierJefferson.JsonPathParser.Extensions;
 public enum DeepCompareResultEnum
    {
        Invalid,
        Equal,
        NotEqual,
    }
public static class DictionaryExtensions
{
   
    public static DeepCompareResultEnum DeepCompare(this object x, object y)
    {
        if (x is IDictionary<string, object?> a && y is IDictionary<string, object?> b)
        {
            var equal = a.DeepCompare(b);
            if (equal) return DeepCompareResultEnum.Equal;
            return DeepCompareResultEnum.NotEqual;
        }
        else
            return DeepCompareResultEnum.Invalid;
    }
    private static bool DeepCompare(this IDictionary<string, object?> x, IDictionary<string, object?> y)
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

                var test = DeepCompare(thisValue, otherValue);
                switch (test)
                {
                    case DeepCompareResultEnum.NotEqual:
                        return false;
                    case DeepCompareResultEnum.Equal:
                        continue;
                }
                if (!thisValue.Equals(otherValue)) return false;
            }

            return true;
        }

        return false;
    }
}