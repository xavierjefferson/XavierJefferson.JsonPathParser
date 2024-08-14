using XavierJefferson.JsonPathParser.Exceptions;
using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.PathRefs;

public abstract class PathRef : IComparable<PathRef>
{
    public static readonly PathRef NoOp = new NoOpPath();

    protected object? Parent;

    protected PathRef(object? parent)
    {
        Parent = parent;
    }

    public virtual int? Index { get; }


    public virtual int CompareTo(PathRef? o)
    {
        return GetAccessor().ToString().CompareTo(o.GetAccessor().ToString()) * -1;
    }

    public abstract void Put(string key, object newVal, Configuration configuration);

    protected abstract object? GetAccessor();

    public abstract void Set(object? newVal, Configuration configuration);

    public abstract void Convert(MapDelegate mapFunction, Configuration configuration);

    public abstract void Delete(Configuration configuration);

    public abstract void Add(object? newVal, Configuration configuration);

    public abstract void Add(string key, object? newVal, Configuration configuration);

    public abstract void RenameKey(string oldKey, string newKeyName, Configuration configuration);

    protected void RenameInMap(object? targetMap, string oldKeyName, string newKeyName, Configuration configuration)
    {
        if (configuration.JsonProvider.IsMap(targetMap))
        {
            if (configuration.JsonProvider.GetMapValue(targetMap, oldKeyName) == IJsonProvider.Undefined)
                throw new PathNotFoundException($"No results for Key {oldKeyName}" + " found in map!");
            configuration.JsonProvider.SetProperty(targetMap, newKeyName,
                configuration.JsonProvider.GetMapValue(targetMap, oldKeyName));
            configuration.JsonProvider.RemoveProperty(targetMap, oldKeyName);
        }
        else
        {
            throw new InvalidModificationException("Can only rename properties in a map");
        }
    }

    protected bool IsTargetInvalid(object? target)
    {
        return target == IJsonProvider.Undefined || target == null;
    }

    public static PathRef Create(object? obj, string property)
    {
        return new ObjectPropertyPathRef(obj, property);
    }

    public static PathRef Create(object? obj, ICollection<string> properties)
    {
        return new ObjectMultiPropertyPathRef(obj, properties);
    }

    public static PathRef Create(object? array, int index)
    {
        return new ArrayIndexPathRef(array, index);
    }

    public static PathRef CreateRoot(object? root)
    {
        return new RootPathRef(root);
    }
}