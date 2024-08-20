namespace XavierJefferson.JsonPathParser.Interfaces;

public interface IReadContext : IJsonContainer
{
    /// <summary>
    ///     Returns the configuration used for reading
    /// </summary>
    /// <returns> an immutable configuration</returns>
    Configuration Configuration { get; }


    /// <summary>
    ///     Reads the given path from this context
    /// </summary>
    /// <param name="path">path to read</param>
    /// <param name="filters">filters</param>
    /// ">///
    /// <returns> result</returns>
    /// </param>
    object? Read(string path, params IPredicate[] filters);

    /// <summary>
    ///     Reads the given path from this context
    /// </summary>
    /// <param name="path">path to read</param>
    /// <param name="type">expected return type (will try to map)</param>
    /// <param name="filters">filters</param>
    /// ">///
    /// <returns> result</returns>
    /// </param>
    object? Read(string path, Type type, params IPredicate[] filters);

    T? Read<T>(string path, params IPredicate[] filters);

    /// <summary>
    ///     Reads the given path from this context
    /// </summary>
    /// <param name="path">path to apply</param>
    /// ">///
    /// <returns> result</returns>
    /// </param>
    object? Read(JsonPath path);

    /// <summary>
    ///     Reads the given path from this context
    /// </summary>
    /// <param name="path">path to apply</param>
    /// <param name="type">expected return type (will try to map)</param>
    /// ">///
    /// <returns> result</returns>
    /// </param>
    object? Read(JsonPath path, Type type);

    /// <summary>
    ///     Reads the given path from this context
    /// </summary>
    /// Sample code to create a TypeRef
    /// <code>
    ///  TypeRef ref = new TypeRef<MyList<int>>() {};
    ///  </code>
    /// </summary>
    /// <param name="path">path to apply</param>
    /// <param name="typeRef">expected return type (will try to map)</param>
    /// ">///
    /// <returns> result</returns>
    /// </param>
    T? Read<T>(JsonPath path);

    /// <summary>
    ///     Reads the given path from this context
    /// </summary>
    /// Sample code to create a TypeRef
    /// <code>
    ///  TypeRef ref = new TypeRef<MyList<int>>() {};
    ///  </code>
    /// </summary>
    /// <param name="path">path to apply</param>
    /// <param name="typeRef">expected return type (will try to map)</param>
    /// ">///
    /// <returns> result</returns>
    /// </param>
    T? Read<T>(string path);

    /// <summary>
    ///     Stops evaluation when maxResults limit has been reached
    /// </summary>
    /// <param name="maxResults">
    ///     ///
    ///     <returns> the read context</returns>
    /// </param>
    IReadContext Limit(int maxResults);

    /// <summary>
    ///     Adds listener to the evaluation of this path
    /// </summary>
    /// <param name="listener">listeners to.Add</param>
    /// <returns> the read context</returns>
    IReadContext WithListeners(params EvaluationCallback[] listener);
}