using XavierJefferson.JsonPathParser.Exceptions;
using XavierJefferson.JsonPathParser.Filtering;
using XavierJefferson.JsonPathParser.Path;
using XavierJefferson.JsonPathParser.UnitTests.Extensions;

namespace XavierJefferson.JsonPathParser.UnitTests;

public class PathCompilerTest : TestUtils
{
    [Fact]
    public void ARootPathMustBeFollowedByPeriodOrBracket()
    {
        Assert.Throws<InvalidPathException>(() => PathCompiler.Compile("$X"));
    }

    [Fact]
    public void ARootPathCanBeCompiled()
    {
        Assert.Equal("$", PathCompiler.Compile("$").ToString());
        Assert.Equal("@", PathCompiler.Compile("@").ToString());
    }

    [Fact]
    public void APathMayNotEndWithPeriod()
    {
        Assert.Throws<InvalidPathException>(() => PathCompiler.Compile("$."));
    }

    [Fact]
    public void APathMayNotEndWithPeriod2()
    {
        Assert.Throws<InvalidPathException>(() => PathCompiler.Compile("$.prop."));
    }

    [Fact]
    public void APathMayNotEndWithScan()
    {
        Assert.Throws<InvalidPathException>(() => PathCompiler.Compile("$.."));
    }

    [Fact]
    public void APathMayNotEndWithScan2()
    {
        Assert.Throws<InvalidPathException>(() => PathCompiler.Compile("$.prop.."));
    }

    [Fact]
    public void APropertyTokenCanBeCompiled()
    {
        Assert.Equal("$['prop']", PathCompiler.Compile("$.prop").ToString());
        Assert.Equal("$['1prop']", PathCompiler.Compile("$.1prop").ToString());
        Assert.Equal("$['@prop']", PathCompiler.Compile("$.@prop").ToString());
    }

    [Fact]
    public void ABracketNotationPropertyTokenCanBeCompiledd()
    {
        Assert.Equal("$['prop']", PathCompiler.Compile("$['prop']").ToString());
        Assert.Equal("$['1prop']", PathCompiler.Compile("$['1prop']").ToString());
        Assert.Equal("$['@prop']", PathCompiler.Compile("$['@prop']").ToString());
        Assert.Equal("$['@prop']", PathCompiler.Compile("$[  '@prop'  ]").ToString());
        Assert.Equal("$[\"prop\"]", PathCompiler.Compile("$[\"prop\"]").ToString());
    }

    [Fact]
    public void AMultiPropertyTokenCanBeCompiledd()
    {
        Assert.Equal("$['prop0','prop1']", PathCompiler.Compile("$['prop0', 'prop1']").ToString());
        Assert.Equal("$['prop0','prop1']", PathCompiler.Compile("$[  'prop0'  , 'prop1'  ]").ToString());
    }

    [Fact]
    public void APropertyChainCanBeCompiledd()
    {
        Assert.Equal("$['abc']", PathCompiler.Compile("$.abc").ToString());
        Assert.Equal("$['aaa']['bbb']", PathCompiler.Compile("$.aaa.bbb").ToString());
        Assert.Equal("$['aaa']['bbb']['ccc']", PathCompiler.Compile("$.aaa.bbb.ccc").ToString());
    }

    [Fact]
    public void APropertyMayNotContainBlanks()
    {
        Assert.Throws<InvalidPathException>(() => PathCompiler.Compile("$.foo bar"));
    }

    [Fact]
    public void AWildcardCanBeCompiled()
    {
        Assert.Equal("$[*]", PathCompiler.Compile("$.*").ToString());
        Assert.Equal("$[*]", PathCompiler.Compile("$[*]").ToString());
        Assert.Equal("$[*]", PathCompiler.Compile("$[ * ]").ToString());
    }

    [Fact]
    public void AWildcardCanFollowAProperty()
    {
        Assert.Equal("$['prop'][*]", PathCompiler.Compile("$.prop[*]").ToString());
        Assert.Equal("$['prop'][*]", PathCompiler.Compile("$['prop'][*]").ToString());
    }

