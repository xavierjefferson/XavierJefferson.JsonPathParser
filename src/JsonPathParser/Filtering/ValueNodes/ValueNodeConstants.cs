namespace XavierJefferson.JsonPathParser.Filtering.ValueNodes;

internal static class ValueNodeConstants
{
    public static NullNode NullNode = new();
    public static BooleanNode True = new("true");
    public static BooleanNode False = new("false");
    public static UndefinedNode Undefined = new();
}