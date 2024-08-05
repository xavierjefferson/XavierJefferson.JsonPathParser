using System.Globalization;
using XavierJefferson.JsonPathParser.Filtering;

namespace XavierJefferson.JsonPathParser.UnitTests.Filtering;

public class RelationalOperatorTest
{
    [Fact]
    public void TestFromStringWithEnglishLocale()
    {
        var ci = new CultureInfo("en-US");
        Thread.CurrentThread.CurrentCulture = ci;
        Thread.CurrentThread.CurrentUICulture = ci;
        Assert.Equal(RelationalOperator.In, RelationalOperator.FromString("in"));
        Assert.Equal(RelationalOperator.In, RelationalOperator.FromString("IN"));
    }

    [Fact]
    public void TestFromStringWithTurkishLocale()
    {
        var ci = new CultureInfo("tr");
        Thread.CurrentThread.CurrentCulture = ci;
        Thread.CurrentThread.CurrentUICulture = ci;
        Assert.Equal(RelationalOperator.In, RelationalOperator.FromString("in"));
        Assert.Equal(RelationalOperator.In, RelationalOperator.FromString("IN"));
    }
}