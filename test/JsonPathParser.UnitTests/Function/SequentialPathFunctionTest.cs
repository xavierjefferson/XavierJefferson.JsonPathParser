namespace XavierJefferson.JsonPathParser.UnitTests.Function;

/**
 * Test cases for functions
 * 
 * -first
 * -last
 * -index(X)
 * 
 * Created by git9527 on 6/11/22.
 */
public class SequentialPathFunctionTest : BaseFunctionTest
{
    private readonly Configuration _conf = ConfigurationData.NewtonsoftJsonConfiguration;

    [Fact]
    public void TestFirstOfNumbers()
    {
        VerifyFunction(_conf, "$.numbers.first()", NumberSeries, 1d);
    }

    [Fact]
    public void TestLastOfNumbers()
    {
        VerifyFunction(_conf, "$.numbers.last()", NumberSeries, 10d);
    }

    [Fact]
    public void TestIndexOfNumbers()
    {
        VerifyFunction(_conf, "$.numbers.index(0)", NumberSeries, 1d);
        VerifyFunction(_conf, "$.numbers.index(-1)", NumberSeries, 10d);
        VerifyFunction(_conf, "$.numbers.index(1)", NumberSeries, 2d);
    }

    [Fact]
    public void TestFirstOfText()
    {
        VerifyFunction(_conf, "$.text.first()", TextSeries, "a");
    }

    [Fact]
    public void TestLastOfText()
    {
        VerifyFunction(_conf, "$.text.last()", TextSeries, "f");
    }

    [Fact]
    public void TestIndexOfText()
    {
        VerifyFunction(_conf, "$.text.index(0)", TextSeries, "a");
        VerifyFunction(_conf, "$.text.index(-1)", TextSeries, "f");
        VerifyFunction(_conf, "$.text.index(1)", TextSeries, "b");
    }
}