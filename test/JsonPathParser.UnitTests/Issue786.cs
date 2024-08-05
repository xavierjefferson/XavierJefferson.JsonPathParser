using XavierJefferson.JsonPathParser.UnitTests.TestData;

namespace XavierJefferson.JsonPathParser.UnitTests;

/**
 * test for issue 786
 */
public class Issue786 : TestUtils
{
    [Fact]
    public void Test()
    {
        Assert.Equal(4, BookLength());
        Assert.Equal(4, BookLength());
        Assert.Equal(4, BookLength());
    }

    private int BookLength()
    {
        return Convert.ToInt32(JsonPath.Read(JsonTestData.JsonDocument, "$..book.length()"));
    }
}