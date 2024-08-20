namespace XavierJefferson.JsonPathParser.Enums;

public enum ConfigurationOptionEnum
{
    /// <summary>
    ///     returns <code>null</code> for missing leaf.
    ///     <pre>
    ///         [
    ///         {
    ///         "foo" : "foo1",
    ///         "bar" : "bar1"
    ///         }
    ///         {
    ///         "foo" : "foo2"
    ///         }
    ///         ]
    ///         *
    ///     </pre>
    ///     * the path :
    ///     * "$[*].bar"
    ///     * Without flag ["bar1"] is returned
    ///     With flag ["bar1", null] is returned
    ///     *
    /// </summary>
    DefaultPathLeafToNull,

    /// <summary>
    ///     Makes this implementation more compliant to the Goessner spec. All results are returned as Lists.
    /// </summary>
    AlwaysReturnList,

    /// <summary>
    ///     Returns a list of path strings representing the path of the evaluation hits
    /// </summary>
    AsPathList,

    /// <summary>
    ///     Suppress all exceptions when evaluating path.
    ///     <br />
    ///     If an exception is thrown and the option {@link Option#ALWAYS_RETURN_LIST} an empty list is returned.
    ///     If an exception is thrown and the option {@link Option#ALWAYS_RETURN_LIST} is not present null is returned.
    /// </summary>
    SuppressExceptions,

    /// <summary>
    ///     Configures JsonPath to require properties defined in path when an <bold>indefinite</bold> path is evaluated.
    /// </summary>
    /// * Given:
    /// *
    /// <pre>
    ///     [
    ///     {
    ///     "a" : "a-val",
    ///     "b" : "b-val"
    ///     },
    ///     {
    ///     "a" : "a-val",
    ///     }
    ///     ]
    /// </pre>
    /// * evaluating the path "$[*].b"
    /// * If REQUIRE_PROPERTIES option is present PathNotFoundException is thrown.
    /// If REQUIRE_PROPERTIES option is not present ["b-val"] is returned.
    /// </summary>
    RequireProperties
}