using System.Collections;
using System.Collections.ObjectModel;
using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Filtering.ValueNodes;

public class ValueListNode : TypedValueNode<ICollection<ValueNode>>, IEnumerable<ValueNode>
{
    private readonly ReadOnlyCollection<ValueNode> _nodes = new(new ValueNode[] { });

    public ValueListNode(IJsonProvider jsonProvider, ICollection<object?>? values)
    {
        _nodes = new ReadOnlyCollection<ValueNode>(values.Select(value => ToValueNode(jsonProvider, value)).ToArray());
    }

    public override ICollection<ValueNode> Value => _nodes;

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
        if (o is ValueListNode valueListNode) return _nodes.Equals(valueListNode._nodes);

        return false;
    }
}