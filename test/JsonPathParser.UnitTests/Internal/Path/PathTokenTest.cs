using XavierJefferson.JsonPathParser.Path;

namespace XavierJefferson.JsonPathParser.UnitTests.Internal.Path;

public class PathTokenTest : TestUtils
{
    [Fact]
    public void is_upstream_definite_in_simple_case()
    {
        Assert.True(MakePathReturningTail(MakePpt("foo")).IsUpstreamDefinite());

        Assert.True(MakePathReturningTail(MakePpt("foo"), MakePpt("bar")).IsUpstreamDefinite());

        Assert.False(MakePathReturningTail(MakePpt("foo", "foo2"), MakePpt("bar")).IsUpstreamDefinite());

        Assert.False(MakePathReturningTail(new WildcardPathToken(), MakePpt("bar")).IsUpstreamDefinite());

        Assert.False(MakePathReturningTail(new ScanPathToken(), MakePpt("bar")).IsUpstreamDefinite());
    }

    [Fact]
    public void is_upstream_definite_in_complex_case()
    {
        Assert.True(MakePathReturningTail(MakePpt("foo"), MakePpt("bar"), MakePpt("baz")).IsUpstreamDefinite());

        Assert.True(MakePathReturningTail(MakePpt("foo"), new WildcardPathToken()).IsUpstreamDefinite());

        Assert.False(
            MakePathReturningTail(new WildcardPathToken(), MakePpt("bar"), MakePpt("baz")).IsUpstreamDefinite());
    }


    private PathToken MakePpt(params string[] properties)
    {
        return new PropertyPathToken(new SerializingList<string>(properties), '\'');
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