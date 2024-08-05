using XavierJefferson.JsonPathParser.Interfaces;
using XavierJefferson.JsonPathParser.Mapper;

namespace XavierJefferson.JsonPathParser.Provider;

public class SystemTextJsonProviderType : ProviderType
{
    public override IMappingProvider GetJsonMapper()
    {
        return new SystemTextJsonMappingProvider();
    }

    public override IJsonProvider GetJsonProvider()
    {
        return new SystemTextJsonProvider();
    }
}