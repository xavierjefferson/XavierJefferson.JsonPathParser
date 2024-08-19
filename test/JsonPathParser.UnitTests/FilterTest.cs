using System.Text.RegularExpressions;
using Moq;
using XavierJefferson.JsonPathParser.Interfaces;
using XavierJefferson.JsonPathParser.UnitTests.Extensions;
using XavierJefferson.JsonPathParser.UnitTests.TestData;

namespace XavierJefferson.JsonPathParser.UnitTests;

public class FilterTest : TestUtils
{
    private object GetJson(IProviderTypeTestCase testCase)
    {
        return testCase.Configuration.JsonProvider.Parse(
            "{" +
            "  \"int-key\" : 1, " +
            "  \"long-key\" : 3000000000, " +
            "  \"double-key\" : 10.1, " +
            "  \"bool-key\" : true, " +
            "  \"null-key\" : null, " +
            "  \"string-key\" : \"string\", " +
            "  \"string-key-empty\" : \"\", " +
            "  \"char-key\" : \"c\", " +
            "  \"arr-empty\" : [], " +
            "  \"int-arr\" : [0,1,2,3,4], " +
            "  \"string-arr\" : [\"a\",\"b\",\"c\",\"d\",\"e\"], " +
            "  \"obj\": {\"foo\": \"bar\"}" +
            "}"
        );
    }

