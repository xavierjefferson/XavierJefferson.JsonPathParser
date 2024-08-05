using XavierJefferson.JsonPathParser.UnitTests.Enums;
using Xunit.Abstractions;

namespace XavierJefferson.JsonPathParser.UnitTests.TestData;

public class ProviderTypeTestCase : IProviderTypeTestCase
{
    public ProviderTypeTestCase()
    {
    }

    public ProviderTypeTestCase(ProviderTypeEnum providerTypeValue, Configuration? provider)
    {
        ProviderTypeValue = providerTypeValue;
        Configuration = provider;
    }

    public ProviderTypeEnum ProviderTypeValue { get; private set; }


    public Configuration? Configuration { get; private set; }

    #region Implementation of IXunitSerializable

    /// <inheritdoc />
    public void Deserialize(IXunitSerializationInfo info)
    {
        ProviderTypeValue = info.GetValue<ProviderTypeEnum>(nameof(ProviderTypeValue));
        switch (ProviderTypeValue)
        {
            case ProviderTypeEnum.NewtonsoftJson:
                Configuration = ConfigurationData.NewtonsoftJsonConfiguration;
                break;
            case ProviderTypeEnum.SystemTextJson:
                Configuration = ConfigurationData.SystemTextJsonConfiguration;
                break;
            default:
                Configuration = null;
                break;
        }
    }

    /// <inheritdoc />
    public void Serialize(IXunitSerializationInfo info)
    {
        info.AddValue(nameof(ProviderTypeValue), ProviderTypeValue);
    }

    #endregion
}