using System.Collections.Immutable;
using System.Collections.ObjectModel;
using XavierJefferson.JsonPathParser.Enums;
using XavierJefferson.JsonPathParser.Exceptions;
using XavierJefferson.JsonPathParser.Interfaces;
using XavierJefferson.JsonPathParser.PathRefs;

namespace XavierJefferson.JsonPathParser.Path;

/// <summary>
///     */
public class EvaluationContextImpl : IEvaluationContext
{
    private static readonly EvaluationAbortException AbortEvaluation = new();

    private readonly Dictionary<IPath, object?> _documentEvalCache = new();
    private readonly bool _forUpdate;
    private readonly IPath _path;
    private readonly object? _pathResult;
    private readonly bool _suppressExceptions;
    private readonly SerializingList<PathRef> _updateOperations;
    private readonly object? _valueResult;
    private int _resultIndex;

    public EvaluationContextImpl(IPath path, object? rootDocument, Configuration configuration, bool forUpdate)
    {
        _forUpdate = forUpdate;
        _path = path ?? throw new ArgumentNullException(nameof(path));
        RootDocument = rootDocument ?? throw new ArgumentNullException(nameof(rootDocument));
        Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _valueResult = Configuration.JsonProvider.CreateArray();
        _pathResult = Configuration.JsonProvider.CreateArray();
        _updateOperations = new SerializingList<PathRef>();
        _suppressExceptions = Configuration.ContainsOption(Option.SuppressExceptions);
    }

    public IJsonProvider JsonProvider => Configuration.JsonProvider;


    public IReadOnlySet<Option> Options => Configuration.Options;


    public Configuration Configuration { get; }


    public object? RootDocument { get; }

    public ICollection<PathRef> UpdateOperations => new ReadOnlyCollection<PathRef>(_updateOperations);


    public object? GetValue()
    {
        return GetValue(true);
    }


    public object? GetValue(bool unwrap)
    {
        if (_path.IsDefinite())
        {
            if (_resultIndex == 0)
            {
                if (_suppressExceptions) return null;
                throw new PathNotFoundException($"No results for path: {_path}");
            }

            var len = JsonProvider.Length(_valueResult);
            var value = len > 0 ? JsonProvider.GetArrayIndex(_valueResult, len - 1) : null;
            if (value != null && unwrap) value = JsonProvider.Unwrap(value);
            return value;
        }

        return _valueResult;
    }


    public T? GetPath<T>()
    {
        if (_resultIndex == 0)
        {
            if (_suppressExceptions) return default;
            throw new PathNotFoundException($"No results for path: {_path}");
        }

        return (T)_pathResult;
    }


    public SerializingList<string> GetPathList()
    {
        var res = new SerializingList<string>();
        if (_resultIndex > 0)
        {
            var objects = Configuration.JsonProvider.AsEnumerable(_pathResult);
            res.AddRange(objects.Select(i => i.ToString()));
        }

        return res;
    }


    public RootPathToken GetRoot()
    {
        return ((CompiledPath)_path).GetRoot();
    }

    public Dictionary<IPath, object?> DocumentEvalCache()
    {
        return _documentEvalCache;
    }

    public bool ForUpdate()
    {
        return _forUpdate;
    }

    public void AddResult(string path, PathRef operation, object? model)
    {
        if (_forUpdate) _updateOperations.Add(operation);

        Configuration.JsonProvider.SetArrayIndex(_valueResult, _resultIndex, model);
        Configuration.JsonProvider.SetArrayIndex(_pathResult, _resultIndex, path);
        _resultIndex++;
        if (Configuration.EvaluationCallbacks.Any())
        {
            var idx = _resultIndex - 1;
            foreach (var listener in Configuration.EvaluationCallbacks)
            {
                var continuation = listener(new FoundResultImpl(idx, path, model));
                if (EvaluationContinuationEnum.Abort == continuation) throw AbortEvaluation;
            }
        }
    }
}