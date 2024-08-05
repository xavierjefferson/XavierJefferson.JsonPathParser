using XavierJefferson.JsonPathParser.Exceptions;

namespace XavierJefferson.JsonPathParser.PathRefs;

public class ArrayIndexPathRef : PathRef
{
    private readonly int _index;

    public ArrayIndexPathRef(object? parent, int index) : base(parent)
    {
        _index = index;
    }

    public override int? Index => _index;

    public override void Put(string key, object? value, Configuration configuration)
    {
        var target = configuration.JsonProvider.GetArrayIndex(Parent, _index);
        if (IsTargetInvalid(target)) return;
        if (configuration.JsonProvider.IsMap(target))
            configuration.JsonProvider.SetProperty(target, key, value);
        else
            throw new InvalidModificationException("Can only add properties to a map");
    }

    public override void Set(object? newVal, Configuration configuration)
    {
        configuration.JsonProvider.SetArrayIndex(Parent, _index, newVal);
    }

    public override void Convert(MapDelegate mapFunction, Configuration configuration)
    {
        var currentValue = configuration.JsonProvider.GetArrayIndex(Parent, _index);
        configuration.JsonProvider.SetArrayIndex(Parent, _index, mapFunction(currentValue, configuration));
    }

    public override void Delete(Configuration configuration)
    {
        configuration.JsonProvider.RemoveProperty(Parent, _index);
    }

    public override void Add(object? value, Configuration configuration)
    {
        var target = configuration.JsonProvider.GetArrayIndex(Parent, _index);
        if (IsTargetInvalid(target)) return;
        if (configuration.JsonProvider.IsArray(target))
            configuration.JsonProvider.SetProperty(target, null, value);
        else
            throw new InvalidModificationException("Can only.Add to an array");
    }

    public override void Add(string key, object? value, Configuration configuration)
    {
        var target = configuration.JsonProvider.GetArrayIndex(Parent, _index);
        if (IsTargetInvalid(target)) return;
        if (configuration.JsonProvider.IsMap(target))
            configuration.JsonProvider.SetProperty(target, key, value);
        else
            throw new InvalidModificationException("Can only.Add properties to a map");
    }


    public override void RenameKey(string oldKeyName, string newKeyName, Configuration configuration)
    {
        var target = configuration.JsonProvider.GetArrayIndex(Parent, _index);
        if (IsTargetInvalid(target)) return;
        RenameInMap(target, oldKeyName, newKeyName, configuration);
    }


    protected override object? GetAccessor()
    {
        return _index;
    }


    public override int CompareTo(PathRef? o)
    {
        if (o is ArrayIndexPathRef pf) return pf._index.CompareTo(_index);
        return base.CompareTo(o);
    }
}