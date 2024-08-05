namespace XavierJefferson.JsonPathParser.UnitTests.Function;

/**
 * Author: Sergey Saiyan sergey.sova42@gmail.com
 * Created at 21/02/2018.
 */
public class KeySetFunctionTest : BaseFunctionTest
{
    private readonly Configuration _conf = ConfigurationData.NewtonsoftJsonConfiguration;

    [Fact]
    public void TestKeySet()
    {
        using (var s = new StreamReader(GetResourceAsStream("keyset.json")))
        {
            var json = s.ReadToEnd();
            VerifyFunction(_conf, "$.data.keys()", json, new JpObjectList { "a", "b" });
        }
    }
}