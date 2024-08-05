namespace XavierJefferson.JsonPathParser.UnitTests.Extensions;

public static class ObjectExtensions
{
    public static JpObjectList? AsList(this object? x)
    {
        return x as JpObjectList;
    }

    public static List<JpDictionary>? AsListOfMap(this object? x)
    {
        if (x is List<JpDictionary> existingList) return existingList;

        var t = x.AsList();

        return t?.OfType<JpDictionary>().ToList();
    }
}