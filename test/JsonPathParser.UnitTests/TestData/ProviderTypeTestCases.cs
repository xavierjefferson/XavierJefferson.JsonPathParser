using System.Collections.ObjectModel;
using XavierJefferson.JsonPathParser.UnitTests.Enums;

namespace XavierJefferson.JsonPathParser.UnitTests.TestData;

internal class ProviderTypeTestCases : TheoryData<ProviderTypeTestCase>
{
    public static ReadOnlyCollection<ProviderTypeTestCase> Cases = new(new List<ProviderTypeTestCase>
    {
        new(ProviderTypeEnum.NewtonsoftJson, ConfigurationData.NewtonsoftJsonConfiguration),
        new(ProviderTypeEnum.SystemTextJson, ConfigurationData.SystemTextJsonConfiguration)
    });

    public ProviderTypeTestCases()
    {
        foreach (var testCase in Cases) Add(testCase);
    }
}