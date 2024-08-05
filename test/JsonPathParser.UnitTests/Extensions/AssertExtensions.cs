namespace XavierJefferson.JsonPathParser.UnitTests.Extensions;

public static class AssertExtensions
{
    public static void ContainsOnly<T>(this Assert actual, ICollection<T> collection, IEnumerable<T> list)
    {
        var l2 = list.ToList();
        Assert.Equal(l2.Distinct().Count(), collection.Distinct().Count());
        Assert.True(l2.All(collection.Contains));
    }

    public static bool ContainsEntry<T, TU>(this IDictionary<T, TU> dictionary, T key, TU value)
    {
        if (!dictionary.ContainsKey(key)) return false;
        var left = dictionary[key];
        var right = value;
        if (left == null && right == null) return true;
        if (left == null || right == null) return false;
        if (left.Equals(right)) return true;
        return false;
    }
}