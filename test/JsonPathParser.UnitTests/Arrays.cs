namespace XavierJefferson.JsonPathParser.UnitTests;

public class Arrays
{
    public static JpObjectList AsList(params object[] array)
    {
        return new JpObjectList(array);
    }

    public static List<T> NewArrayList<T>(params T[] p)
    {
        return new List<T>(p);
    }
}