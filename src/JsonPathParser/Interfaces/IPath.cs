namespace XavierJefferson.JsonPathParser.Interfaces;

/// <summary>
///     */
public interface IPath
{
    /// <summary>
    ///     Evaluates this path
    /// </summary>
    /// <param name="document">the json document to apply the path on</param>
    /// <param name="rootDocument">the root json document that started this evaluation</param>
    /// <param name="configuration">configuration to use</param>
    /// <param name="forUpdate">is this a read or a Write operation</param>
    /// <returns> EvaluationContext containing results of evaluation</returns>
    IEvaluationContext Evaluate(object? document, object? rootDocument, Configuration configuration,
        bool forUpdate = false);

    /// <summary>
    /// </summary>
    /// <returns> true id this path is definite</returns>
    bool IsDefinite();

    /// <summary>
    /// </summary>
    /// <returns> true id this path is a function</returns>
    bool IsFunctionPath();

    /// <summary>
    /// </summary>
    /// <returns> true id this path is starts with '$' and false if the path starts with '@'</returns>
    bool IsRootPath();
}