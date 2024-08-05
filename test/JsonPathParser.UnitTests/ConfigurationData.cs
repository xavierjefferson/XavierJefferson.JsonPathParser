using XavierJefferson.JsonPathParser.Mapper;
using XavierJefferson.JsonPathParser.Provider;

namespace XavierJefferson.JsonPathParser.UnitTests;

public class ConfigurationData
{
    public static readonly Configuration NewtonsoftJsonConfiguration = Configuration.DefaultConfiguration();

    public static readonly Configuration SystemTextJsonConfiguration = Configuration.DefaultConfiguration()
        .SetJsonProvider<SystemTextJsonProvider>().SetMappingProvider<SystemTextJsonMappingProvider>();
}