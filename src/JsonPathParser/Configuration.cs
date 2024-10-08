using System.Collections.ObjectModel;
using XavierJefferson.JsonPathParser.Enums;
using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser;

/// <summary>
///     Immutable configuration object
/// </summary>
public class Configuration
{
    private static IDefaults? _defaults;

    internal Configuration(IJsonProvider jsonProvider, IMappingProvider mappingProvider,
        ReadOnlySet<ConfigurationOptionEnum> options,
        IEnumerable<EvaluationCallbackDelegate> evaluationCallbacks)
    {
        ArgumentNullException.ThrowIfNull(evaluationCallbacks);
        JsonProvider = jsonProvider ?? throw new ArgumentNullException(nameof(jsonProvider));
        MappingProvider = mappingProvider ?? throw new ArgumentNullException(nameof(mappingProvider));
        ArgumentNullException.ThrowIfNull(options);
        Options = options;
        EvaluationCallbacks = new ReadOnlyCollection<EvaluationCallbackDelegate>(evaluationCallbacks.ToList());
    }

    /// <summary>
    ///     Returns the evaluation callbacks registered in this configuration
    /// </summary>
    /// <returns> the evaluation callbacks</returns>
    public ReadOnlyCollection<EvaluationCallbackDelegate>? EvaluationCallbacks { get; }

    /// <summary>
    ///     Returns <see cref="IJsonProvider" /> used by this configuration
    /// </summary>
    /// <returns> jsonProvider used</returns>
    public IJsonProvider JsonProvider { get; }

    /// <summary>
    ///     Returns <see cref="IMappingProvider" /> used by this configuration
    /// </summary>
    /// <returns> mappingProvider used</returns>
    public IMappingProvider MappingProvider { get; }

    /// <summary>
    ///     Returns the options used by this configuration
    /// </summary>
    /// <returns> the new configuration instance</returns>
    public ReadOnlySet<ConfigurationOptionEnum> Options { get; }

    /// <summary>
    ///     HashSet Default configuration
    /// </summary>
    /// <param name="defaults">default configuration settings</param>
    public static void SetDefaults(IDefaults defaults)
    {
        _defaults = defaults;
    }

    internal static IDefaults GetEffectiveDefaults()
    {
        if (_defaults == null)
            return DefaultsImpl.Instance;
        return _defaults;
    }

    /// <summary>
    ///     Creates a new Configuration by the provided evaluation listeners to the current listeners
    ///     <param name="evaluationListener">listeners</param>
    ///     <returns> a new configuration</returns>
    public Configuration AddEvaluationCallbacks(params EvaluationCallbackDelegate[] evaluationListener)
    {
        return CreateBuilder().WithJsonProvider(JsonProvider).WithMappingProvider(MappingProvider).WithOptions(Options)
            .WithEvaluationCallbacks(EvaluationCallbacks.Union(evaluationListener).ToArray()).Build();
    }

    /// <summary>
    ///     Creates a new Configuration with the provided evaluation listeners
    /// </summary>
    /// <param name="evaluationListener">listeners</param>
    /// <returns> a new configuration</returns>
    public Configuration SetEvaluationCallbacks(params EvaluationCallbackDelegate[] evaluationListener)
    {
        return CreateBuilder().WithJsonProvider(JsonProvider).WithMappingProvider(MappingProvider).WithOptions(Options)
            .WithEvaluationCallbacks(evaluationListener).Build();
    }


    /// <summary>
    ///     Creates a new Configuration based on the given <see cref="IJsonProvider" />
    /// </summary>
    /// <param name="newJsonProvider">json provider to use in new configuration</param>
    /// <returns> a new configuration</returns>
    public Configuration SetJsonProvider(IJsonProvider newJsonProvider)
    {
        return CreateBuilder().WithJsonProvider(newJsonProvider).WithMappingProvider(MappingProvider)
            .WithOptions(Options)
            .WithEvaluationCallbacks(EvaluationCallbacks).Build();
    }

    public Configuration SetJsonProvider<T>() where T : IJsonProvider, new()
    {
        return SetJsonProvider(new T());
    }

    /// <summary>
    ///     Creates a new Configuration based on the given <see cref="IMappingProvider" />
    ///     <param name="newMappingProvider">mapping provider to use in new configuration</param>
    /// </summary>
    /// <returns> a new configuration</returns>
    public Configuration SetMappingProvider(IMappingProvider newMappingProvider)
    {
        return CreateBuilder().WithJsonProvider(JsonProvider).WithMappingProvider(newMappingProvider)
            .WithOptions(Options)
            .WithEvaluationCallbacks(EvaluationCallbacks).Build();
    }

    public Configuration SetMappingProvider<T>() where T : IMappingProvider, new()
    {
        return SetMappingProvider(new T());
    }

    /// <summary>
    ///     Creates a new configuration by adding the new options to the options used in this configuration.
    /// </summary>
    /// <param name="options">options to.Add</param>
    /// <returns> a new configuration</returns>
    public Configuration AddOptions(params ConfigurationOptionEnum[] options)
    {
        var opts = Options.Union(options);
        return CreateBuilder().WithJsonProvider(JsonProvider).WithMappingProvider(MappingProvider).WithOptions(opts)
            .WithEvaluationCallbacks(EvaluationCallbacks).Build();
    }

    /// <summary>
    ///     Creates a new configuration with the provided options. Options in this configuration are discarded.
    /// </summary>
    /// <param name="options">></param>
    //////<returns> the new configuration instance</returns>
    public Configuration SetOptions(params ConfigurationOptionEnum[] options)
    {
        var a = CreateBuilder().WithJsonProvider(JsonProvider).WithMappingProvider(MappingProvider);
        return a.WithOptions(options).WithEvaluationCallbacks(EvaluationCallbacks).Build();
    }


    /// <summary>
    ///     Check if this configuration contains the given option
    /// </summary>
    /// <param name="configurationOption">option to check</param>
    /// <returns> true if configurations contains option</returns>
    public bool ContainsOption(ConfigurationOptionEnum configurationOption)
    {
        return Options.Contains(configurationOption);
    }

    /// <summary>
    ///     Creates a new configuration based on default values
    /// </summary>
    /// <returns> a new configuration based on defaults</returns>
    public static Configuration DefaultConfiguration()
    {
        var defaults = GetEffectiveDefaults();
        return CreateBuilder().WithJsonProvider(defaults.JsonProvider).WithOptions(defaults.Options).Build();
    }

    /// <summary>
    ///     Returns a new ConfigurationBuilder
    /// </summary>
    /// <returns> a builder</returns>
    public static ConfigurationBuilder CreateBuilder()
    {
        return new ConfigurationBuilder();
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(JsonProvider, MappingProvider, Options);
    }

    public override bool Equals(object? o)
    {
        if (this == o) return true;
        if (o is Configuration configuration)
            return JsonProvider.GetType() == configuration.JsonProvider.GetType() &&
                   MappingProvider.GetType() == configuration.MappingProvider.GetType() &&
                   Equals(Options, configuration.Options);
        return false;
    }
}