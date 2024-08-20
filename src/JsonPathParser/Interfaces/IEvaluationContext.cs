using XavierJefferson.JsonPathParser.PathRefs;

namespace XavierJefferson.JsonPathParser.Interfaces;

public interface IEvaluationContext
{
    ///<summary>
    ///</summary>
    ///<returns> the configuration used for this evaluation</returns>
    Configuration Configuration { get; }

    /// <summary>
    ///     The json document that is evaluated
    /// </summary>
    /// <returns> the document</returns>
    object? RootDocument { get; }

    ICollection<PathRef> UpdateOperations { get; }

    /// <summary>
    ///     This method does not adhere to configuration settings. It will return a single object (not wrapped in a List) even
    ///     if the
    ///     configuration contains the {@link com.jayway.jsonpath.Option#ALWAYS_RETURN_LIST}
    /// </summary>



   
    ///     <returns> evaluation result</returns>
    object? GetValue();

    /// <summary>
    ///     See {@link com.jayway.jsonpath.@public.EvaluationContext#getValue()}
    /// </summary>
    /// <param name="unwrap">tells th underlying json provider if primitives should be unwrapped</param>



   
    ///     <returns> evaluation result</returns>
    object? GetValue(bool unwrap);

    /// <summary>
    ///     Returns the list of formalized paths that represent the result of the evaluation
    ///     <param name="
    ///     
    ///     
    ///         ">///
    ///         <returns> list of paths</returns></param>
    T? GetPath<T>();


    /// <summary>
    ///     Convenience method to get list of hits as string path representations
    /// </summary>
    /// <returns> list of path representations</returns>
    IList<string> GetPathList();
}