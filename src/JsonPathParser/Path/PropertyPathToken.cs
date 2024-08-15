using System.Diagnostics;
using System.Text;
using XavierJefferson.JsonPathParser.Exceptions;
using XavierJefferson.JsonPathParser.Helpers;
using XavierJefferson.JsonPathParser.PathRefs;

namespace XavierJefferson.JsonPathParser.Path;

/// <summary>
///     */
public class PropertyPathToken : PathToken
{
    private readonly SerializingList<string> _properties;
    private readonly string _stringDelimiter;

    public PropertyPathToken(SerializingList<string> properties, char stringDelimiter)
    {
        if (!properties.Any()) throw new InvalidPathException("Empty properties");
        _properties = properties;
        _stringDelimiter = stringDelimiter.ToString();
    }

    public SerializingList<string> GetProperties()
    {
        return _properties;
    }

    public bool SinglePropertyCase()
    {
        return _properties.Count() == 1;
    }

    public bool MultiPropertyMergeCase()
    {
        return IsLeaf() && _properties.Count() > 1;
    }

    public bool MultiPropertyIterationCase()
    {
        // Semantics of this case is the same as semantics of ArrayPathToken with INDEX_SEQUENCE operation.
        return !IsLeaf() && _properties.Count() > 1;
    }


    public override void Evaluate(string currentPath, PathRef parent, object? model, EvaluationContextImpl context)
    {
        // Can't assert it in ctor because isLeaf() could be changed later on.
        Debug.Assert(Assertions.OnlyOneIsTrueNonThrow(SinglePropertyCase(), MultiPropertyMergeCase(),
            MultiPropertyIterationCase()));

        if (!context.JsonProvider.IsMap(model))
        {
            if (!IsUpstreamDefinite()
                || context.Options.Contains(Option.SuppressExceptions))
                return;

            var m = model == null ? "null" : model.GetType().FullName;
            throw new PathNotFoundException(
                $"Expected to find an object with property {GetPathFragment()} in path {currentPath} but found '{m}'. " +
                $"This is not a json object according to the JsonProvider: '{context.Configuration.JsonProvider.GetType().FullName}'.");
        }

        if (SinglePropertyCase() || MultiPropertyMergeCase())
        {
            HandleObjectProperty(currentPath, model, context, _properties);
            return;
        }

        Debug.Assert(MultiPropertyIterationCase());
        var currentlyHandledProperty = new SerializingList<string>(1);
        currentlyHandledProperty.Add(null);
        foreach (var property in _properties)
        {
            currentlyHandledProperty[0] = property;
            HandleObjectProperty(currentPath, model, context, currentlyHandledProperty);
        }
    }


    public override bool IsTokenDefinite()
    {
        // in case of leaf multiprops will be merged, so it's kinda definite
        return SinglePropertyCase() || MultiPropertyMergeCase();
    }


    public override string GetPathFragment()
    {
        return new StringBuilder()
            .Append("[")
            .Append(StringHelper.Join(",", _stringDelimiter, _properties.ToArray()))
            .Append("]").ToString();
    }
}