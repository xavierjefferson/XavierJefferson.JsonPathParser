namespace XavierJefferson.JsonPathParser;

public class JpObjectList : List<object?>
{
    public JpObjectList()
    {
    }

    public JpObjectList(IEnumerable<object?> collection) : base(collection)
    {
    }
}