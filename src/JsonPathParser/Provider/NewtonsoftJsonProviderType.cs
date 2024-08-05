using XavierJefferson.JsonPathParser.Interfaces;
using XavierJefferson.JsonPathParser.Mapper;

namespace XavierJefferson.JsonPathParser.Provider;

public class NewtonsoftJsonProviderType : ProviderType
{
    public override IMappingProvider GetJsonMapper()
    {
        return new NewtonsoftJsonMappingProvider();
    }

    public override IJsonProvider GetJsonProvider()
    {
        return new NewtonsoftJsonProvider();
    }
}