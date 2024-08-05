namespace XavierJefferson.JsonPathParser.Interfaces;

/// <summary>
///     Maps object between different Types
/// </summary>
public interface IMappingProvider
{
    /// <summary>
    /// </summary>
    /// <param name="source">object to map</param>
    /// <param name="targetType">the type the source object should be mapped to</param>
    /// <param name="configuration">current configuration</param>
    /// <returns> return the mapped object</returns>
    object? Map(object? source, Type targetType, Configuration configuration);

    /// <summary>
    /// </summary>
    /// <param name="source">object to map</param>
    /// <param name="configuration">current configuration</param>
    /// <returns> return the mapped object</returns>
    object? Map<T>(object? source, Configuration configuration);
}