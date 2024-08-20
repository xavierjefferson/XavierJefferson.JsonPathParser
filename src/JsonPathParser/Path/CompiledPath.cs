using XavierJefferson.JsonPathParser.Enums;
using XavierJefferson.JsonPathParser.Exceptions;
using XavierJefferson.JsonPathParser.Function;
using XavierJefferson.JsonPathParser.Interfaces;
using XavierJefferson.JsonPathParser.Logging;
using XavierJefferson.JsonPathParser.PathRefs;

namespace XavierJefferson.JsonPathParser.Path;

public class CompiledPath : IPath
{
    private static readonly ILog Logger = LoggerFactory.GetLogger(typeof(CompiledPath));

    private readonly bool _isRootPath;

    private readonly RootPathToken _root;


    public CompiledPath(RootPathToken root, bool isRootPath)
    {
        _root = InvertScannerFunctionRelationship(root);
        _isRootPath = isRootPath;
    }


    public bool IsRootPath()
    {
        return _isRootPath;
    }


    public IEvaluationContext Evaluate(object? document, object? rootDocument, Configuration configuration,
        bool forUpdate = false)
    {
        if (Logger.IsDebugEnabled()) Logger.Debug($"Evaluating path: {ToString()}");

        var context = new EvaluationContextImpl(this, rootDocument, configuration, forUpdate);
        try
        {
            var op = context.ForUpdate() ? PathRef.CreateRoot(rootDocument) : PathRef.NoOp;
            _root.Evaluate("", op, document, context);
        }
        catch (EvaluationAbortException)
        {
        }

        return context;
    }


    public bool IsDefinite()
    {
        return _root.IsPathDefinite();
    }


    public bool IsFunctionPath()
    {
        return _root.IsFunctionPath();
    }

    /// <summary>
    ///     In the event the writer of the path referenced a function at the tail end of a scanner, augment the query such
    ///     that the root node is the function and the parameter to the function is the scanner.   This way we maintain
    ///     relative sanity in the path expression, functions either evaluate scalar values or arrays, they're
    ///     not re-entrant nor should they maintain state, they do however take parameters.
    /// </summary>
    /// <param name="path">
    ///     this is our old root path which will become a parameter (assuming there's a scanner terminated by a
    ///     function
    /// </param>
    /// <returns>A function with the scanner as input, or if this situation doesn't exist just the input path</returns>
    private RootPathToken InvertScannerFunctionRelationship(RootPathToken path)
    {
        if (path.IsFunctionPath() && path.Next() is ScanPathToken)
        {
            PathToken? token = path;
            PathToken? prior = null;
            while (null != (token = token.Next()) && !(token is FunctionPathToken)) prior = token;
            // Invert the relationship $..path.function() to $.function($..path)
            if (token is FunctionPathToken functionPathToken)
            {
                prior.SetNext(null);
                path.SetTail(prior);

                // Now generate a new parameter from our path
                var parameter = new Parameter();
                parameter.Path = new CompiledPath(path, true);
                parameter.ParameterType = ParameterTypeEnum.Path;
                functionPathToken.Parameters = new List<Parameter> { parameter };
                var functionRoot = new RootPathToken('$');
                functionRoot.SetTail(token);
                functionRoot.SetNext(token);

                // Define the function as the root
                return functionRoot;
            }
        }

        return path;
    }


    public override string ToString()
    {
        return _root.ToString();
    }

    public RootPathToken GetRoot()
    {
        return _root;
    }
}