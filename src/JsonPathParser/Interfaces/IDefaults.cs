namespace XavierJefferson.JsonPathParser.Interfaces;

public interface IDefaults
{
    /// <summary>
    ///     Returns the default <see cref="json."/>
    ///     <returns> default json provider</returns>
    IJsonProvider JsonProvider { get; }

    /// <summary>
    ///     Returns default setOptions
    ///     <returns> setOptions</returns>
    HashSet<Option> Options { get; }

    /// <summary>
    ///     Returns the default <see cref="MappingProvider"/>
    /// </summary>
    /// <returns> default mapping provider</returns>
    IMappingProvider MappingProvider { get; }
}