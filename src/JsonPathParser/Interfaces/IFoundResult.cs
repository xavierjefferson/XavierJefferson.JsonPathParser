namespace XavierJefferson.JsonPathParser.Interfaces;

///<summary>
///</summary>
public interface IFoundResult
{
    /// <summary>
    ///     the index of this result. First result i 0
    ///     ///
    /// </summary>
    /// <returns> index</returns>
    int Index { get; }

    /// <summary>
    ///     The path of this result
    ///     ///
    /// </summary>
    /// <returns> path</returns>
    string Path { get; }

    /// <summary>
    ///     The result object
    ///     ///
    /// </summary>
    /// <returns> the result object</returns>
    object? Result { get; }
}