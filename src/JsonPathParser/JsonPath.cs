using System.Collections;
using System.Text;
using System.Text.Json;
using XavierJefferson.JsonPathParser.Exceptions;
using XavierJefferson.JsonPathParser.Helpers;
using XavierJefferson.JsonPathParser.Interfaces;
using XavierJefferson.JsonPathParser.Path;

namespace XavierJefferson.JsonPathParser;

/// <summary>
///     <p />
///     JsonPath is to JSON what XPATH is to XML, a simple way to extract parts of a given document. JsonPath is
///     available in many programming languages such as Javascript, Python and PHP.
///     <p />
///     JsonPath allows you to compile a json path string to use it many times or to compile and apply in one
///     single on demand operation.
///     <p />
///     Given the Json document:
///     <p />
///     <pre>
///         string json =
///         "{
///         "store":
///         {
///         "book":
///         [
///         {
///         "category": "reference",
///         "author": "Nigel Rees",
///         "title": "Sayings of the Century",
///         "price": 8.95
///         },
///         {
///         "category": "fiction",
///         "author": "Evelyn Waugh",
///         "title": "Sword of Honour",
///         "price": 12.99
///         }
///         ],
///         "bicycle":
///         {
///         "color": "red",
///         "price": 19.95
///         }
///         }
///         }";
///     </pre>
///     <p />
///     A JsonPath can be compiled and used as shown:
///     <p />
///     <code>
///  JsonPath path = JsonPath.compile("$.store.book[1]");
///  <br />
///  List&lt;object&gt; books = path.read(json);
///  </code>
///     </p>
///     Or:
///     <p />
///     <code>
///  List&lt;object&gt; authors = JsonPath.read(json, "$.store.book[*].author")
///  </code>
///     <p />
///     If the json path returns a single value (is definite):
///     </p>
///     <code>
///  string author = JsonPath.read(json, "$.store.book[1].author")
///  </code>
/// </summary>
public class JsonPath
{
    private readonly IPath _path;

    private JsonPath(string jsonPath, IPredicate[]? filters)
    {
        if (jsonPath == null) throw new ArgumentNullException(nameof(jsonPath));
        _path = PathCompiler.Compile(jsonPath, filters);
    }

    /// <summary>
    ///     Returns the string representation of this JsonPath
    /// </summary>
    /// <returns> path as string</returns>
    public string? GetPath()
    {
        return _path.ToString();
    }

    /// <summary>
    ///     @see JsonPath#isDefinite()
    /// </summary>
    public static bool IsPathDefinite(string path)
    {
        return Compile(path).IsDefinite();
    }

    /// <summary>
    ///     Checks if a path points to a single item or if it potentially returns multiple items
    ///     <p />
    ///     a path is considered <strong>not</strong> definite if it contains a scan fragment ".."
    ///     or an array position fragment that is not based on a single index
    ///     <p />
    ///     <p />
    ///     definite path examples are:
    ///     <p />
    ///     $store.book
    ///     $store.book[1].title
    ///     <p />
    ///     not definite path examples are:
    ///     <p />
    ///     $..book
    ///     $.store.book[*]
    ///     $.store.book[1,2]
    ///     $.store.book[?(@.category = 'fiction')]
    /// </summary>
    /// <returns> true if path is definite (points to single item)</returns>
    public bool IsDefinite()
    {
        return _path.IsDefinite();
    }

    /// <summary>
    ///     Applies this JsonPath to the provided json document.
    ///     Note that the document must be identified as either a List or Dictionary by
    ///     the {@link JsonProvider}
    /// </summary>
    /// <param name="jsonObject">a container object</param>
    /// <param name="
    /// 
    /// 
    /// <T>
    ///     ">expected return type</param>
    ///     <returns> object(s) matched by the given path</returns>
    public object? Read(object? jsonObject)
    {
        return Read(jsonObject, Configuration.DefaultConfiguration());
    }

