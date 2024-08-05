using XavierJefferson.JsonPathParser.UnitTests.Enums;
using Xunit.Abstractions;

namespace XavierJefferson.JsonPathParser.UnitTests.TestData;

public interface IProviderTypeTestCase : IXunitSerializable
{
    ProviderTypeEnum ProviderTypeValue { get; }
    Configuration? Configuration { get; }
}