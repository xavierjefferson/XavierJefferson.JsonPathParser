using XavierJefferson.JsonPathParser.Provider;

namespace XavierJefferson.JsonPathParser.Extensions;

public static class ConfigurationBuilderExtensions
{
    public static ConfigurationBuilder WithProviderType<TV>(this ConfigurationBuilder q)
        where TV : ProviderType, new()
    {
        new TV().Configure(q);
        return q;
    }
}