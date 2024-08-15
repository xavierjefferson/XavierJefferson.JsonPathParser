using Moq;
using XavierJefferson.JsonPathParser.Exceptions;
using XavierJefferson.JsonPathParser.Interfaces;
using XavierJefferson.JsonPathParser.Path;
using XavierJefferson.JsonPathParser.UnitTests.Extensions;

namespace XavierJefferson.JsonPathParser.UnitTests;

public class PathCompilerTest : TestUtils
{
    [Fact(Skip = "Backward compatibility <= 2.0.0")]
    public void a_path_must_start_with_dollarsign_or_at()
    {
        Assert.Throws<InvalidPathException>(() => PathCompiler.Compile("x"));
    }

    [Fact(Skip = "Backward compatibility <= 2.0.0")]
    public void a_square_bracket_may_not_follow_a_period()
    {
        Assert.Throws<InvalidPathException>(() => PathCompiler.Compile("$.["));
    }

    [Fact]
    public void a_root_path_must_be_followed_by_period_or_bracket()
    {
        Assert.Throws<InvalidPathException>(() => PathCompiler.Compile("$X"));
    }

    [Fact]
    public void a_root_path_can_be_compiled()
    {
        Assert.Equal("$", PathCompiler.Compile("$").ToString());
        Assert.Equal("@", PathCompiler.Compile("@").ToString());
    }

    [Fact]
    public void a_path_may_not_end_with_period()
    {
        Assert.Throws<InvalidPathException>(() => PathCompiler.Compile("$."));
    }

    [Fact]
    public void a_path_may_not_end_with_period_2()
    {
        Assert.Throws<InvalidPathException>(() => PathCompiler.Compile("$.prop."));
    }

    [Fact]
    public void a_path_may_not_end_with_scan()
    {
        Assert.Throws<InvalidPathException>(() => PathCompiler.Compile("$.."));
    }

    [Fact]
    public void a_path_may_not_end_with_scan_2()
    {
        Assert.Throws<InvalidPathException>(() => PathCompiler.Compile("$.prop.."));
    }

    [Fact]
    public void a_property_token_can_be_compiled()
    {
        Assert.Equal("$['prop']", PathCompiler.Compile("$.prop").ToString());
        Assert.Equal("$['1prop']", PathCompiler.Compile("$.1prop").ToString());
        Assert.Equal("$['@prop']", PathCompiler.Compile("$.@prop").ToString());
    }

    [Fact]
    public void a_bracket_notation_property_token_can_be_compiledd()
    {
        Assert.Equal("$['prop']", PathCompiler.Compile("$['prop']").ToString());
        Assert.Equal("$['1prop']", PathCompiler.Compile("$['1prop']").ToString());
        Assert.Equal("$['@prop']", PathCompiler.Compile("$['@prop']").ToString());
        Assert.Equal("$['@prop']", PathCompiler.Compile("$[  '@prop'  ]").ToString());
        Assert.Equal("$[\"prop\"]", PathCompiler.Compile("$[\"prop\"]").ToString());
    }

    [Fact]
    public void a_multi_property_token_can_be_compiledd()
    {
        Assert.Equal("$['prop0','prop1']", PathCompiler.Compile("$['prop0', 'prop1']").ToString());
        Assert.Equal("$['prop0','prop1']", PathCompiler.Compile("$[  'prop0'  , 'prop1'  ]").ToString());
    }

    [Fact]
    public void a_property_chain_can_be_compiledd()
    {
        Assert.Equal("$['abc']", PathCompiler.Compile("$.abc").ToString());
        Assert.Equal("$['aaa']['bbb']", PathCompiler.Compile("$.aaa.bbb").ToString());
        Assert.Equal("$['aaa']['bbb']['ccc']", PathCompiler.Compile("$.aaa.bbb.ccc").ToString());
    }

    [Fact]
    public void a_property_may_not_contain_blanks()
    {
        Assert.Throws<InvalidPathException>(() => PathCompiler.Compile("$.foo bar"));
    }

    [Fact]
    public void a_wildcard_can_be_compiledd()
    {
        Assert.Equal("$[*]", PathCompiler.Compile("$.*").ToString());
        Assert.Equal("$[*]", PathCompiler.Compile("$[*]").ToString());
        Assert.Equal("$[*]", PathCompiler.Compile("$[ * ]").ToString());
    }

