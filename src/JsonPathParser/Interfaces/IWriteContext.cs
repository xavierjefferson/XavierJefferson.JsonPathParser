using System;

namespace XavierJefferson.JsonPathParser.Interfaces;

public interface IWriteContext : IJsonContainer
{
    /// <summary>
    ///     Returns the configuration used for reading
    /// </summary>
    /// <returns> an immutable configuration</returns>
    Configuration Configuration { get; }


    /// <summary>
    ///     HashSet the value a the given path
    /// </summary>
    /// <param name="path">path to set</param>
    /// <param name="newValue">new value</param>
    /// <param name="filters">filters</param>
    /// <returns> a document context</returns>
    IDocumentContext Set(string path, object? newValue, params IPredicate[] filters);

    /// <summary>
    ///     HashSet the value a the given path
    /// </summary>
    /// <param name="path">path to set</param>
    /// <param name="newValue">new value</param>
    /// <returns> a document context</returns>
    IDocumentContext Set(JsonPath path, object? newValue);

    /// <summary>
    ///     Replaces the value on the given path with the result of the {@link MapFunction}.
    /// </summary>
    /// <param name="path">path to be converted set</param>
    /// <param name="mapFunction">Converter object to be invoked</param>
    /// <param name="filters">filters</param>
    /// <returns> a document context</returns>
    IDocumentContext Map(string path, MapDelegate mapFunction, params IPredicate[] filters);

    /// <summary>
    ///     Replaces the value on the given path with the result of the {@link MapFunction}.
    /// </summary>
    /// <param name="path">path to be converted set</param>
    /// <param name="mapFunction">Converter object to be invoked (or lambda:))</param>
    /// <returns> a document context</returns>
    IDocumentContext? Map(JsonPath path, MapDelegate mapFunction);

    /// <summary>
    ///     Deletes the given path
    /// </summary>
    /// <param name="path">path to delete</param>
    /// <param name="filters">filters</param>
    /// <returns> a document context</returns>
    IDocumentContext Delete(string path, params IPredicate[] filters);

    /// <summary>
    ///     Deletes the given path
    /// </summary>
    /// <param name="path">path to delete</param>
    /// <returns> a document context</returns>
    IDocumentContext Delete(JsonPath path);

    /// <summary>
    ///     Add value to array
    ///     <pre>
    ///         <code>
    ///  MyList<int>
    ///                 array = new MyList
    ///                 <int>
    ///                     (){{
    ///                     .Add(0);
    ///                     .Add(1);
    ///                     }};
    ///                     * JsonPath.Parse(array).Add("$", 2);
    ///                     * assertThat(array).containsExactly(0,1,2);
    ///  </code>
    ///     </pre>
    /// </summary>
    /// <param name="path">path to array</param>
    /// <param name="value">value to.Add</param>
    /// <param name="filters">filters</param>
    /// <returns> a document context</returns>
    IDocumentContext Add(string path, object? value, params IPredicate[] filters);

    /// <summary>
    ///     Add value to array at the given path
    /// </summary>
    /// <param name="path">path to array</param>
    /// <param name="value">value to.Add</param>
    /// <returns> a document context</returns>
    IDocumentContext Add(JsonPath path, object? value);

    /// <summary>
    ///     Add or update the key with a the given value at the given path
    /// </summary>
    /// <param name="path">path to object</param>
    /// <param name="key">key to.Add</param>
    /// <param name="value">value of key</param>
    /// <param name="filters">filters</param>
    /// <returns> a document context</returns>
    IDocumentContext Add(string path, string key, object? value, params IPredicate[] filters);

    /// <summary>
    ///     Add or update the key with a the given value at the given path
    /// </summary>
    /// <param name="path">path to array</param>
    /// <param name="key">key to.Add</param>
    /// <param name="value">value of key</param>
    /// <returns> a document context</returns>
    IDocumentContext Add(JsonPath path, string key, object? value);

    /// <summary>
    ///     Renames the last key element of a given path.
    /// </summary>
    /// <param name="path">The path to the old key. Should be resolved to a map</param>
    /// or an array including map items.
    /// <param name="oldKeyName">The old key name.</param>
    /// <param name="newKeyName">The new key name.</param>
    /// <param name="filters">filters.</param>
    /// <returns> a document content.</returns>
    IDocumentContext RenameKey(string path, string oldKeyName, string newKeyName, params IPredicate[] filters);

    /// <summary>
    ///     Renames the last key element of a given path.
    /// </summary>
    /// <param name="path">The path to the old key. Should be resolved to a map</param>
    /// or an array including map items.
    /// <param name="oldKeyName">The old key name.</param>
    /// <param name="newKeyName">The new key name.</param>
    /// <returns> a document content.</returns>
    IDocumentContext RenameKey(JsonPath path, string oldKeyName, string newKeyName);

    ///<summary>Add or update the key with a the given value at the given path</summary>
    ///<param name="filters">filters</param>
    ///<param name="key"  > key to add</param>
    ///<param name="path"> path to object </param>
    ///<param name="value">value of key</param>
    ///<returns>a document context</returns>
    IDocumentContext Put(String path, String key, Object value, params IPredicate[] filters);

  
    ///<returns> a document context</returns>
    ///<summary> Add or update the key with a the given value at the given path</summary>
    ///<param name="value"> value of key</param>
    ///<param name="path"> path to array</param>
    ///<param name="key">  key to add</param>
    IDocumentContext Put(JsonPath path, String key, Object value);
}