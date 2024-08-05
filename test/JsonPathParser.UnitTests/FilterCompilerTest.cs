using XavierJefferson.JsonPathParser.Exceptions;
using XavierJefferson.JsonPathParser.Filtering;

namespace XavierJefferson.JsonPathParser.UnitTests;

public class FilterCompilerTest
{
    [Theory]
    [InlineData("[?(@)]", "[?(@)]")]
    [InlineData("[?(@['firstname'])]", "[?(@.firstname)]")]
    [InlineData("[?($['firstname'])]", "[?($.firstname)]")]
    [InlineData("[?(@['firstname'])]", "[?(@['firstname'])]")]
    [InlineData("[?($['firstname']['lastname'])]", "[?($['firstname'].lastname)]")]
    [InlineData("[?($['firstname']['lastname'])]", "[?($['firstname']['lastname'])]")]
    [InlineData("[?($['firstname']['lastname'][*])]", "[?($['firstname']['lastname'].*)]")]
    [InlineData("[?($['firstname']['num_eq'] == 1)]", "[?($['firstname']['num_eq'] == 1)]")]
    [InlineData("[?($['firstname']['num_gt'] > 1.1)]", "[?($['firstname']['num_gt'] > 1.1)]")]
    [InlineData("[?($['firstname']['num_lt'] < 11.11)]", "[?($['firstname']['num_lt'] < 11.11)]")]
    [InlineData("[?($['firstname']['str_eq'] == 'hej')]", "[?($['firstname']['str_eq'] == 'hej')]")]
    [InlineData("[?($['firstname']['str_eq'] == '')]", "[?($['firstname']['str_eq'] == '')]")]
    [InlineData("[?($['firstname']['str_eq'] == null)]", "[?($['firstname']['str_eq'] == null)]")]
    [InlineData("[?($['firstname']['str_eq'] == true)]", "[?($['firstname']['str_eq'] == true)]")]
    [InlineData("[?($['firstname']['str_eq'] == false)]", "[?($['firstname']['str_eq'] == false)]")]
    [InlineData("[?(@['firstname'] && @['lastname'])]", "[?(@.firstname && @.lastname)]")]
    [InlineData("[?((@['firstname'] || @['lastname']) && @['and'])]", "[?((@.firstname || @.lastname) && @.and)]")]
    [InlineData("[?((@['a'] || @['b'] || @['c']) && @['x'])]", "[?((@.a || @.b || @.c) && @.x)]")]
    [InlineData("[?((@['a'] && @['b'] && @['c']) || @['x'])]", "[?((@.a && @.b && @.c) || @.x)]")]
    [InlineData("[?(((@['a'] && @['b']) || @['c']) || @['x'])]", "[?((@.a && @.b || @.c) || @.x)]")]
    [InlineData("[?((@['a'] && @['b']) || (@['c'] && @['d']))]", "[?((@.a && @.b) || (@.c && @.d))]")]
    [InlineData("[?(@['a'] IN [1,2,3])]", "[?(@.a IN [1,2,3])]")]
    [InlineData("[?(@['a'] IN {'foo':'bar'})]", "[?(@.a IN {'foo':'bar'})]")]
    [InlineData("[?(@['value'] < '7')]", "[?(@.value<'7')]")]
    [InlineData("[?(@['message'] == 'it\\\\')]", "[?(@.message == 'it\\\\')]")]
    [InlineData("[?(@['message'].min() > 10)]", "[?(@.message.min() > 10)]")]
    [InlineData("[?(@['message'].min() == 10)]", "[?(@.message.min()==10)]")]
    [InlineData("[?(10 == @['message'].min())]", "[?(10 == @.message.min())]")]
    [InlineData("[?(@)]", "[?(((@)))]")]
    [InlineData("[?(@['name'] =~ /.*?/i)]", "[?(@.name =~ /.*?/i)]")]
    [InlineData("[?(@['name'] =~ /.*?/)]", "[?(@.name =~ /.*?/)]")]
    [InlineData("[?($[\"firstname\"][\"lastname\"])]", "[?($[\"firstname\"][\"lastname\"])]")]
    [InlineData("[?($[\"firstname\"]['lastname'])]", "[?($[\"firstname\"].lastname)]")]
    [InlineData("[?($[\"firstname\",\"lastname\"])]", "[?($[\"firstname\", \"lastname\"])]")]
    [InlineData("[?(((@['a'] && @['b']) || @['c']) || @['x'])]", "[?(((@.a && @.b || @.c)) || @.x)]")]
    public void valid_filters_compile(string expected, string expression)
    {
        var fc = FilterCompiler.Compile(expression);
        var z0 = fc.ToString();
        Assert.Equal(expected, z0);
    }

    [Fact]
    public void string_quote_style_is_serialized()
    {
        Assert.Equal("[?('apa' == 'apa')]", FilterCompiler.Compile("[?('apa' == 'apa')]").ToString());
        Assert.Equal("[?('apa' == \"apa\")]", FilterCompiler.Compile("[?('apa' == \"apa\")]").ToString());
    }

    [Fact]
    public void string_can_contain_path_chars()
    {
        Assert.Equal("[?(@[')]@$)]'] == ')]@$)]')]", FilterCompiler.Compile("[?(@[')]@$)]'] == ')]@$)]')]").ToString());
        Assert.Equal("[?(@[\")]@$)]\"] == \")]@$)]\")]",
            FilterCompiler.Compile("[?(@[\")]@$)]\"] == \")]@$)]\")]").ToString());
    }

    [Fact]
    public void invalid_path_when_string_literal_is_unquoted()
    {
        Assert.Throws<InvalidPathException>(() => { FilterCompiler.Compile("[?(@.foo == x)]"); });
    }

    [Fact]
    public void or_has_lower_priority_than_and()
    {
        Assert.Equal("[?((@['category'] == 'fiction' && @['author'] == 'Evelyn Waugh') || @['price'] > 15)]",
            FilterCompiler.Compile("[?(@.category == 'fiction' && @.author == 'Evelyn Waugh' || @.price > 15)]")
                .ToString());
    }

    [Theory]
    [InlineData("[?(@))]")]
    [InlineData("[?(@ FOO 1)]")]
    [InlineData("[?(@ || )]")]
    [InlineData("[?(@ == 'foo )]")]
    [InlineData("[?(@ == 1' )]")]
    [InlineData("[?(@.foo bar == 1)]")]
    [InlineData("[?(@.i == 5 @.i == 8)]")]
    [InlineData("[?(!5)]")]
    [InlineData("[?(!'foo')]")]
    public void invalid_filters_does_not_compile(string input)
    {
        AssertInvalidPathException(input);
    }

    [Fact]
    // issue #178
    public void compile_and_serialize_not_exists_filter()
    {
        var compiled = FilterCompiler.Compile("[?(!@.foo)]");
        var serialized = compiled.ToString();
        Assert.Equal("[?(!@['foo'])]", serialized);
    }


    private void AssertInvalidPathException(string filter)
    {
        Assert.Throws<InvalidPathException>(() => { FilterCompiler.Compile(filter); });
    }
}