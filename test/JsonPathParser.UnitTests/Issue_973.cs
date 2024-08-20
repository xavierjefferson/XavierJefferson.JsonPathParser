using XavierJefferson.JsonPathParser.Filtering;
using XavierJefferson.JsonPathParser.UnitTests.TestData;

namespace XavierJefferson.JsonPathParser.UnitTests;

public class Issue_973
{
    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void shouldNotCauseStackOverflow(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var _ = Criteria.Parse(jsonProvider, "@[\"\",/\\");
    }
}