using XavierJefferson.JsonPathParser.UnitTests.TestData;

namespace XavierJefferson.JsonPathParser.UnitTests.Function;

/**
 * Author: Sergey Saiyan sergey.sova42@gmail.com
 * Created at 21/02/2018.
 */
public class KeySetFunctionTest : BaseFunctionTest
{
    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void TestKeySet(IProviderTypeTestCase testCase)
    {
        using (var s = new StreamReader(GetResourceAsStream("keyset.json")))
        {
            var json = s.ReadToEnd();
            VerifyFunction(testCase.Configuration, "$.data.keys()", json, new List<string> { "a", "b" });
        }
    }
}