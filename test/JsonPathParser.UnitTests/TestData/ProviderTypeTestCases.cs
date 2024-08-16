using XavierJefferson.JsonPathParser.Mapper;
using XavierJefferson.JsonPathParser.Provider;
using XavierJefferson.JsonPathParser.UnitTests.Enums;

namespace XavierJefferson.JsonPathParser.UnitTests.TestData;

internal class ProviderTypeTestCases : TheoryData<ProviderTypeTestCase>
{
    public static Dictionary<ProviderTypeEnum, Configuration> RootData = new()
    {
        {
            ProviderTypeEnum.NewtonsoftJson, Configuration.DefaultConfiguration()
        },
        {
            ProviderTypeEnum.SystemTextJson, Configuration.DefaultConfiguration()
                .SetJsonProvider<SystemTextJsonProvider>().SetMappingProvider<SystemTextJsonMappingProvider>()
        }
    };

    public static Dictionary<ProviderTypeEnum, ProviderTypeTestCase> Cases =
        RootData.ToDictionary(i => i.Key, i => new ProviderTypeTestCase(i.Key, i.Value));

    public ProviderTypeTestCases()
    {
        foreach (var type in Cases) Add(type.Value);
    }
}