namespace XavierJefferson.JsonPathParser.UnitTests.Function;

/**
 * TDD for Issue #234
 * 
 * Verifies the use-case where-in the function path expression is cached and re-used but the JsonPath includes a function
 * whose arguments are then dependent upon state that changes externally from the internal Cache.getCache state.  The
 * prior implementation had a bug where-in the parameter values were cached -- the present implementation (as of Issue #234)
 * now uses a late binding approach to eval the function parameters.  Cache invalidation isn't an option given the need
 * for nested function calls.
 * 
 * Once this bug is fixed, most of the concern then centers around the need to ensure nested functions are processed
 * correctly.
 * 
 * @see NestedFunctionTest for examples of where that is performed.
 */
public class Issue234
{
    [Fact]
    public void TestIssue234()
    {
        var context = new JpDictionary();
        context["key"] = "first";
        const string pathAsString = "concat(\"/\", $.key)";
        var value = JsonPath.Read(context, pathAsString);
        Assert.Equal("/first", value);
        context["key"] = "second";
        value = JsonPath.Read(context, pathAsString);
        Assert.Equal("/second", value);
    }
}