using System.Collections;
using System.Collections.ObjectModel;

namespace XavierJefferson.JsonPathParser.Filtering.ValueNodes;

public class ValueListNode : TypedValueNode<ICollection<ValueNode>>, IEnumerable<ValueNode>
{
    private readonly SerializingList<ValueNode> _nodes = new();

    public ValueListNode(ICollection<object?>? values)
    {
        foreach (var value in values) _nodes.Add(ToValueNode(value));
    }

    public override ICollection<ValueNode> Value => new ReadOnlyCollection<ValueNode>(_nodes);

    public IEnumerator<ValueNode> GetEnumerator()
    {
        return _nodes.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _nodes.GetEnumerator();
    }

    public bool Contains(ValueNode node)
    {
        return _nodes.Contains(node);
    }

    public bool Subsetof(ValueListNode right)
    {
        return _nodes.All(leftNode => right._nodes.Contains(leftNode));
    }

    [Obsolete]
    public ICollection<ValueNode> GetNodes()
    {
        return new ReadOnlyCollection<ValueNode>(_nodes);
    }


    public override int GetHashCode()
    {
        return _nodes.GetHashCode();
    }

    public override ValueListNode AsValueListNode()
    {
        return this;
    }


    public override string ToString()
    {
        return "[" + string.Join(",", _nodes) + "]";
    }


    public override bool Equals(object? o)
    {
        if (this == o) return true;
        if (!(o is ValueListNode)) return false;

        var that = (ValueListNode)o;

        return _nodes.Equals(that._nodes);
    }
}