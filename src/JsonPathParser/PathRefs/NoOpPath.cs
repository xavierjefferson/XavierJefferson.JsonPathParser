namespace XavierJefferson.JsonPathParser.PathRefs;

internal class NoOpPath : PathRef
{
    public NoOpPath() : base(null)
    {
    }

    protected override object? GetAccessor()
    {
        return null;
    }


    public override void Set(object? newVal, Configuration configuration)
    {
    }


    public override void Convert(MapDelegate mapFunction, Configuration configuration)
    {
    }


    public override void Delete(Configuration configuration)
    {
    }


    public override void Add(object? newVal, Configuration configuration)
    {
    }


    public override void Add(string key, object? newVal, Configuration configuration)
    {
    }


    public override void RenameKey(string oldKeyName, string newKeyName, Configuration configuration)
    {
    }

    public override void Put(string key, object? newVal, Configuration configuration)
    {
    }
}