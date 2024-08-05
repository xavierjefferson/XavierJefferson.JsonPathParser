using XavierJefferson.JsonPathParser.Exceptions;

namespace XavierJefferson.JsonPathParser.PathRefs;

public class ObjectPropertyPathRef : PathRef
{
    private readonly string _property;

    public ObjectPropertyPathRef(object? parent, string property) : base(parent)
    {
        this._property = property;
    }

    public override void Set(object? newVal, Configuration configuration)
    {
        configuration.JsonProvider.SetProperty(Parent, _property, newVal);
    }
    public override void Put(String key, Object value, Configuration configuration)
    {
        Object target = configuration.JsonProvider.GetMapValue(Parent, _property);
        if (IsTargetInvalid(target))
        {
            return;
        }
        if (configuration.JsonProvider.IsMap(target))
        {
            configuration.JsonProvider.SetProperty(target, key, value);
        }
        else
        {
            throw new InvalidModificationException("Can only add properties to a map");
        }
    }

    public override void Convert(MapDelegate mapFunction, Configuration configuration)
    {
        var currentValue = configuration.JsonProvider.GetMapValue(Parent, _property);
        configuration.JsonProvider.SetProperty(Parent, _property, mapFunction(currentValue, configuration));
    }


    public override void Delete(Configuration configuration)
    {
        configuration.JsonProvider.RemoveProperty(Parent, _property);
    }

    public override void Add(object? value, Configuration configuration)
    {
        var target = configuration.JsonProvider.GetMapValue(Parent, _property);
        if (IsTargetInvalid(target)) return;
        if (configuration.JsonProvider.IsArray(target))
            configuration.JsonProvider.SetArrayIndex(target, configuration.JsonProvider.Length(target), value);
        else
            throw new InvalidModificationException("Can only.Add to an array");
    }

    public override void Add(string key, object? value, Configuration configuration)
    {
        var target = configuration.JsonProvider.GetMapValue(Parent, _property);
        if (IsTargetInvalid(target)) return;
        if (configuration.JsonProvider.IsMap(target))
            configuration.JsonProvider.SetProperty(target, key, value);
        else
            throw new InvalidModificationException("Can only.Add properties to a map");
    }


    public override void RenameKey(string oldKeyName, string newKeyName, Configuration configuration)
    {
        var target = configuration.JsonProvider.GetMapValue(Parent, _property);
        if (IsTargetInvalid(target)) return;
        RenameInMap(target, oldKeyName, newKeyName, configuration);
    }


    protected override object? GetAccessor()
    {
        return _property;
    }
}