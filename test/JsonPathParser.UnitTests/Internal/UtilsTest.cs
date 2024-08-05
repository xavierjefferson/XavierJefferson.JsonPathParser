using XavierJefferson.JsonPathParser.Exceptions;
using XavierJefferson.JsonPathParser.Helpers;

namespace XavierJefferson.JsonPathParser.UnitTests.Internal;

public class UtilsTest
{
    [Fact]
    public void TestConcat()
    {
        Assert.Equal("", StringHelper.Concat());
        Assert.Equal("", StringHelper.Concat(""));
        Assert.Equal("", StringHelper.Concat("", ""));
        Assert.Equal("a", StringHelper.Concat("a"));
        Assert.Equal("a", StringHelper.Concat("", "a", ""));
        Assert.Equal("abc", StringHelper.Concat("a", "b", "c"));
    }

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
    public void TestIsEmpty()
    {
        Assert.True(StringHelper.IsEmpty(null));
        Assert.True(StringHelper.IsEmpty(""));
        Assert.False(StringHelper.IsEmpty("foo"));
    }

    [Fact]
    public void TestIndexOf()
    {
        Assert.Equal(-1, StringHelper.IndexOf("bar", "foo", 0));
        Assert.Equal(-1, StringHelper.IndexOf("bar", "a", 2));
        Assert.Equal(1, StringHelper.IndexOf("bar", "a", 0));
        Assert.Equal(1, StringHelper.IndexOf("bar", "a", 1));
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
        Assert.Throws<ArgumentException>(() => Assertions.onlyOneIsTrue("foo", false, false));
    }

    [Fact]
    public void testOnlyOneIsTrueThrow2()
    {
        Assert.Throws<ArgumentException>(() => Assertions.onlyOneIsTrue("foo", true, true));
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