    [Fact]
    public void AnArrayIndexPathCanBeCompiledd()
    {
        Assert.Equal("$[1]", PathCompiler.Compile("$[1]").ToString());
        Assert.Equal("$[1,2,3]", PathCompiler.Compile("$[1,2,3]").ToString());
        Assert.Equal("$[1,2,3]", PathCompiler.Compile("$[ 1 , 2 , 3 ]").ToString());
    }

    [Fact]
    public void AnArraySlicePathCanBeCompiledd()
    {
        Assert.Equal("$[-1:]", PathCompiler.Compile("$[-1:]").ToString());
        Assert.Equal("$[1:2]", PathCompiler.Compile("$[1:2]").ToString());
        Assert.Equal("$[:2]", PathCompiler.Compile("$[:2]").ToString());
    }

    [Fact]
    public void AnInlineCriteriaCanBeParsed()
    {
        Assert.Equal("$[?]", PathCompiler.Compile("$[?(@.foo == 'bar')]").ToString());
        Assert.Equal("$[?]", PathCompiler.Compile("$[?(@.foo == \"bar\")]").ToString());
    }

    [Fact]
    public void APlaceholderCriteriaCanBeParsed()
    {
        var predicate = SimplePredicate.Create(_ => false);
        Assert.Equal("$[?]", PathCompiler.Compile("$[?]", predicate).ToString());
        Assert.Equal("$[?,?]", PathCompiler.Compile("$[?,?]", predicate, predicate).ToString());
        Assert.Equal("$[?,?,?]", PathCompiler.Compile("$[?,?,?]", predicate, predicate, predicate).ToString());
    }

    [Fact]
    public void AScanTokenCanBeParsed()
    {
        Assert.Equal("$..['prop']..[*]", PathCompiler.Compile("$..['prop']..[*]").ToString());
    }

    [Fact]
    public void IssuePredicateCanHaveEscapedBackslashInProp()
    {
        var json = "{\n"
                   + "    \"logs\": [\n"
                   + "        {\n"
                   + "            \"message\": \"it\\\\\",\n"
                   + "            \"id\": 2\n"
                   + "        }\n"
                   + "    ]\n"
                   + "}";
        // message: it\ -> (after json escaping) -> "it\\" -> (after java escaping) -> "\"it\\\\\""

        var result = JsonPath.Read(json, "$.logs[?(@.message == 'it\\\\')].message").AsList();

        MyAssert.ContainsExactly(result, "it\\");
    }

    [Fact]
    public void IssuePredicateCanHaveBracketInRegex()
    {
        var json = "{\n"
                   + "    \"logs\": [\n"
                   + "        {\n"
                   + "            \"message\": \"(it\",\n"
                   + "            \"id\": 2\n"
                   + "        }\n"
                   + "    ]\n"
                   + "}";

        var result = JsonPath.Read(json, "$.logs[?(@.message =~ /\\(it/)].message").AsList();

        MyAssert.ContainsExactly(result, "(it");
    }

    [Fact]
    public void IssuePredicateCanHaveAndInRegex()
    {
        var json = "{\n"
                   + "    \"logs\": [\n"
                   + "        {\n"
                   + "            \"message\": \"it\",\n"
                   + "            \"id\": 2\n"
                   + "        }\n"
                   + "    ]\n"
                   + "}";

        var result = JsonPath.Read(json, "$.logs[?(@.message =~ /&&|it/)].message").AsList();

        MyAssert.ContainsExactly(result, "it");
    }

    [Fact]
    public void IssuePredicateCanHaveAndInProp()
    {
        var json = "{\n"
                   + "    \"logs\": [\n"
                   + "        {\n"
                   + "            \"message\": \"&& it\",\n"
                   + "            \"id\": 2\n"
                   + "        }\n"
                   + "    ]\n"
                   + "}";

        var result = JsonPath.Read(json, "$.logs[?(@.message == '&& it')].message").AsList();

        MyAssert.ContainsExactly(result, "&& it");
    }

    [Fact]
    public void IssuePredicateBracketsMustChangePriorities()
    {
        var json = "{\n"
                   + "    \"logs\": [\n"
                   + "        {\n"
                   + "            \"id\": 2\n"
                   + "        }\n"
                   + "    ]\n"
                   + "}";

        var result = JsonPath.Read(json, "$.logs[?(@.message && (@.id == 1 || @.id == 2))].id").AsList();

        Assert.Empty(result);

        var result2 = JsonPath.Read(json, "$.logs[?((@.id == 2 || @.id == 1) && @.message)].id").AsList();
        Assert.Empty(result2);
    }

