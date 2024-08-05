namespace XavierJefferson.JsonPathParser.Path;

public class IndexedEnumerable<T>
{
    public IndexedEnumerable(T value, int index)
    {
        Value = value;
        Index = index;
        Index = index;
        Value = value;
    }

    public T Value { get; }
    public int Index { get; }
}