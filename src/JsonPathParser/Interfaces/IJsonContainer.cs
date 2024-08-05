namespace XavierJefferson.JsonPathParser.Interfaces;

public interface IJsonContainer
{
    /// <summary>
    ///     Returns the JSON model that this context is operating on as a JSON string
    /// </summary>
    /// <returns> json model as string</returns>
    string? JsonString { get; }

    /// <summary>
    ///     Returns the JSON model that this context is operating on
    /// </summary>
    /// <returns> json model</returns>
    object? Json { get; }
}