using XavierJefferson.JsonPathParser.Enums;

namespace XavierJefferson.JsonPathParser.Interfaces;

/// <summary>
///     A delegate that can be registered on a <see cref="Configuration"/> that is called when a
///     result is added to the result of this path evaluation.
/// </summary>
public delegate EvaluationContinuationEnum EvaluationCallback(IFoundResult found);