    [Fact]
    public void IssuePredicateOrHasLowerPriorityThanAnd()
    {
        var json = "{\n"
                   + "    \"logs\": [\n"
                   + "        {\n"
                   + "            \"id\": 2\n"
                   + "        }\n"
                   + "    ]\n"
                   + "}";

        var result = JsonPath.Read(json, "$.logs[?(@.x && @.y || @.id)]").AsList();
        Assert.Single(result);
    }

    [Fact]
    public void IssuePredicateCanHaveDoubleQuotes()
    {
        var json = "{\n"
                   + "    \"logs\": [\n"
                   + "        {\n"
                   + "            \"message\": \"\\\"it\\\"\"\n"
                   + "        }\n"
                   + "    ]\n"
                   + "}";
        var result = JsonPath.Read(json, "$.logs[?(@.message == '\"it\"')].message").AsList();
        MyAssert.ContainsExactly(result, "\"it\"");
    }

    [Fact]
    public void IssuePredicateCanHaveSingleQuotes()
    {
        var json = "{\n"
                   + "    \"logs\": [\n"
                   + "        {\n"
                   + "            \"message\": \"'it'\"\n"
                   + "        }\n"
                   + "    ]\n"
                   + "}";
        var parse = JsonPath.Parse(json);
        var c = JsonPath.Compile("$.logs[?(@.message == \"'it'\")].message");
        var result = parse.Read(c).AsList();
        MyAssert.ContainsExactly(result, "'it'");
    }

    [Fact]
    public void IssuePredicateCanHaveSingleQuotesEscaped()
    {
        var json = "{\n"
                   + "    \"logs\": [\n"
                   + "        {\n"
                   + "            \"message\": \"'it'\"\n"
                   + "        }\n"
                   + "    ]\n"
                   + "}";
        var parse = JsonPath.Parse(json);
        var compile = JsonPath.Compile("$.logs[?(@.message == '\\'it\\'')].message");
        var result = parse.Read(compile).AsList();
        MyAssert.ContainsExactly(result, "'it'");
    }

    [Fact]
    public void IssuePredicateCanHaveSquareBracketInProp()
    {
        var json = "{\n"
                   + "    \"logs\": [\n"
                   + "        {\n"
                   + "            \"message\": \"] it\",\n"
                   + "            \"id\": 2\n"
                   + "        }\n"
                   + "    ]\n"
                   + "}";

        var result = JsonPath.Read(json, "$.logs[?(@.message == '] it')].message").AsList();

        MyAssert.ContainsExactly(result, "] it");
    }

    [Fact]
    public void AFunctionCanBeCompiledd()
    {
        Assert.Equal("$['aaa'].foo()", PathCompiler.Compile("$.aaa.foo()").ToString());
        Assert.Equal("$['aaa'].foo(...)", PathCompiler.Compile("$.aaa.foo(5)").ToString());
        Assert.Equal("$['aaa'].foo(...)", PathCompiler.Compile("$.aaa.foo($.bar)").ToString());
        Assert.Equal("$['aaa'].foo(...)", PathCompiler.Compile("$.aaa.foo(5,10,15)").ToString());
    }

    [Fact]
    public void ArrayIndexesMustBeSeparatedByCommas()
    {
        Assert.Throws<InvalidPathException>(() => PathCompiler.Compile("$[0, 1, 2 4]"));
    }

    [Fact]
    public void TrailingCommaAfterListIsNotAccepted()
    {
        Assert.Throws<InvalidPathException>(() => PathCompiler.Compile("$['1','2',]"));
    }

    [Fact]
    public void AcceptOnlyASingleCommaBetweenIndexes()
    {
        Assert.Throws<InvalidPathException>(() => PathCompiler.Compile("$['1', ,'3']"));
    }

    [Fact]
    public void PropertyMustBeSeparatedByCommas()
    {
        Assert.Throws<InvalidPathException>(() => PathCompiler.Compile("$['aaa'}'bbb']"));
    }
}