using System.Text.RegularExpressions;
using Xunit.Abstractions;

namespace XavierJefferson.JsonPathParser.UnitTests.TestData;

public class PatternFlagTestCase : IXunitSerializable
{
    public RegexOptions RegexOptions { get; set; }
    public string? Pattern { get; set; }
    public void Deserialize(IXunitSerializationInfo info)
    {
        RegexOptions = info.GetValue<RegexOptions>(nameof(RegexOptions));
        Pattern = info.GetValue<string>(nameof(Pattern));

    }

    public void Serialize(IXunitSerializationInfo info)
    {
        info.AddValue(nameof(RegexOptions), RegexOptions);
        info.AddValue(nameof(Pattern), Pattern);
    }
}