    [Fact]
    public void a_wildcard_can_follow_a_property()
    {
        Assert.Equal("$['prop'][*]", PathCompiler.Compile("$.prop[*]").ToString());
        Assert.Equal("$['prop'][*]", PathCompiler.Compile("$['prop'][*]").ToString());
    }

    [Fact]
    public void an_array_index_path_can_be_compiledd()
    {
        Assert.Equal("$[1]", PathCompiler.Compile("$[1]").ToString());
        Assert.Equal("$[1,2,3]", PathCompiler.Compile("$[1,2,3]").ToString());
        Assert.Equal("$[1,2,3]", PathCompiler.Compile("$[ 1 , 2 , 3 ]").ToString());
    }

    [Fact]
    public void an_array_slice_path_can_be_compiledd()
    {
        Assert.Equal("$[-1:]", PathCompiler.Compile("$[-1:]").ToString());
        Assert.Equal("$[1:2]", PathCompiler.Compile("$[1:2]").ToString());
        Assert.Equal("$[:2]", PathCompiler.Compile("$[:2]").ToString());
    }

    [Fact]
    public void an_inline_criteria_can_be_parsed()
    {
        Assert.Equal("$[?]", PathCompiler.Compile("$[?(@.foo == 'bar')]").ToString());
        Assert.Equal("$[?]", PathCompiler.Compile("$[?(@.foo == \"bar\")]").ToString());
    }

    [Fact]
    public void a_placeholder_criteria_can_be_parsed()
    {
        var p = new Mock<IPredicate>();
        p.Setup(i => i.Apply(It.IsAny<IPredicateContext>())).Returns((IPredicateContext context) => { return false; });
        Assert.Equal("$[?]", PathCompiler.Compile("$[?]", p.Object).ToString());
        Assert.Equal("$[?,?]", PathCompiler.Compile("$[?,?]", p.Object, p.Object).ToString());
        Assert.Equal("$[?,?,?]", PathCompiler.Compile("$[?,?,?]", p.Object, p.Object, p.Object).ToString());
    }

    [Fact]
    public void a_scan_token_can_be_parsed()
    {
        Assert.Equal("$..['prop']..[*]", PathCompiler.Compile("$..['prop']..[*]").ToString());
    }

    [Fact]
    public void issue_predicate_can_have_escaped_backslash_in_prop()
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
    public void issue_predicate_can_have_bracket_in_regex()
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
    public void issue_predicate_can_have_and_in_regex()
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
    public void issue_predicate_can_have_and_in_prop()
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
    public void issue_predicate_brackets_must_change_priorities()
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
    public void issue_predicate_or_has_lower_priority_than_and()
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
    public void issue_predicate_can_have_double_quotes()
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
    public void issue_predicate_can_have_single_quotes()
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
    public void issue_predicate_can_have_single_quotes_escaped()
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
    public void issue_predicate_can_have_square_bracket_in_prop()
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
    public void a_function_can_be_compiledd()
    {
        Assert.Equal("$['aaa'].foo()", PathCompiler.Compile("$.aaa.foo()").ToString());
        Assert.Equal("$['aaa'].foo(...)", PathCompiler.Compile("$.aaa.foo(5)").ToString());
        Assert.Equal("$['aaa'].foo(...)", PathCompiler.Compile("$.aaa.foo($.bar)").ToString());
        Assert.Equal("$['aaa'].foo(...)", PathCompiler.Compile("$.aaa.foo(5,10,15)").ToString());
    }

    [Fact]
    public void array_indexes_must_be_separated_by_commas()
    {
        Assert.Throws<InvalidPathException>(() => PathCompiler.Compile("$[0, 1, 2 4]"));
    }

    [Fact]
    public void trailing_comma_after_list_is_not_accepted()
    {
        Assert.Throws<InvalidPathException>(() => PathCompiler.Compile("$['1','2',]"));
    }

    [Fact]
    public void accept_only_a_single_comma_between_indexes()
    {
        Assert.Throws<InvalidPathException>(() => PathCompiler.Compile("$['1', ,'3']"));
    }

    [Fact]
    public void property_must_be_separated_by_commas()
    {
        Assert.Throws<InvalidPathException>(() => PathCompiler.Compile("$['aaa'}'bbb']"));
    }
}