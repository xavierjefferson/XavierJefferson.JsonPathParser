using System.Text;
using XavierJefferson.JsonPathParser.Extensions;

namespace XavierJefferson.JsonPathParser;

public class SerializingList<T> : List<T>
{
    public SerializingList()
    {
    }

    public SerializingList(IEnumerable<T> items) : base(items)
    {
    }

    public SerializingList(int count) : base(count)
    {
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        // sb.Append("[");
        foreach (var item in this.ToIndexedEnumerable())
        {
            if (item.Index > 0) sb.Append(",");

            if (item.Value is string)
                sb.Append($"{item.Value}");
            else
                sb.Append(item.Value);
        }

        //sb.Append("]");
        return sb.ToString();
    }
}