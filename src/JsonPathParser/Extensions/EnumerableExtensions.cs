using XavierJefferson.JsonPathParser.Path;

namespace XavierJefferson.JsonPathParser.Extensions;

public static class EnumerableExtensions
{
    public static IEnumerable<IndexedEnumerable<T?>> ToIndexedEnumerable<T>(this IEnumerable<T?> enumerable)
    {
        return enumerable.Select((i, j) => new IndexedEnumerable<T?>(i, j));
    }

    public static bool ContainsAll<T>(this IEnumerable<T?> enumerable, IEnumerable<T?> collection)
    {
        return collection.All(enumerable.Contains);
    }
}