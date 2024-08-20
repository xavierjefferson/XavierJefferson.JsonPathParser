using System.Collections.Immutable;
using XavierJefferson.JsonPathParser.Enums;
using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser;
/// <summary>
///     Configuration builder
/// </summary>
public class ConfigurationBuilder
{
    private ReadOnlySet<ConfigurationOptionEnum> _options = new ReadOnlySet<ConfigurationOptionEnum>(new HashSet<ConfigurationOptionEnum>());
    private List<EvaluationCallback> _evaluationCallbacks = new();
    public IMappingProvider? MappingProvider { get; private set; }

    public IJsonProvider? JsonProvider { get; private set; }

    public ConfigurationBuilder WithJsonProvider(IJsonProvider? provider)
    {
        JsonProvider = provider;
        return this;
    }

    public ConfigurationBuilder WithMappingProvider(IMappingProvider? provider)
    {
        MappingProvider = provider;
        return this;
    }

    public ConfigurationBuilder WithOptions(params ConfigurationOptionEnum[] flags)
    {
        this._options = new ReadOnlySet<ConfigurationOptionEnum>(flags);
        return this;
    }

    public ConfigurationBuilder WithOptions(IEnumerable<ConfigurationOptionEnum> options)
    {
        this._options = new ReadOnlySet<ConfigurationOptionEnum>(options);
        return this;
    }

    public ConfigurationBuilder WithEvaluationCallbacks(params EvaluationCallback[] callbacks)
    {
        _evaluationCallbacks = callbacks.ToList();
        return this;
    }

    public ConfigurationBuilder WithEvaluationCallbacks(ICollection<EvaluationCallback>? callbacks)
    {
        _evaluationCallbacks = callbacks == null
            ? new List<EvaluationCallback>()
            : new List<EvaluationCallback>(callbacks);
        return this;
    }

    public Configuration Build()
    {
        if (JsonProvider == null || MappingProvider == null)
        {
            var defaults = Configuration.GetEffectiveDefaults();
            if (JsonProvider == null) JsonProvider = defaults.JsonProvider;
            if (MappingProvider == null) MappingProvider = defaults.MappingProvider;
        }

        return new Configuration(JsonProvider, MappingProvider, _options, _evaluationCallbacks);
    }
}