    //----------------------------------------------------------------------------
    //
    // EQ
    //
    //----------------------------------------------------------------------------
    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void int_eq_evals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "int-key").Eq(jsonProvider, 1))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "int-key").Eq(jsonProvider, 666))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void int_eq_string_evals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "int-key").Eq(jsonProvider, "1"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "int-key").Eq(jsonProvider, "666"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));


        Assert.True(Filter.Parse("[?(1 == '1')]").Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Parse("[?('1' == 1)]").Apply(CreatePredicateContext(GetJson(testCase), testCase)));

        Assert.False(Filter.Parse("[?(1 === '1')]").Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Parse("[?('1' === 1)]").Apply(CreatePredicateContext(GetJson(testCase), testCase)));

        Assert.True(Filter.Parse("[?(1 === 1)]").Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void long_eq_evals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "long-key").Eq(jsonProvider, 3000000000L))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "long-key").Eq(jsonProvider, 666L))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void double_eq_evals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "double-key").Eq(jsonProvider, 10.1D))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "double-key").Eq(jsonProvider, 10.10D))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "double-key").Eq(jsonProvider, 10.11D))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void string_eq_evals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "string-key").Eq(jsonProvider, "string"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "string-key").Eq(jsonProvider, "666"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void boolean_eq_evals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "bool-key").Eq(jsonProvider, true))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "bool-key").Eq(jsonProvider, false))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void null_eq_evals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "null-key").Eq(jsonProvider, null))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "null-key").Eq(jsonProvider, "666"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "string-key").Eq(jsonProvider, null))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void arr_eq_evals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "arr-empty").Eq(jsonProvider, "[]"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "int-arr").Eq(jsonProvider, "[0,1,2,3,4]"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "int-arr").Eq(jsonProvider, "[0,1,2,3]"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "int-arr").Eq(jsonProvider, "[0,1,2,3,4,5]"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    //----------------------------------------------------------------------------
    //
    // NE
    //
    //----------------------------------------------------------------------------
    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void int_ne_evals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "int-key").Ne(jsonProvider, 1))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "int-key").Ne(jsonProvider, 666))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void long_ne_evals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "long-key").Ne(jsonProvider, 3000000000L))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "long-key").Ne(jsonProvider, 666L))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void double_ne_evals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "double-key").Ne(jsonProvider, 10.1D))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "double-key").Ne(jsonProvider, 10.10D))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "double-key").Ne(jsonProvider, 10.11D))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void string_ne_evals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "string-key").Ne(jsonProvider, "string"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "string-key").Ne(jsonProvider, "666"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void boolean_ne_evals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "bool-key").Ne(jsonProvider, true))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "bool-key").Ne(jsonProvider, false))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void null_ne_evals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "null-key").Ne(jsonProvider, null))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "null-key").Ne(jsonProvider, "666"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "string-key").Ne(jsonProvider, null))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    //----------------------------------------------------------------------------
    //
    // LT
    //
    //----------------------------------------------------------------------------
    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void int_lt_evals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "int-key").Lt(jsonProvider, 10))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "int-key").Lt(jsonProvider, 0))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void long_lt_evals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "long-key").Lt(jsonProvider, 4000000000L))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "long-key").Lt(jsonProvider, 666L))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void double_lt_evals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "double-key").Lt(jsonProvider, 100.1D))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "double-key").Lt(jsonProvider, 1.1D))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void string_lt_evals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "char-key").Lt(jsonProvider, "x"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "char-key").Lt(jsonProvider, "a"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    //----------------------------------------------------------------------------
    //
    // LTE
    //
    //----------------------------------------------------------------------------
    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void int_lte_evals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "int-key").Lte(jsonProvider, 10))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "int-key").Lte(jsonProvider, 1))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "int-key").Lte(jsonProvider, 0))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void long_lte_evals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "long-key").Lte(jsonProvider, 4000000000L))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "long-key").Lte(jsonProvider, 3000000000L))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "long-key").Lte(jsonProvider, 666L))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void double_lte_evals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "double-key").Lte(jsonProvider, 100.1D))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "double-key").Lte(jsonProvider, 10.1D))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "double-key").Lte(jsonProvider, 1.1D))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    //----------------------------------------------------------------------------
    //
    // GT
    //
    //----------------------------------------------------------------------------
    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void int_gt_evals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "int-key").Gt(jsonProvider, 10))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "int-key").Gt(jsonProvider, 0))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void long_gt_evals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "long-key").Gt(jsonProvider, 4000000000L))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "long-key").Gt(jsonProvider, 666L))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void double_gt_evals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "double-key").Gt(jsonProvider, 100.1D))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "double-key").Gt(jsonProvider, 1.1D))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void string_gt_evals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "char-key").Gt(jsonProvider, "x"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "char-key").Gt(jsonProvider, "a"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    //----------------------------------------------------------------------------
    //
    // GTE
    //
    //----------------------------------------------------------------------------
    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void int_gte_evals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "int-key").Gte(jsonProvider, 10))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "int-key").Gte(jsonProvider, 1))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "int-key").Gte(jsonProvider, 0))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void long_gte_evals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "long-key").Gte(jsonProvider, 4000000000L))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "long-key").Gte(jsonProvider, 3000000000L))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "long-key").Gte(jsonProvider, 666L))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void double_gte_evals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "double-key").Gte(jsonProvider, 100.1D))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "double-key").Gte(jsonProvider, 10.1D))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "double-key").Gte(jsonProvider, 1.1D))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    //----------------------------------------------------------------------------
    //
    // Regex
    //
    //----------------------------------------------------------------------------
    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void string_regex_evals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "string-key").Regex(jsonProvider, new Regex("^string$")))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "string-key").Regex(jsonProvider, new Regex("^tring$")))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "null-key").Regex(jsonProvider, new Regex("^string$")))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "int-key").Regex(jsonProvider, new Regex("^string$")))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void list_regex_evals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "string-arr").Regex(jsonProvider, new Regex("^d$")))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "string-arr").Regex(jsonProvider, new Regex("^q$")))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void obj_regex_doesnt_break(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "obj").Regex(jsonProvider, new Regex("^foo$")))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    //----------------------------------------------------------------------------
    //
    // JSON equality
    //
    //----------------------------------------------------------------------------
    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void json_evals(IProviderTypeTestCase testCase)
    {
        var nest = "{\"a\":true}";
        var arr = "[1,2]";
        var json = "{\"foo\":" + arr + ", \"bar\":" + nest + "}";
        var tree = testCase.Configuration.JsonProvider.Parse(json);
        var context = CreatePredicateContext(tree, testCase);
        var farr = Filter.Parse("[?(@.foo == " + arr + ")]");
        //Filter fobjF = Filter.Parse("[?(@.foo == " + nest + ")]");
        //Filter fobjT = Filter.Parse("[?(@.bar == " + nest + ")]");
        var apply = farr.Apply(context);
        Assert.True(apply);
        //Assert.False( fobjF.Apply(context));
        //Assert.True( fobjT.Apply(context));
    }

    //----------------------------------------------------------------------------
    //
    // IN
    //
    //----------------------------------------------------------------------------
    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void string_in_evals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "string-key").In(jsonProvider, "a", null, "string"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "string-key").In(jsonProvider, "a", null))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "null-key").In(jsonProvider, "a", null))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "null-key").In(jsonProvider, "a", "b"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "string-arr").In(jsonProvider, "a"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    //----------------------------------------------------------------------------
    //
    // NIN
    //
    //----------------------------------------------------------------------------
    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void string_nin_evals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "string-key").Nin(jsonProvider, "a", null, "string"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "string-key").Nin(jsonProvider, "a", null))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "null-key").Nin(jsonProvider, "a", null))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "null-key").Nin(jsonProvider, "a", "b"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "string-arr").Nin(jsonProvider, "a"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    //----------------------------------------------------------------------------
    //
    // ALL
    //
    //----------------------------------------------------------------------------
    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void int_all_evals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "int-arr").All(jsonProvider, new List<object?> { 0, 1 }))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "int-arr").All(jsonProvider, new List<object?> { 0, 7 }))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void string_all_evals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "string-arr").All(jsonProvider, new List<object?> { "a", "b" }))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "string-arr").All(jsonProvider, new List<object?> { "a", "x" }))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void not_array_all_evals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "string-key").All(jsonProvider, new List<object?> { "a", "b" }))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    //----------------------------------------------------------------------------
    //
    // SIZE
    //
    //----------------------------------------------------------------------------
    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void array_size_evals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "string-arr").Size(jsonProvider, 5))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "string-arr").Size(jsonProvider, 7))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void string_size_evals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "string-key").Size(jsonProvider, 6))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "string-key").Size(jsonProvider, 7))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void other_size_evals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "int-key").Size(jsonProvider, 6))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void null_size_evals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "null-key").Size(jsonProvider, 6))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    //----------------------------------------------------------------------------
    //
    // SUBSETOF
    //
    //----------------------------------------------------------------------------
    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void array_subsetof_evals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        // list is a superset
        var list = new ObjectList("a", "b", "c", "d", "e", "f", "g");
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "string-arr").SubsetOf((IJsonProvider)jsonProvider, list))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        // list is exactly the same set (but in a different order)
        list = new ObjectList("e", "d", "b", "c", "a");
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "string-arr").SubsetOf((IJsonProvider)jsonProvider, list))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        // list is missing one element
        list = new ObjectList("a", "b", "c", "d");
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "string-arr").SubsetOf((IJsonProvider)jsonProvider, list))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    //----------------------------------------------------------------------------
    //
    // ANYOF
    //
    //----------------------------------------------------------------------------
    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void array_anyof_evals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var list = new ObjectList("a", "z");
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "string-arr").AnyOf((IJsonProvider)jsonProvider, list))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        list = new ObjectList("z", "b", "a");
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "string-arr").AnyOf((IJsonProvider)jsonProvider, list))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        list = new ObjectList("x", "y", "z");
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "string-arr").AnyOf((IJsonProvider)jsonProvider, list))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    //----------------------------------------------------------------------------
    //
    // NONEOF
    //
    //----------------------------------------------------------------------------
    [Theory] [ClassData(typeof(ProviderTypeTestCases))]
    public void array_noneof_evals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var list = new ObjectList("a", "z");
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "string-arr").NoneOf((IJsonProvider)jsonProvider, list))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        list = new ObjectList("z", "b", "a");
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "string-arr").NoneOf((IJsonProvider)jsonProvider, list))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        list = new ObjectList("x", "y", "z");
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "string-arr").NoneOf((IJsonProvider)jsonProvider, list))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    //----------------------------------------------------------------------------
    //
    // EXISTS
    //
    //----------------------------------------------------------------------------
    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void exists_evals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "string-key").Exists(jsonProvider, true))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "string-key").Exists(jsonProvider, false))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));

        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "missing-key").Exists(jsonProvider, true))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "missing-key").Exists(jsonProvider, false))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    //----------------------------------------------------------------------------
    //
    // TYPE
    //
    //----------------------------------------------------------------------------

    [InlineData(true, "string-key", typeof(string))]
    [InlineData(false, "string-key", typeof(double))]
    [InlineData(false, "int-key", typeof(string))]
    [InlineData(true, "int-key", typeof(double))]
    [InlineData(false, "null-key", typeof(string))]
    [InlineData(true, "int-arr", typeof(List<object?>))]
    [Theory]
    public void type_evals(bool expectedValue, string where, Type type)
    {
        var testCase = ProviderTypeTestCases.Cases.First().Value; 
        var jsonProvider = testCase.Configuration.JsonProvider;
        var tmp = Filter.Create(Criteria.Where(jsonProvider, where).Type(type))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase));
        Assert.Equal(expectedValue, tmp);
    }

    //----------------------------------------------------------------------------
    //
    // NOT EMPTY
    //
    //----------------------------------------------------------------------------
    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void not_empty_evals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "string-key").Empty(false))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "string-key-empty").Empty(false))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));

        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "int-arr").Empty(false))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "arr-empty").Empty(false))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));

        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "null-key").Empty(false))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    //----------------------------------------------------------------------------
    //
    // EMPTY
    //
    //----------------------------------------------------------------------------
    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void empty_evals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "string-key").Empty(false))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "string-key").Empty(true))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));

        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "string-key-empty").Empty(true))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "string-key-empty").Empty(false))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));

        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "int-arr").Empty(false))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "int-arr").Empty(true))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));

        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "arr-empty").Empty(true))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "arr-empty").Empty(false))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));

        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "null-key").Empty(true))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "null-key").Empty(false))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }


    //----------------------------------------------------------------------------
    //
    // MATCHES
    //
    //----------------------------------------------------------------------------

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void matches_evals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var mockPredicate = new Mock<IPredicate>();
        mockPredicate.Setup(i => i.Apply(It.IsAny<IPredicateContext>())).Returns(
            (IPredicateContext context) =>
            {
                var t = context.Item as IDictionary<string, object?>;
                var i = Convert.ToInt32(t["int-key"]);

                return i == 1;
            });
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "string-key").Eq(jsonProvider, "string")
                .And(jsonProvider, "$").Matches(mockPredicate.Object))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    //----------------------------------------------------------------------------
    //
    // OR
    //
    //----------------------------------------------------------------------------
    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void or_and_filters_evaluates(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var model = new Dictionary<string, object?> { { "foo", true }, { "bar", false } };


        var isFoo = Filter.Create(Criteria.Where(jsonProvider, "foo").Is(jsonProvider, true));
        var isBar = Filter.Create(Criteria.Where(jsonProvider, "bar").Is(jsonProvider, true));


        var fooOrBar = Filter.Create(Criteria.Where(jsonProvider, "foo").Is(jsonProvider, true)).Or(Criteria.Where(jsonProvider, "bar").Is(jsonProvider, true));
        var fooAndBar = Filter.Create(Criteria.Where(jsonProvider, "foo").Is(jsonProvider, true)).And(Criteria.Where(jsonProvider, "bar").Is(jsonProvider, true));

        Assert.True(isFoo.Or(isBar).Apply(CreatePredicateContext(model, testCase)));
        Assert.False(isFoo.And(isBar).Apply(CreatePredicateContext(model, testCase)));
        Assert.True(fooOrBar.Apply(CreatePredicateContext(model, testCase)));
        Assert.False(fooAndBar.Apply(CreatePredicateContext(model, testCase)));
    }


    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void TestFilterWithOrShortCircuit1(IProviderTypeTestCase testCase)
    {
        var json = testCase.Configuration.JsonProvider
            .Parse("{\"firstname\":\"Bob\",\"surname\":\"Smith\",\"age\":30}");
        Assert.False(Filter.Parse("[?((@.firstname == 'Bob' || @.firstname == 'Jane') && @.surname == 'Doe')]")
            .Apply(CreatePredicateContext(json, testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void TestFilterWithOrShortCircuit2(IProviderTypeTestCase testCase)
    {
        var json = testCase.Configuration.JsonProvider
            .Parse("{\"firstname\":\"Bob\",\"surname\":\"Smith\",\"age\":30}");
        Assert.True(Filter.Parse("[?((@.firstname == 'Bob' || @.firstname == 'Jane') && @.surname == 'Smith')]")
            .Apply(CreatePredicateContext(json, testCase)));
    }

    [Fact]
    public void criteria_can_be_parsed()
    {
        var criteria = Filter.Parse("[?(@.foo == 'baar')]");
        Assert.Equal("[?(@['foo'] == 'baar')]", criteria.ToString());

        criteria = Filter.Parse("[?(@.foo)]");
        Assert.Equal("[?(@['foo'])]", criteria.ToString());
    }


    [Fact]
    public void inline_in_criteria_evaluates()
    {
        var list = JsonPath.Read(JsonTestData.JsonDocument, "$.store.book[?(@.category in ['reference', 'fiction'])]")
            .AsList();
        Assert.Equal(4, list.Count);
    }
}