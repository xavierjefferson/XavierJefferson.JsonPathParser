using XavierJefferson.JsonPathParser.Exceptions;
using XavierJefferson.JsonPathParser.Helpers;

namespace XavierJefferson.JsonPathParser.UnitTests.Internal;

public class UtilsTest
{
   

    [Fact]
    public void TestEscape()
    {
        Assert.Null(StringHelper.Escape(null, true));

        Assert.Equal("\\\\f\\'o\\\"o\\rb\\fa\\t\\nr\\bb\\/a", StringHelper.Escape("\\f\'o\"o\rb\fa\t\nr\bb/a", true));
        Assert.Equal("\\uFFFF\\u0FFF\\u00FF\\u000F\\u0010",
            StringHelper.Escape("\uffff\u0fff\u00ff\u000f\u0010", true));
    }

    [Fact]
    public void TestUnescape()
    {
        Assert.Null(StringHelper.Unescape(null));

        Assert.Equal("foo", StringHelper.Unescape("foo"));
        Assert.Equal("\\", StringHelper.Unescape("\\"));
        Assert.Equal("\\", StringHelper.Unescape("\\\\"));
        Assert.Equal("\'", StringHelper.Unescape("\\\'"));
        Assert.Equal("\"", StringHelper.Unescape("\\\""));
        Assert.Equal("\r", StringHelper.Unescape("\\r"));
        Assert.Equal("\f", StringHelper.Unescape("\\f"));
        Assert.Equal("\t", StringHelper.Unescape("\\t"));
        Assert.Equal("\n", StringHelper.Unescape("\\n"));
        Assert.Equal("\b", StringHelper.Unescape("\\b"));
        Assert.Equal("a", StringHelper.Unescape("\\a"));
        Assert.Equal("\uffff", StringHelper.Unescape("\\uffff"));
    }

    [Fact]
    public void TestUnescapeThrow()
    {
        Assert.Throws<JsonPathException>(() => StringHelper.Unescape("\\uuuuu"));
    }

    [Fact]
    public void TestIsTrue()
    {
        Assertions.IsTrue(true, "foo");
    }

    [Fact]
    public void TestIsTrueThrow()
    {
        Assert.Throws<ArgumentException>(() => Assertions.IsTrue(false, "foo"));
    }

    [Fact]
    public void testOnlyOneIsTrueThrow1()
    {
        Assert.Throws<ArgumentException>(() => Assertions.OnlyOneIsTrue("foo", false, false));
    }

    [Fact]
    public void testOnlyOneIsTrueThrow2()
    {
        Assert.Throws<ArgumentException>(() => Assertions.OnlyOneIsTrue("foo", true, true));
    }

    [Fact]
    public void testOnlyOneIsTrueNonThrow()
    {
        Assert.True(Assertions.OnlyOneIsTrueNonThrow(true));

        Assert.False(Assertions.OnlyOneIsTrueNonThrow(true, true, true));
        Assert.False(Assertions.OnlyOneIsTrueNonThrow(true, true, false));
        Assert.False(Assertions.OnlyOneIsTrueNonThrow(false, false, false));
    }
}