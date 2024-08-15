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
        Assert.True(Filter.Create(Criteria.Where("int-key").Eq(1))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where("int-key").Eq(666))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void int_eq_string_evals(IProviderTypeTestCase testCase)
    {
        Assert.True(Filter.Create(Criteria.Where("int-key").Eq("1"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where("int-key").Eq("666"))
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
        Assert.True(Filter.Create(Criteria.Where("long-key").Eq(3000000000L))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where("long-key").Eq(666L))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void double_eq_evals(IProviderTypeTestCase testCase)
    {
        Assert.True(Filter.Create(Criteria.Where("double-key").Eq(10.1D))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where("double-key").Eq(10.10D))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where("double-key").Eq(10.11D))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void string_eq_evals(IProviderTypeTestCase testCase)
    {
        Assert.True(Filter.Create(Criteria.Where("string-key").Eq("string"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where("string-key").Eq("666"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void boolean_eq_evals(IProviderTypeTestCase testCase)
    {
        Assert.True(Filter.Create(Criteria.Where("bool-key").Eq(true))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where("bool-key").Eq(false))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void null_eq_evals(IProviderTypeTestCase testCase)
    {
        Assert.True(Filter.Create(Criteria.Where("null-key").Eq(null))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where("null-key").Eq("666"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where("string-key").Eq(null))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void arr_eq_evals(IProviderTypeTestCase testCase)
    {
        Assert.True(Filter.Create(Criteria.Where("arr-empty").Eq("[]"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where("int-arr").Eq("[0,1,2,3,4]"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where("int-arr").Eq("[0,1,2,3]"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where("int-arr").Eq("[0,1,2,3,4,5]"))
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
        Assert.False(Filter.Create(Criteria.Where("int-key").Ne(1))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where("int-key").Ne(666))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void long_ne_evals(IProviderTypeTestCase testCase)
    {
        Assert.False(Filter.Create(Criteria.Where("long-key").Ne(3000000000L))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where("long-key").Ne(666L))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void double_ne_evals(IProviderTypeTestCase testCase)
    {
        Assert.False(Filter.Create(Criteria.Where("double-key").Ne(10.1D))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where("double-key").Ne(10.10D))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where("double-key").Ne(10.11D))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void string_ne_evals(IProviderTypeTestCase testCase)
    {
        Assert.False(Filter.Create(Criteria.Where("string-key").Ne("string"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where("string-key").Ne("666"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void boolean_ne_evals(IProviderTypeTestCase testCase)
    {
        Assert.False(Filter.Create(Criteria.Where("bool-key").Ne(true))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where("bool-key").Ne(false))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void null_ne_evals(IProviderTypeTestCase testCase)
    {
        Assert.False(Filter.Create(Criteria.Where("null-key").Ne(null))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where("null-key").Ne("666"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where("string-key").Ne(null))
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
        Assert.True(Filter.Create(Criteria.Where("int-key").Lt(10))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where("int-key").Lt(0))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void long_lt_evals(IProviderTypeTestCase testCase)
    {
        Assert.True(Filter.Create(Criteria.Where("long-key").Lt(4000000000L))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where("long-key").Lt(666L))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void double_lt_evals(IProviderTypeTestCase testCase)
    {
        Assert.True(Filter.Create(Criteria.Where("double-key").Lt(100.1D))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where("double-key").Lt(1.1D))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void string_lt_evals(IProviderTypeTestCase testCase)
    {
        Assert.True(Filter.Create(Criteria.Where("char-key").Lt("x"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where("char-key").Lt("a"))
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
        Assert.True(Filter.Create(Criteria.Where("int-key").Lte(10))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where("int-key").Lte(1))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where("int-key").Lte(0))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void long_lte_evals(IProviderTypeTestCase testCase)
    {
        Assert.True(Filter.Create(Criteria.Where("long-key").Lte(4000000000L))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where("long-key").Lte(3000000000L))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where("long-key").Lte(666L))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void double_lte_evals(IProviderTypeTestCase testCase)
    {
        Assert.True(Filter.Create(Criteria.Where("double-key").Lte(100.1D))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where("double-key").Lte(10.1D))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where("double-key").Lte(1.1D))
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
        Assert.False(Filter.Create(Criteria.Where("int-key").Gt(10))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where("int-key").Gt(0))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void long_gt_evals(IProviderTypeTestCase testCase)
    {
        Assert.False(Filter.Create(Criteria.Where("long-key").Gt(4000000000L))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where("long-key").Gt(666L))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void double_gt_evals(IProviderTypeTestCase testCase)
    {
        Assert.False(Filter.Create(Criteria.Where("double-key").Gt(100.1D))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where("double-key").Gt(1.1D))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void string_gt_evals(IProviderTypeTestCase testCase)
    {
        Assert.False(Filter.Create(Criteria.Where("char-key").Gt("x"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where("char-key").Gt("a"))
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
        Assert.False(Filter.Create(Criteria.Where("int-key").Gte(10))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where("int-key").Gte(1))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where("int-key").Gte(0))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void long_gte_evals(IProviderTypeTestCase testCase)
    {
        Assert.False(Filter.Create(Criteria.Where("long-key").Gte(4000000000L))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where("long-key").Gte(3000000000L))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where("long-key").Gte(666L))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void double_gte_evals(IProviderTypeTestCase testCase)
    {
        Assert.False(Filter.Create(Criteria.Where("double-key").Gte(100.1D))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where("double-key").Gte(10.1D))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where("double-key").Gte(1.1D))
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
        Assert.True(Filter.Create(Criteria.Where("string-key").Regex(new Regex("^string$")))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where("string-key").Regex(new Regex("^tring$")))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where("null-key").Regex(new Regex("^string$")))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where("int-key").Regex(new Regex("^string$")))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void list_regex_evals(IProviderTypeTestCase testCase)
    {
        Assert.True(Filter.Create(Criteria.Where("string-arr").Regex(new Regex("^d$")))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where("string-arr").Regex(new Regex("^q$")))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void obj_regex_doesnt_break(IProviderTypeTestCase testCase)
    {
        Assert.False(Filter.Create(Criteria.Where("obj").Regex(new Regex("^foo$")))
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
        Assert.True(Filter.Create(Criteria.Where("string-key").In("a", null, "string"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where("string-key").In("a", null))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where("null-key").In("a", null))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where("null-key").In("a", "b"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where("string-arr").In("a"))
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
        Assert.False(Filter.Create(Criteria.Where("string-key").Nin("a", null, "string"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where("string-key").Nin("a", null))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where("null-key").Nin("a", null))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where("null-key").Nin("a", "b"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where("string-arr").Nin("a"))
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
        Assert.True(Filter.Create(Criteria.Where("int-arr").All(new List<object?> { 0, 1 }))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where("int-arr").All(new List<object?> { 0, 7 }))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void string_all_evals(IProviderTypeTestCase testCase)
    {
        Assert.True(Filter.Create(Criteria.Where("string-arr").All(new List<object?> { "a", "b" }))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where("string-arr").All(new List<object?> { "a", "x" }))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void not_array_all_evals(IProviderTypeTestCase testCase)
    {
        Assert.False(Filter.Create(Criteria.Where("string-key").All(new List<object?> { "a", "b" }))
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
        Assert.True(Filter.Create(Criteria.Where("string-arr").Size(5))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where("string-arr").Size(7))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void string_size_evals(IProviderTypeTestCase testCase)
    {
        Assert.True(Filter.Create(Criteria.Where("string-key").Size(6))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where("string-key").Size(7))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void other_size_evals(IProviderTypeTestCase testCase)
    {
        Assert.False(Filter.Create(Criteria.Where("int-key").Size(6))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void null_size_evals(IProviderTypeTestCase testCase)
    {
        Assert.False(Filter.Create(Criteria.Where("null-key").Size(6))
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
        // list is a superset
        var list = new ObjectList("a", "b", "c", "d", "e", "f", "g");
        Assert.True(Filter.Create(Criteria.Where("string-arr").SubsetOf(list))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        // list is exactly the same set (but in a different order)
        list = new ObjectList("e", "d", "b", "c", "a");
        Assert.True(Filter.Create(Criteria.Where("string-arr").SubsetOf(list))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        // list is missing one element
        list = new ObjectList("a", "b", "c", "d");
        Assert.False(Filter.Create(Criteria.Where("string-arr").SubsetOf(list))
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
        var list = new ObjectList("a", "z");
        Assert.True(Filter.Create(Criteria.Where("string-arr").AnyOf(list))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        list = new ObjectList("z", "b", "a");
        Assert.True(Filter.Create(Criteria.Where("string-arr").AnyOf(list))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        list = new ObjectList("x", "y", "z");
        Assert.False(Filter.Create(Criteria.Where("string-arr").AnyOf(list))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    //----------------------------------------------------------------------------
    //
    // NONEOF
    //
    //----------------------------------------------------------------------------
    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void array_noneof_evals(IProviderTypeTestCase testCase)
    {
        var list = new ObjectList("a", "z");
        Assert.False(Filter.Create(Criteria.Where("string-arr").NoneOf(list))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        list = new ObjectList("z", "b", "a");
        Assert.False(Filter.Create(Criteria.Where("string-arr").NoneOf(list))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        list = new ObjectList("x", "y", "z");
        Assert.True(Filter.Create(Criteria.Where("string-arr").NoneOf(list))
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
        Assert.True(Filter.Create(Criteria.Where("string-key").Exists(true))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where("string-key").Exists(false))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));

        Assert.False(Filter.Create(Criteria.Where("missing-key").Exists(true))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where("missing-key").Exists(false))
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
        var tmp = Filter.Create(Criteria.Where(where).Type(type))
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
        Assert.True(Filter.Create(Criteria.Where("string-key").Empty(false))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where("string-key-empty").Empty(false))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));

        Assert.True(Filter.Create(Criteria.Where("int-arr").Empty(false))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where("arr-empty").Empty(false))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));

        Assert.False(Filter.Create(Criteria.Where("null-key").Empty(false))
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
        Assert.True(Filter.Create(Criteria.Where("string-key").Empty(false))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where("string-key").Empty(true))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));

        Assert.True(Filter.Create(Criteria.Where("string-key-empty").Empty(true))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where("string-key-empty").Empty(false))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));

        Assert.True(Filter.Create(Criteria.Where("int-arr").Empty(false))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where("int-arr").Empty(true))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));

        Assert.True(Filter.Create(Criteria.Where("arr-empty").Empty(true))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where("arr-empty").Empty(false))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));

        Assert.False(Filter.Create(Criteria.Where("null-key").Empty(true))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where("null-key").Empty(false))
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
        var mockPredicate = new Mock<IPredicate>();
        mockPredicate.Setup(i => i.Apply(It.IsAny<IPredicateContext>())).Returns(
            (IPredicateContext context) =>
            {
                var t = context.Item as IDictionary<string, object?>;
                var i = Convert.ToInt32(t["int-key"]);

                return i == 1;
            });
        Assert.True(Filter.Create(Criteria.Where("string-key").Eq("string")
                .And("$").Matches(mockPredicate.Object))
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
        var model = new Dictionary<string, object?> { { "foo", true }, { "bar", false } };


        var isFoo = Filter.Create(Criteria.Where("foo").Is(true));
        var isBar = Filter.Create(Criteria.Where("bar").Is(true));


        var fooOrBar = Filter.Create(Criteria.Where("foo").Is(true)).Or(Criteria.Where("bar").Is(true));
        var fooAndBar = Filter.Create(Criteria.Where("foo").Is(true)).And(Criteria.Where("bar").Is(true));

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