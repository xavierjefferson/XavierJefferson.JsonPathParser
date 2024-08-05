using XavierJefferson.JsonPathParser.Exceptions;

namespace XavierJefferson.JsonPathParser.PathRefs;

public class RootPathRef : PathRef
{
    public RootPathRef(object? parent) : base(parent)
    {
    }
    public override void Put(String key, Object newVal, Configuration configuration)
    {
        if (configuration.JsonProvider.IsMap(Parent))
        {
            configuration.JsonProvider.SetProperty(Parent, key, newVal);
        }
        else
        {
            throw new InvalidModificationException("Invalid put operation. $ is not a map");
        }
    }

    protected override object? GetAccessor()
    {
        return "$";
    }


    public override void Set(object? newVal, Configuration configuration)
    {
        throw new InvalidModificationException("Invalid set operation");
    }

    public override void Convert(MapDelegate mapFunction, Configuration configuration)
    {
        throw new InvalidModificationException("Invalid map operation");
    }


    public override void Delete(Configuration configuration)
    {
        throw new InvalidModificationException("Invalid delete operation");
    }


    public override void Add(object? newVal, Configuration configuration)
    {
        if (configuration.JsonProvider.IsArray(Parent))
            configuration.JsonProvider.SetArrayIndex(Parent, configuration.JsonProvider.Length(Parent), newVal);
        else
            throw new InvalidModificationException("Invalid.Add operation. $ is not an array");
    }


    public override void Add(string key, object? newVal, Configuration configuration)
    {
        if (configuration.JsonProvider.IsMap(Parent))
            configuration.JsonProvider.SetProperty(Parent, key, newVal);
        else
            throw new InvalidModificationException("Invalid Add operation. $ is not a map");
    }


    public override void RenameKey(string oldKeyName, string newKeyName, Configuration configuration)
    {
        var target = Parent;
        if (IsTargetInvalid(target)) return;
        RenameInMap(target, oldKeyName, newKeyName, configuration);
    }
}