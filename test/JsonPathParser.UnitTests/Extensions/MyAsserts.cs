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
        var testAsList = toTest.ToList();
        if (Enumerable.SequenceEqual(toFind, testAsList))
        {
            return true;
        }
        if (toFind.Count != testAsList.Count) return false;
        for (var i = 0; i < toFind.Count; i++)
        {
            T? left = testAsList[i];
            T? right = toFind[i];
            if (left == null && right == null) continue;
            if (left == null || right == null) return false;
            switch (left.DeepCompare(right))
            {
                case DeepCompareResultEnum.NotEqual:
                    return false;
                case DeepCompareResultEnum.Equal:
                    continue;
            }
            if (!left.Equals((object?)right)) return false;
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