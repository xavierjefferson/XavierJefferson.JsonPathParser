using System.Collections;

namespace XavierJefferson.JsonPathParser;

public class ReadOnlySet<T> : IReadOnlySet<T>
{
    private readonly ISet<T> _set;

    public ReadOnlySet(ISet<T> set)
    {
        ArgumentNullException.ThrowIfNull(set);
        _set = set;
    }

    public ReadOnlySet(IEnumerable<T> collection) : this(new HashSet<T>(collection))
    {
    }

    public int Count => _set.Count;

    public bool Contains(T item)
    {
        return _set.Contains(item);
    }

    public bool IsProperSubsetOf(IEnumerable<T> other)
    {
        return _set.IsProperSubsetOf(other);
    }

    public bool IsProperSupersetOf(IEnumerable<T> other)
    {
        return _set.IsProperSupersetOf(other);
    }

    public bool IsSubsetOf(IEnumerable<T> other)
    {
        return _set.IsSubsetOf(other);
    }

    public bool IsSupersetOf(IEnumerable<T> other)
    {
        return _set.IsSupersetOf(other);
    }

    public bool Overlaps(IEnumerable<T> other)
    {
        return _set.Overlaps(other);
    }

    public bool SetEquals(IEnumerable<T> other)
    {
        return _set.SetEquals(other);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _set.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}