using XavierJefferson.JsonPathParser.Filtering;
using XavierJefferson.JsonPathParser.UnitTests.TestData;

namespace XavierJefferson.JsonPathParser.UnitTests.Filtering;

public class PatternFlagTest : TestUtils
{
    [Theory]
    [ClassData(typeof(PatternFlagTestCases))]
    public void TestParseFlags(PatternFlagTestCase testCase)
    {
        Assert.Equal(testCase.RegexOptions, RegexFlag.ParseFlags(testCase.Pattern));
    }
}