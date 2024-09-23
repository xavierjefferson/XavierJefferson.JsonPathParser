using XavierJefferson.JsonPathParser.Path;

namespace XavierJefferson.JsonPathParser.UnitTests.Internal.Path;

public class PathTokenTest : TestUtils
{
    [Fact]
    public void is_upstream_definite_in_simple_case()
    {
        Assert.True(MakePathReturningTail(MakePropertyPathToken("foo")).IsUpstreamDefinite());

        Assert.True(MakePathReturningTail(MakePropertyPathToken("foo"), MakePropertyPathToken("bar")).IsUpstreamDefinite());

        Assert.False(MakePathReturningTail(MakePropertyPathToken("foo", "foo2"), MakePropertyPathToken("bar")).IsUpstreamDefinite());

        Assert.False(MakePathReturningTail(new WildcardPathToken(), MakePropertyPathToken("bar")).IsUpstreamDefinite());

        Assert.False(MakePathReturningTail(new ScanPathToken(), MakePropertyPathToken("bar")).IsUpstreamDefinite());
    }

    [Fact]
    public void is_upstream_definite_in_complex_case()
    {
        Assert.True(MakePathReturningTail(MakePropertyPathToken("foo"), MakePropertyPathToken("bar"), MakePropertyPathToken("baz")).IsUpstreamDefinite());

        Assert.True(MakePathReturningTail(MakePropertyPathToken("foo"), new WildcardPathToken()).IsUpstreamDefinite());

        Assert.False(
            MakePathReturningTail(new WildcardPathToken(), MakePropertyPathToken("bar"), MakePropertyPathToken("baz")).IsUpstreamDefinite());
    }


    private PathToken MakePropertyPathToken(params string[] properties)
    {
        return new PropertyPathToken(new List<string?>(properties), '\'');
    }

    private PathToken? MakePathReturningTail(params PathToken[] tokens)
    {
        PathToken? last = null;
        foreach (var token in tokens)
        {
            if (last != null) last.AppendTailToken(token);
            last = token;
        }

        return last;
    }
}