    /// <summary>
    ///     Applies this JsonPath to the provided json document.
    ///     Note that the document must be identified as either a List or Dictionary by
    ///     the {@link JsonProvider}
    /// </summary>
    /// <param name="jsonObject">a container object</param>
    /// <param name="configuration">configuration to use</param>
    /// <param name="
    /// 
    /// 
    /// <T>
    ///     ">expected return type</param>
    ///     <returns> object(s) matched by the given path</returns>
    public object? Read(object? jsonObject, Configuration configuration)
    {
        var optAsPathList = configuration.ContainsOption(Option.AsPathList);
        var optAlwaysReturnList = configuration.ContainsOption(Option.AlwaysReturnList);
        var optSuppressExceptions = configuration.ContainsOption(Option.SuppressExceptions);

        if (_path.IsFunctionPath())
        {
            if (optAsPathList || optAlwaysReturnList)
            {
                if (optSuppressExceptions) return _path.IsDefinite() ? null : configuration.JsonProvider.CreateArray();
                throw new JsonPathException("Options " + Option.AsPathList + " and " + Option.AlwaysReturnList +
                                            " are not allowed when using path functions!");
            }

            var evaluationContext = _path.Evaluate(jsonObject, jsonObject, configuration);
            if (optSuppressExceptions && !evaluationContext.GetPathList().Any())
                return _path.IsDefinite() ? null : configuration.JsonProvider.CreateArray();
            return evaluationContext.GetValue(true);
        }

        if (optAsPathList)
        {
            var evaluationContext = _path.Evaluate(jsonObject, jsonObject, configuration);
            if (optSuppressExceptions && !evaluationContext.GetPathList().Any())
                return configuration.JsonProvider.CreateArray();
            return evaluationContext.GetPath<object?>();
        }
        else
        {
            var evaluationContext = _path.Evaluate(jsonObject, jsonObject, configuration);
            if (optSuppressExceptions && !evaluationContext.GetPathList().Any())
            {
                if (optAlwaysReturnList)
                    return configuration.JsonProvider.CreateArray();
                return _path.IsDefinite() ? null : configuration.JsonProvider.CreateArray();
            }

            var res = evaluationContext.GetValue(false);
            if (optAlwaysReturnList && _path.IsDefinite())
            {
                var array = configuration.JsonProvider.CreateArray();
                configuration.JsonProvider.SetArrayIndex(array, 0, res);
                return array;
            }

            return res;
        }
    }

