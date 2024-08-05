namespace XavierJefferson.JsonPathParser.Interfaces;

public interface IPredicateContext
{
    /// <summary>
    ///     Returns the current item being evaluated by this predicate
    /// </summary>
    /// <returns>current document</returns>
    object? Item { get; }

    /// <summary>
    ///     Returns the root document (the complete JSON)
    /// </summary>
    /// <returns> root document</returns>
    object? Root { get; }

    /// <summary>
    ///     Configuration to use when evaluating
    /// </summary>
    /// <returns> configuration</returns>
    Configuration? Configuration { get; }

    /// <summary>
    ///     Returns the current item being evaluated by this predicate. It will be mapped
    ///     to the provided class
    ///     <returns> current document</returns>
    object? GetItem(Type type);

    T? GetItem<T>();
}