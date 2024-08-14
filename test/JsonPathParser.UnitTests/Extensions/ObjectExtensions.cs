namespace XavierJefferson.JsonPathParser.UnitTests.Extensions;

public static class ObjectExtensions
{
    public static List<object?>? AsList(this object? x)
    {
        return x as List<object?>;
    }

    public static List<IDictionary<string, object?>>? AsListOfMap(this object? x)
    {
        if (x is List<IDictionary<string, object?>> existingList) return existingList;

        var t = x.AsList();

        return t?.OfType<IDictionary<string, object?>>().ToList();
    }
}