using XavierJefferson.JsonPathParser.Exceptions;
using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.PathRefs;

public class ObjectMultiPropertyPathRef : PathRef
{
    private readonly ICollection<string> _properties;

    public ObjectMultiPropertyPathRef(object? parent, ICollection<string> properties) : base(parent)
    {
        _properties = properties;
    }

    public override void Put(string key, object newVal, Configuration configuration)
    {
        throw new InvalidModificationException("Put can not be performed to multiple properties");
    }

    public override void Set(object? newVal, Configuration configuration)
    {
        foreach (var property in _properties) configuration.JsonProvider.SetProperty(Parent, property, newVal);
    }

    public override void Convert(MapDelegate mapFunction, Configuration configuration)
    {
        foreach (var property in _properties)
        {
            var currentValue = configuration.JsonProvider.GetMapValue(Parent, property);
            if (currentValue != IJsonProvider.Undefined)
                configuration.JsonProvider.SetProperty(Parent, property, mapFunction(currentValue, configuration));
        }
    }

    public override void Delete(Configuration configuration)
    {
        foreach (var property in _properties) configuration.JsonProvider.RemoveProperty(Parent, property);
    }


    public override void Add(object? newVal, Configuration configuration)
    {
        throw new InvalidModificationException("Add can not be performed to multiple properties");
    }


    public override void Add(string key, object? newVal, Configuration configuration)
    {
        throw new InvalidModificationException("Put can not be performed to multiple properties");
    }


    public override void RenameKey(string oldKeyName, string newKeyName, Configuration configuration)
    {
        throw new InvalidModificationException("Rename can not be performed to multiple properties");
    }


    protected override object? GetAccessor()
    {
        return string.Join("&&", _properties);
    }
}