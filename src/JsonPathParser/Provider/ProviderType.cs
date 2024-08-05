using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Provider;

public abstract class ProviderType
{
    public abstract IJsonProvider GetJsonProvider();
    public abstract IMappingProvider GetJsonMapper();

    public void Configure(ConfigurationBuilder builder)
    {
        builder.WithJsonProvider(GetJsonProvider());
        builder.WithMappingProvider(GetJsonMapper());
    }
}