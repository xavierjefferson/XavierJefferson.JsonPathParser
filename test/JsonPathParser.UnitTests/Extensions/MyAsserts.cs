using XavierJefferson.JsonPathParser.Extensions;

namespace XavierJefferson.JsonPathParser.UnitTests.Extensions;

public static class MyAsserts
{
    public static bool ContainsAll<T>(this IEnumerable<T> toTest, params T[] toFind)
    {
        return ContainsAll(toTest, toFind.ToList());
    }

    public static bool ContainsAll<T>(this IEnumerable<T> toTest, IList<T> toFind)
    {
        var m = toTest.ToList();
        foreach (var x in toFind)
        {
            var found = m.Any(i => (i == null && x == null) || (i != null && x != null) || i.Equals(x));
            if (!found) return false;
        }

        return true;
    }

    public static bool ContainsExactly<T>(this IEnumerable<T> toTest, params T[] toFind)
    {
        return ContainsExactly(toTest, toFind.ToList());
    }

    public static bool ContainsExactly<T>(this IEnumerable<T> toTest, IList<T> toFind)
    {
        var m = toTest.ToList();
        if (toFind.Count != m.Count) return false;
        for (var i = 0; i < toFind.Count; i++)
        {
            if (m[i] == null && toFind[i] == null) continue;
            if (m[i] == null || toFind[i] == null) return false;
            if (m[i] is IDictionary<string, object?> a && toFind[i] is IDictionary<string, object?> b)
                return a.DeepEquals(b);
            if (!m[i].Equals(toFind[i])) return false;
        }

        return true;
    }

    public static bool ContainsOnly<T>(this IEnumerable<T> toTest, params T[] toFind)
    {
        return ContainsOnly(toTest, toFind.ToList());
    }

    public static bool ContainsOnly<T>(this IEnumerable<T> toTest, IList<T> toFind)
    {
        var k = toTest.ToArray();
        var f = k.ContainsAll(toFind);
        if (f == false) return false;
        return toFind.ContainsAll(k);
    }
}