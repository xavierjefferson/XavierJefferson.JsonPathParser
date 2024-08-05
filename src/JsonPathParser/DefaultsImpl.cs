using XavierJefferson.JsonPathParser.Interfaces;
using XavierJefferson.JsonPathParser.Mapper;
using XavierJefferson.JsonPathParser.Provider;

namespace XavierJefferson.JsonPathParser;

public class DefaultsImpl : IDefaults
{
    public static readonly DefaultsImpl Instance = new();

    private DefaultsImpl()
    {
    }


    public IJsonProvider JsonProvider => new NewtonsoftJsonProvider();


    public HashSet<Option> Options => new();


    public IMappingProvider MappingProvider { get; } = new NewtonsoftJsonMappingProvider();
}