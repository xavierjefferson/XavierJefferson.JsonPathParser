using XavierJefferson.JsonPathParser.Filtering.ValueNodes;
using Xunit.Abstractions;

namespace XavierJefferson.JsonPathParser.UnitTests.TestData;

public class RegexTestCase : IXunitSerializable
{
    public RegexTestCase()
    {
    }

    public RegexTestCase(string pattern, ValueNode valueNode, bool expectedResult)
    {
        Pattern = pattern;
        ValueNode = valueNode;
        ExpectedResult = expectedResult;
    }

    public string Pattern { get; private set; }
    public ValueNode ValueNode { get; private set; }
    public bool ExpectedResult { get; private set; }

    public void Deserialize(IXunitSerializationInfo info)
    {
        Pattern = info.GetValue<string>(nameof(Pattern));
        ValueNode = RegexTestCases.RegExpTestCaseDictionary[Pattern];
        ExpectedResult = info.GetValue<bool>(nameof(ExpectedResult));
    }

    public void Serialize(IXunitSerializationInfo info)
    {
        info.AddValue(nameof(Pattern), Pattern);
        info.AddValue(nameof(ExpectedResult), ExpectedResult);
    }
}