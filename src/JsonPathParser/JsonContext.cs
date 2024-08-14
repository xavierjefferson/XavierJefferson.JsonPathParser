using XavierJefferson.JsonPathParser.Helpers;
using XavierJefferson.JsonPathParser.Interfaces;
using XavierJefferson.JsonPathParser.Logging;

namespace XavierJefferson.JsonPathParser;

public class JsonContext : IDocumentContext
{
    private static readonly ILog Logger = LoggerFactory.GetLogger(typeof(JsonContext));

    public JsonContext(object? json, Configuration configuration)
    {
        Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        Json = json ?? throw new ArgumentNullException(nameof(json));
    }


    public Configuration Configuration { get; }

    //------------------------------------------------
    //
    // ReadContext impl
    //
    //------------------------------------------------


    public object? Json { get; }

    public string? JsonString => Configuration.JsonProvider.ToJson(Json);


    public object? Read(string path, params IPredicate[] filters)
    {
        Assertions.NotEmpty(path, "path can not be null or empty");
        var tmp = Read(PathFromCache(path, filters));
        return tmp;
    }


    public object? Read(string path, Type type, params IPredicate[] filters)
    {
        return Convert(Read(path, filters), type, Configuration);
    }


    public object? Read(JsonPath path)
    {
        if (path == null) throw new ArgumentNullException(nameof(path));
        return path.Read(Json, Configuration);
    }


    public object? Read(JsonPath path, Type type)
    {
        return Convert(Read(path), type, Configuration);
    }


    public T? Read<T>(JsonPath path)
    {
        return Convert<T>(Read(path), Configuration);
    }

    public T? Read<T>(string path, params IPredicate[] filters)
    {
        return Convert<T>(Read(path, filters), Configuration);
    }

    public T? Read<T>(string path)
    {
        return Convert<T>(Read(path), Configuration);
    }


    public IReadContext Limit(int maxResults)
    {
        return WithListeners(new LimitingEvaluationListener(maxResults).ResultFound);
    }


    public IReadContext WithListeners(params EvaluationCallback[] listener)
    {
        return new JsonContext(Json, Configuration.SetEvaluationListeners(listener));
    }


    public IDocumentContext Set(string path, object? newValue, params IPredicate[] filters)
    {
        return Set(PathFromCache(path, filters), newValue);
    }


    public IDocumentContext Set(JsonPath path, object? newValue)
    {
        var modified = path.Set(Json, newValue, Configuration.AddOptions(Option.AsPathList)) as ICollection<string>;
        if (Logger.IsDebugEnabled())
            foreach (var p in modified)
                Logger.Debug($"HashSet path {p} new value {newValue}");
        return this;
    }


    public IDocumentContext Map(string path, MapDelegate mapFunction, params IPredicate[] filters)
    {
        Map(PathFromCache(path, filters), mapFunction);
        return this;
    }


    public IDocumentContext? Map(JsonPath path, MapDelegate mapFunction)
    {
        var obj = path.Map(Json, mapFunction, Configuration);
        return obj == null ? null : this;
    }


    public IDocumentContext Delete(string path, params IPredicate[] filters)
    {
        return Delete(PathFromCache(path, filters));
    }


    public IDocumentContext Delete(JsonPath path)
    {
        var modified = path.Delete(Json, Configuration.AddOptions(Option.AsPathList)) as ICollection<string>;
        if (Logger.IsDebugEnabled())
            foreach (var p in modified)
                Logger.Debug($"Delete path {p}");
        return this;
    }


    public IDocumentContext Add(string path, object? value, params IPredicate[] filters)
    {
        return Add(PathFromCache(path, filters), value);
    }


    public IDocumentContext Add(JsonPath path, object? value)
    {
        var modified = path.Add(Json, value, Configuration.AddOptions(Option.AsPathList)) as ICollection<string>;
        if (Logger.IsDebugEnabled() && modified != null)
            foreach (var p in modified)
                Logger.Debug($"Add path {p} new value {value}");
        return this;
    }


    public IDocumentContext Add(string path, string key, object? value, params IPredicate[] filters)
    {
        return Add(PathFromCache(path, filters), key, value);
    }


    public IDocumentContext RenameKey(string path, string oldKeyName, string newKeyName, params IPredicate[] filters)
    {
        return RenameKey(PathFromCache(path, filters), oldKeyName, newKeyName);
    }

    public IDocumentContext Put(string path, string key, object value, params IPredicate[] filters)
    {
        return Put(PathFromCache(path, filters), key, value);
    }

    public IDocumentContext Put(JsonPath path, string key, object value)
    {
        var modified = path.Put<IList<string>>(Json, key, value, Configuration.AddOptions(Option.AsPathList));
        if (Logger.IsDebugEnabled())
            foreach (var p in modified)
                Logger.DebugFormat($"Put path {p} key {key} value {value}");
        return this;
    }

    public IDocumentContext RenameKey(JsonPath path, string oldKeyName, string newKeyName)
    {
        var modified =
            path.RenameKey(Json, oldKeyName, newKeyName, Configuration.AddOptions(Option.AsPathList)) as
                ICollection<string>;
        if (Logger.IsDebugEnabled() && modified != null)
            foreach (var p in modified)
                Logger.Debug($"Rename path {p} new value {newKeyName}");
        return this;
    }


    public IDocumentContext Add(JsonPath path, string key, object? value)
    {
        var modified =
            path.Add(Json, key, value, Configuration.AddOptions(Option.AsPathList)) as ICollection<string>;
        if (Logger.IsDebugEnabled() && modified != null)
            foreach (var p in modified)
                Logger.Debug($"Put path {p} key {key} value {value}");
        return this;
    }

    private static object? Convert(object? obj, Type targetType, Configuration configuration)
    {
        return configuration.MappingProvider.Map(obj, targetType, configuration);
    }

    private static T? Convert<T>(object? obj, Configuration configuration)
    {
        return (T?)configuration.MappingProvider.Map<T>(obj, configuration);
    }

    private JsonPath PathFromCache(string path, IPredicate[]? filters)
    {
        var cache = CacheManager.Instance;
        var cacheKey = filters == null || filters.Length == 0
            ? path
            : string.Concat(new[] { path }.Union(filters.Select(i => i.ToString())));
        var jsonPath = cache.Get(cacheKey) as JsonPath;
        if (jsonPath == null)
        {
            jsonPath = JsonPath.Compile(path, filters);
            cache.Add(cacheKey, jsonPath);
        }

        return jsonPath;
    }
}