    /// <summary>
    ///     HashSet the value this path points to in the provided jsonObject
    /// </summary>
    /// <param name="jsonObject">a json object</param>
    /// <param name="newVal"></param>
    /// <param name="configuration">configuration to use</param>
    /// <param name="
    /// 
    /// 
    /// <T>
    ///     ">expected return type</param>
    ///     <returns> the updated jsonObject or the path list to updated objects if option AS_PATH_LIST is set.</returns>
    public object? Set(object? jsonObject, object? newVal, Configuration configuration)
    {
        if (jsonObject == null) throw new ArgumentNullException(nameof(jsonObject));
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));
        var evaluationContext = _path.Evaluate(jsonObject, jsonObject, configuration, true);
        if (!evaluationContext.GetPathList().Any())
        {
            var optSuppressExceptions = configuration.ContainsOption(Option.SuppressExceptions);
            if (optSuppressExceptions)
                return HandleMissingPathInContext(configuration);
            throw new PathNotFoundException();
        }

        foreach (var updateOperation in evaluationContext.UpdateOperations)
            updateOperation.Set(newVal, configuration);
        return ResultByConfiguration(jsonObject, configuration, evaluationContext);
    }


    /// <summary>
    ///     Replaces the value on the given path with the result of the {@link MapFunction}.
    /// </summary>
    /// <param name="jsonObject">a json object</param>
    /// <param name="mapFunction">Converter object to be invoked</param>
    /// <param name="configuration">configuration to use</param>
    /// <returns> the updated jsonObject or the path list to updated objects if option AS_PATH_LIST is set.</returns>
    public object? Map(object? jsonObject, MapDelegate mapFunction, Configuration configuration)
    {
        if (jsonObject == null) throw new ArgumentNullException(nameof(jsonObject));
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));
        if (mapFunction == null) throw new ArgumentNullException(nameof(mapFunction));
        var evaluationContext = _path.Evaluate(jsonObject, jsonObject, configuration, true);
        if (!evaluationContext.GetPathList().Any())
        {
            var optSuppressExceptions = configuration.ContainsOption(Option.SuppressExceptions);
            if (optSuppressExceptions)
                return HandleMissingPathInContext(configuration);
            throw new PathNotFoundException();
        }

        foreach (var updateOperation in evaluationContext.UpdateOperations)
            updateOperation.Convert(mapFunction, configuration);
        return ResultByConfiguration(jsonObject, configuration, evaluationContext);
    }

    /// <summary>
    ///     Deletes the object this path points to in the provided jsonObject
    /// </summary>
    /// <param name="jsonObject">a json object</param>
    /// <param name="configuration">configuration to use</param>
    /// <param name="
    /// 
    /// 
    /// <T>
    ///     ">expected return type</param>
    ///     <returns> the updated jsonObject or the path list to deleted objects if option AS_PATH_LIST is set.</returns>
    public object? Delete(object? jsonObject, Configuration configuration)
    {
        if (jsonObject == null) throw new ArgumentNullException(nameof(jsonObject));
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));
        var evaluationContext = _path.Evaluate(jsonObject, jsonObject, configuration, true);
        if (!evaluationContext.GetPathList().Any())
        {
            var optSuppressExceptions = configuration.ContainsOption(Option.SuppressExceptions);
            if (optSuppressExceptions)
                return HandleMissingPathInContext(configuration);
            throw new PathNotFoundException();
        }

        foreach (var updateOperation in evaluationContext.UpdateOperations.OrderByDescending(i => i.Index))
            updateOperation.Delete(configuration);
        return ResultByConfiguration(jsonObject, configuration, evaluationContext);
    }

    /// <summary>
    ///     Adds a new value to the Array this path points to in the provided jsonObject
    /// </summary>
    /// <param name="jsonObject">a json object</param>
    /// <param name="value">the value to.Add</param>
    /// <param name="configuration">configuration to use</param>
    /// <param name="
    /// 
    /// 
    /// <T>
    ///     ">expected return type</param>
    ///     <returns> the updated jsonObject or the path list to updated object if option AS_PATH_LIST is set.</returns>
    public object? Add(object? jsonObject, object? value, Configuration configuration)
    {
        if (jsonObject == null) throw new ArgumentNullException(nameof(jsonObject));
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));
        var evaluationContext = _path.Evaluate(jsonObject, jsonObject, configuration, true);
        if (!evaluationContext.GetPathList().Any())
        {
            var optSuppressExceptions = configuration.ContainsOption(Option.SuppressExceptions);
            if (optSuppressExceptions)
                return HandleMissingPathInContext(configuration);
            throw new PathNotFoundException();
        }

        foreach (var updateOperation in evaluationContext.UpdateOperations) updateOperation.Add(value, configuration);
        return ResultByConfiguration(jsonObject, configuration, evaluationContext);
    }

    /// <summary>
    ///     Adds or updates the object this path points to in the provided jsonObject with a key with a value
    /// </summary>
    /// <param name="jsonObject">a json object?</param>
    /// <param name="key">the key to.Add or update</param>
    /// <param name="value">the new value</param>
    /// <param name="configuration">configuration to use</param>
    /// <param name="
    /// 
    /// 
    /// <T>
    ///     ">expected return type</param>
    ///     <returns> the updated jsonObject or the path list to updated objects if option AS_PATH_LIST is set.</returns>
    public object? Add(object? jsonObject, string key, object? value, Configuration configuration)
    {
        if (jsonObject == null) throw new ArgumentNullException(nameof(jsonObject));
        Assertions.NotEmpty(key, "key can not be null or empty");
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));
        var evaluationContext = _path.Evaluate(jsonObject, jsonObject, configuration, true);
        if (!evaluationContext.GetPathList().Any())
        {
            var optSuppressExceptions = configuration.ContainsOption(Option.SuppressExceptions);
            if (optSuppressExceptions)
                return HandleMissingPathInContext(configuration);
            throw new PathNotFoundException();
        }

        foreach (var updateOperation in evaluationContext.UpdateOperations)
            updateOperation.Add(key, value, configuration);
        return ResultByConfiguration(jsonObject, configuration, evaluationContext);
    }

    public object? RenameKey(object? jsonObject, string oldKeyName, string newKeyName, Configuration configuration)
    {
        if (jsonObject == null) throw new ArgumentNullException(nameof(jsonObject));
        Assertions.NotEmpty(newKeyName, "newKeyName can not be null or empty");
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));
        var evaluationContext = _path.Evaluate(jsonObject, jsonObject, configuration, true);
        foreach (var updateOperation in evaluationContext.UpdateOperations)
        {
            var optSuppressExceptions = configuration.ContainsOption(Option.SuppressExceptions);
            try
            {
                updateOperation.RenameKey(oldKeyName, newKeyName, configuration);
            }
            catch (Exception e)
            {
                if (optSuppressExceptions)
                {
                    // With option SUPPRESS_EXCEPTIONS,
                    // the PathNotFoundException should be ignored and the other updateOperation should be continued.
                }
                else
                {
                    throw;
                }
            }
        }

        return ResultByConfiguration(jsonObject, configuration, evaluationContext);
    }

    /// <summary>
    ///     Applies this JsonPath to the provided json string
    /// </summary>
    /// <param name="json">a json string</param>
    /// <param name="
    /// 
    /// 
    /// <T>
    ///     ">expected return type</param>
    ///     <returns> list of objects matched by the given path</returns>
    public object? Read(string json)
    {
        return Read(json, Configuration.DefaultConfiguration());
    }

    /// <summary>
    ///     Applies this JsonPath to the provided json string
    /// </summary>
    /// <param name="json">a json string</param>
    /// <param name="configuration">configuration to use</param>
    /// <param name="
    /// 
    /// 
    /// <T>
    ///     ">expected return type</param>
    ///     <returns> list of objects matched by the given path</returns>
    public object? Read(string json, Configuration configuration)
    {
        Assertions.NotEmpty(json, "json can not be null or empty");
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        return Read(configuration.JsonProvider.Parse(json), configuration);
    }

    /// <summary>
    ///     Applies this JsonPath to the provided json URL
    /// </summary>
    /// <param name="uri">url to read from</param>
    /// <param name="
    /// 
    /// 
    /// <T>
    ///     ">expected return type</param>
    ///     <returns> list of objects matched by the given path</returns>
    public object? Read(Uri uri)
    {
        return Read(uri, Configuration.DefaultConfiguration());
    }

    public T? Read<T>(string json, Configuration? configuration = null)
    {
        var tmp = configuration == null ? Read(json) : Read(json, configuration);
        return (T?)tmp;
    }

    /// <summary>
    ///     Applies this JsonPath to the provided json file
    /// </summary>
    /// <param name="jsonFile">file to read from</param>
    /// <param name="
    /// 
    /// 
    /// <T>
    ///     ">expected return type</param>
    ///     <returns> list of objects matched by the given path</returns>
    public object? Read(FileInfo jsonFile)
    {
        return Read(jsonFile, Configuration.DefaultConfiguration());
    }


    /// <summary>
    ///     Applies this JsonPath to the provided json file
    /// </summary>
    /// <param name="jsonFile">file to read from</param>
    /// <param name="configuration">configuration to use</param>
    /// <param name="
    /// 
    /// 
    /// <T>
    ///     ">expected return type</param>
    ///     <returns> list of objects matched by the given path</returns>
    public object? Read(FileInfo jsonFile, Configuration configuration)
    {
        if (jsonFile == null) throw new ArgumentNullException(nameof(jsonFile));
        Assertions.IsTrue(jsonFile.Exists, "json file does not exist");
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));
        using (var fis = File.OpenRead(jsonFile.FullName))
        {
            return Read(fis, configuration);
        }
    }

    /// <summary>
    ///     Applies this JsonPath to the provided json input stream
    /// </summary>
    /// <param name="jsonInputStream">input stream to read from</param>
    /// <param name="
    /// 
    /// 
    /// <T>
    ///     ">expected return type</param>
    ///     <returns> list of objects matched by the given path</returns>
    public object? Read(Stream jsonInputStream)
    {
        return Read(jsonInputStream, Configuration.DefaultConfiguration());
    }

    /// <summary>
    ///     Applies this JsonPath to the provided json input stream
    /// </summary>
    /// <param name="jsonInputStream">input stream to read from</param>
    /// <param name="configuration">configuration to use</param>
    /// <param name="
    /// 
    /// 
    /// <T>
    ///     ">expected return type</param>
    ///     <returns> list of objects matched by the given path</returns>
    public object? Read(Stream jsonInputStream, Configuration configuration)
    {
        if (jsonInputStream == null) throw new ArgumentNullException(nameof(jsonInputStream));
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));
        return Read(jsonInputStream, Encoding.UTF8, configuration);
    }

    /// <summary>
    ///     Applies this JsonPath to the provided json input stream
    /// </summary>
    /// <param name="jsonInputStream">input stream to read from</param>
    /// <param name="configuration">configuration to use</param>
    /// <param name="
    /// 
    /// 
    /// <T>
    ///     ">expected return type</param>
    ///     <returns> list of objects matched by the given path</returns>
    public object? Read(Stream jsonInputStream, Encoding charset, Configuration configuration)
    {
        if (jsonInputStream == null) throw new ArgumentNullException(nameof(jsonInputStream));
        if (charset == null) throw new ArgumentNullException(nameof(charset));
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));
        using (jsonInputStream)
        {
            return Read(configuration.JsonProvider.Parse(jsonInputStream, charset), configuration);
        }
    }

    // --------------------------------------------------------
    //
    // Static factory methods
    //
    // --------------------------------------------------------

    /// <summary>
    ///     Compiles a JsonPath
    /// </summary>
    /// <param name="jsonPath">to compile</param>
    /// <param name="filters">filters to be applied to the filter place holders  [?] in the path</param>
    /// <returns> compiled JsonPath</returns>
    public static JsonPath Compile(string jsonPath, params IPredicate[]? filters)
    {
        Assertions.NotEmpty(jsonPath, "json can not be null or empty");

        return new JsonPath(jsonPath, filters);
    }


    // --------------------------------------------------------
    //
    // Static utility functions
    //
    // --------------------------------------------------------

    /// <summary>
    ///     Creates a new JsonPath and applies it to the provided Json object
    /// </summary>
    /// <param name="json">a json object</param>
    /// <param name="jsonPath">the json path</param>
    /// <param name="filters">filters to be applied to the filter place holders  [?] in the path</param>
    /// <param name="
    /// 
    /// 
    /// <T>
    ///     ">expected return type</param>
    ///     <returns> list of objects matched by the given path</returns>
    public static object? Read(object? json, string jsonPath, params IPredicate[] filters)
    {
        return Parse(json).Read(jsonPath, filters);
    }

    public static T? Read<T>(object? json, string jsonPath, params IPredicate[] filters)
    {
        return Parse(json).Read<T>(jsonPath, filters);
    }

    /// <summary>
    ///     Creates a new JsonPath and applies it to the provided Json string
    /// </summary>
    /// <param name="json">a json string</param>
    /// <param name="jsonPath">the json path</param>
    /// <param name="filters">filters to be applied to the filter place holders  [?] in the path</param>
    /// <param name="
    /// 
    /// 
    /// <T>
    ///     ">expected return type</param>
    ///     <returns> list of objects matched by the given path</returns>
    public static object? Read(string json, string jsonPath, params IPredicate[] filters)
    {
        return new ParseContextImpl().Parse(json).Read(jsonPath, filters);
    }


    /// <summary>
    ///     Creates a new JsonPath and applies it to the provided Json object
    /// </summary>
    /// <param name="uri">url pointing to json doc</param>
    /// <param name="jsonPath">the json path</param>
    /// <param name="filters">filters to be applied to the filter place holders  [?] in the path</param>
    /// <param name="
    /// 
    /// 
    /// <T>
    ///     ">expected return type</param>
    ///     <returns> list of objects matched by the given path</returns>
    [Obsolete]
    public static object? Read(Uri uri, string jsonPath, params IPredicate[] filters)
    {
        return new ParseContextImpl().Parse(uri).Read(jsonPath, filters);
    }

    /// <summary>
    ///     Creates a new JsonPath and applies it to the provided Json object
    /// </summary>
    /// <param name="jsonFile">json file</param>
    /// <param name="jsonPath">the json path</param>
    /// <param name="filters">filters to be applied to the filter place holders  [?] in the path</param>
    /// <param name="
    /// 
    /// 
    /// <T>
    ///     ">expected return type</param>
    ///     <returns> list of objects matched by the given path</returns>
    public static object? Read(FileInfo jsonFile, string jsonPath, params IPredicate[] filters)
    {
        return new ParseContextImpl().Parse(jsonFile).Read(jsonPath, filters);
    }

    /// <summary>
    ///     Creates a new JsonPath and applies it to the provided Json object
    /// </summary>
    /// <param name="jsonInputStream">json input stream</param>
    /// <param name="jsonPath">the json path</param>
    /// <param name="filters">filters to be applied to the filter place holders  [?] in the path</param>
    /// <param name="
    /// 
    /// 
    /// <T>
    ///     ">expected return type</param>
    ///     <returns> list of objects matched by the given path</returns>
    public static object? Read(Stream jsonInputStream, string jsonPath, params IPredicate[] filters)
    {
        return new ParseContextImpl().Parse(jsonInputStream).Read(jsonPath, filters);
    }


    // --------------------------------------------------------
    //
    // Static Fluent API
    //
    // --------------------------------------------------------


    /// <summary>
    ///     Creates a {@link ParseContext} that can be used to Parse JSON input. The Parse context
    ///     is as thread safe as the underlying {@link JsonProvider}. Note that not all JsonProvider are
    ///     thread safe.
    /// </summary>
    /// <param name="configuration">configuration to use when parsing JSON</param>
    /// <returns> a parsing context based on given configuration</returns>
    public static IParseContext Using(Configuration configuration)
    {
        return new ParseContextImpl(configuration);
    }

    public static IParseContext Create(Action<ConfigurationBuilder>? action = null)
    {
        var cb = new ConfigurationBuilder();
        var c = DefaultsImpl.Instance;
        cb.WithOptions(c.Options).WithJsonProvider(c.JsonProvider).WithMappingProvider(c.MappingProvider);
        if (action != null)
            action(cb);
        return new ParseContextImpl(cb.Build());
    }

    /// <summary>
    ///     Creates a {@link ParseContext} that will Parse a given JSON input.
    /// </summary>
    /// <param name="provider">jsonProvider to use when parsing JSON</param>
    /// <returns> a parsing context based on given jsonProvider</returns>
    [Obsolete]
    public static IParseContext Using(IJsonProvider provider)
    {
        return new ParseContextImpl(Configuration.CreateBuilder().WithJsonProvider(provider).Build());
    }

    /// <summary>
    ///     Parses the given JSON input using the default {@link Configuration} and
    ///     returns a {@link DocumentContext} for path evaluation
    /// </summary>
    /// <param name="json">input</param>
    /// <returns> a read context</returns>
    public static IDocumentContext Parse(object? json)
    {
        if (json is string stringInstance) return new ParseContextImpl().Parse(stringInstance);
        if (json is IDictionary<string, object?> || json is IList) return new ParseContextImpl().Parse(json);
        return new ParseContextImpl().Parse(JsonSerializer.Serialize(json));
    }

    /// <summary>
    ///     Parses the given JSON input using the default {@link Configuration} and
    ///     returns a {@link DocumentContext} for path evaluation
    /// </summary>
    /// <param name="json">string</param>
    /// <returns> a read context</returns>
    public static IDocumentContext Parse(string json)
    {
        return new ParseContextImpl().Parse(json);
    }

    /// <summary>
    ///     Parses the given JSON input using the default {@link Configuration} and
    ///     returns a {@link DocumentContext} for path evaluation
    /// </summary>
    /// <param name="json">stream</param>
    /// <returns> a read context</returns>
    public static IDocumentContext Parse(Stream json)
    {
        return new ParseContextImpl().Parse(json);
    }

    /// <summary>
    ///     Parses the given JSON input using the default {@link Configuration} and
    ///     returns a {@link DocumentContext} for path evaluation
    /// </summary>
    /// <param name="json">file</param>
    /// <returns> a read context</returns>
    public static IDocumentContext Parse(FileInfo json)
    {
        return new ParseContextImpl().Parse(json);
    }

    /// <summary>
    ///     Parses the given JSON input using the default {@link Configuration} and
    ///     returns a {@link DocumentContext} for path evaluation
    /// </summary>
    /// <param name="json">url</param>
    /// <returns> a read context</returns>
    [Obsolete]
    public static IDocumentContext Parse(Uri json)
    {
        return new ParseContextImpl().Parse(json);
    }

    /// <summary>
    ///     Parses the given JSON input using the provided {@link Configuration} and
    ///     returns a {@link DocumentContext} for path evaluation
    /// </summary>
    /// <param name="json">input</param>
    /// <returns> a read context</returns>
    public static IDocumentContext Parse(object? json, Configuration configuration)
    {
        return new ParseContextImpl(configuration).Parse(json);
    }

    /// <summary>
    ///     Parses the given JSON input using the provided {@link Configuration} and
    ///     returns a {@link DocumentContext} for path evaluation
    /// </summary>
    /// <param name="json">input</param>
    /// <returns> a read context</returns>
    public static IDocumentContext Parse(string json, Configuration configuration)
    {
        return new ParseContextImpl(configuration).Parse(json);
    }

    /// <summary>
    ///     Parses the given JSON input using the provided {@link Configuration} and
    ///     returns a {@link DocumentContext} for path evaluation
    /// </summary>
    /// <param name="json">input</param>
    /// <returns> a read context</returns>
    public static IDocumentContext Parse(Stream json, Configuration configuration)
    {
        return new ParseContextImpl(configuration).Parse(json);
    }

    /// <summary>
    ///     Parses the given JSON input using the provided {@link Configuration} and
    ///     returns a {@link DocumentContext} for path evaluation
    /// </summary>
    /// <param name="json">input</param>
    /// <param name="configuration"></param>
    /// <returns> a read context</returns>
    public static IDocumentContext Parse(FileInfo json, Configuration configuration)
    {
        return new ParseContextImpl(configuration).Parse(json);
    }

    /// <summary>
    ///     Parses the given JSON input using the provided {@link Configuration} and
    ///     returns a {@link DocumentContext} for path evaluation
    /// </summary>
    /// <param name="json">input</param>
    /// <param name="configuration"></param>
    /// <returns> a read context</returns>
    [Obsolete]
    public static IDocumentContext Parse(Uri json, Configuration configuration)
    {
        return new ParseContextImpl(configuration).Parse(json);
    }

    private object? ResultByConfiguration(object? jsonObject, Configuration configuration,
        IEvaluationContext evaluationContext)
    {
        if (configuration.ContainsOption(Option.AsPathList))
            return evaluationContext.GetPathList();
        return jsonObject;
    }

    private object? HandleMissingPathInContext(Configuration configuration)
    {
        var optAsPathList = configuration.ContainsOption(Option.AsPathList);
        var optAlwaysReturnList = configuration.ContainsOption(Option.AlwaysReturnList);
        if (optAsPathList) return configuration.JsonProvider.CreateArray();

        if (optAlwaysReturnList)
            return configuration.JsonProvider.CreateArray();
        return _path.IsDefinite() ? null : configuration.JsonProvider.CreateArray();
    }

    /// <summary>
    ///     Adds or updates the Object this path points to in the provided jsonObject with a key with a value
    /// </summary>
    /// <param name="jsonObject">a json object</param>
    /*
    
    * @param jsonObject    a json object
    * @param key           the key to add or update
    * @param value         the new value
    * @param configuration configuration to use
    * @param <T>           expected return type
    * @return the updated jsonObject or the path list to updated objects if option AS_PATH_LIST is set.
    */
    public T Put<T>(object? jsonObject, string key, object? value, Configuration configuration)
    {
        if (jsonObject == null) throw new ArgumentNullException(nameof(jsonObject));
        Assertions.NotEmpty(key, "key can not be null or empty");
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        var evaluationContext = _path.Evaluate(jsonObject, jsonObject, configuration, true);
        if (!evaluationContext.GetPathList().Any())
        {
            var optSuppressExceptions = configuration.ContainsOption(Option.SuppressExceptions);
            if (optSuppressExceptions)
                return (T)HandleMissingPathInContext(configuration);
            throw new PathNotFoundException();
        }

        foreach (var updateOperation in evaluationContext.UpdateOperations)
            updateOperation.Put(key, value, configuration);
        return (T)ResultByConfiguration(jsonObject, configuration, evaluationContext);
    }
}