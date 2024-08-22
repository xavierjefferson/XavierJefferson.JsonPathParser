using XavierJefferson.JsonPathParser.Enums;
using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser;

/// <summary>
///     Configuration builder
/// </summary>
public class ConfigurationBuilder
{
    private List<EvaluationCallbackDelegate> _evaluationCallbacks = new();
    private ReadOnlySet<ConfigurationOptionEnum> _options = new(new HashSet<ConfigurationOptionEnum>());
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
        _options = new ReadOnlySet<ConfigurationOptionEnum>(flags);
        return this;
    }

    public ConfigurationBuilder WithOptions(IEnumerable<ConfigurationOptionEnum> options)
    {
        _options = new ReadOnlySet<ConfigurationOptionEnum>(options);
        return this;
    }

    public ConfigurationBuilder WithEvaluationCallbacks(params EvaluationCallbackDelegate[] callbacks)
    {
        _evaluationCallbacks = callbacks.ToList();
        return this;
    }

    public ConfigurationBuilder WithEvaluationCallbacks(ICollection<EvaluationCallbackDelegate>? callbacks)
    {
        _evaluationCallbacks = callbacks == null
            ? new List<EvaluationCallbackDelegate>()
            : new List<EvaluationCallbackDelegate>(callbacks);
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