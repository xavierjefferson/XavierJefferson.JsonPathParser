using System.Text.RegularExpressions;
using XavierJefferson.JsonPathParser.Filtering;
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
    public void IntEqEvals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "int-key").Eq(1))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "int-key").Eq(666))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void IntEqStringEvals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "int-key").Eq("1"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "int-key").Eq("666"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));


        Assert.True(Filter.Parse("[?(1 == '1')]").Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Parse("[?('1' == 1)]").Apply(CreatePredicateContext(GetJson(testCase), testCase)));

        Assert.False(Filter.Parse("[?(1 === '1')]").Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Parse("[?('1' === 1)]").Apply(CreatePredicateContext(GetJson(testCase), testCase)));

        Assert.True(Filter.Parse("[?(1 === 1)]").Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void LongEqEvals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "long-key").Eq(3000000000L))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "long-key").Eq(666L))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void DoubleEqEvals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "double-key").Eq(10.1D))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "double-key").Eq(10.10D))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "double-key").Eq(10.11D))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void StringEqEvals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "string-key").Eq("string"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "string-key").Eq("666"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void BooleanEqEvals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "bool-key").Eq(true))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "bool-key").Eq(false))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void NullEqEvals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "null-key").Eq(null))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "null-key").Eq("666"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "string-key").Eq(null))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void ArrEqEvals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "arr-empty").Eq("[]"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "int-arr").Eq("[0,1,2,3,4]"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "int-arr").Eq("[0,1,2,3]"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "int-arr").Eq("[0,1,2,3,4,5]"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    //----------------------------------------------------------------------------
    //
    // NE
    //
    //----------------------------------------------------------------------------
    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void IntNeEvals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "int-key").Ne(1))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "int-key").Ne(666))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void LongNeEvals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "long-key").Ne(3000000000L))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "long-key").Ne(666L))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void DoubleNeEvals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "double-key").Ne(10.1D))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "double-key").Ne(10.10D))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "double-key").Ne(10.11D))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void StringNeEvals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "string-key").Ne("string"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "string-key").Ne("666"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void BooleanNeEvals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "bool-key").Ne(true))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "bool-key").Ne(false))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void NullNeEvals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "null-key").Ne(null))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "null-key").Ne("666"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "string-key").Ne(null))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    //----------------------------------------------------------------------------
    //
    // LT
    //
    //----------------------------------------------------------------------------
    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void IntLtEvals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "int-key").Lt(10))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "int-key").Lt(0))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void LongLtEvals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "long-key").Lt(4000000000L))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "long-key").Lt(666L))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void DoubleLtEvals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "double-key").Lt(100.1D))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "double-key").Lt(1.1D))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void StringLtEvals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "char-key").Lt("x"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "char-key").Lt("a"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    //----------------------------------------------------------------------------
    //
    // LTE
    //
    //----------------------------------------------------------------------------
    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void IntLteEvals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "int-key").Lte(10))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "int-key").Lte(1))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "int-key").Lte(0))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void LongLteEvals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "long-key").Lte(4000000000L))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "long-key").Lte(3000000000L))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "long-key").Lte(666L))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void DoubleLteEvals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "double-key").Lte(100.1D))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "double-key").Lte(10.1D))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "double-key").Lte(1.1D))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    //----------------------------------------------------------------------------
    //
    // GT
    //
    //----------------------------------------------------------------------------
    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void IntGtEvals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "int-key").Gt(10))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "int-key").Gt(0))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void LongGtEvals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "long-key").Gt(4000000000L))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "long-key").Gt(666L))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void DoubleGtEvals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "double-key").Gt(100.1D))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "double-key").Gt(1.1D))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void StringGtEvals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "char-key").Gt("x"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "char-key").Gt("a"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    //----------------------------------------------------------------------------
    //
    // GTE
    //
    //----------------------------------------------------------------------------
    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void IntGteEvals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "int-key").Gte(10))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "int-key").Gte(1))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "int-key").Gte(0))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void LongGteEvals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "long-key").Gte(4000000000L))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "long-key").Gte(3000000000L))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "long-key").Gte(666L))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void DoubleGteEvals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "double-key").Gte(100.1D))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "double-key").Gte(10.1D))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "double-key").Gte(1.1D))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    //----------------------------------------------------------------------------
    //
    // Regex
    //
    //----------------------------------------------------------------------------
    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void StringRegexEvals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "string-key").Regex(new Regex("^string$")))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "string-key").Regex(new Regex("^tring$")))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "null-key").Regex(new Regex("^string$")))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "int-key").Regex(new Regex("^string$")))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void ListRegexEvals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "string-arr").Regex(new Regex("^d$")))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "string-arr").Regex(new Regex("^q$")))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void ObjRegexDoesntBreak(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "obj").Regex(new Regex("^foo$")))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    //----------------------------------------------------------------------------
    //
    // JSON equality
    //
    //----------------------------------------------------------------------------
    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void JsonEvals(IProviderTypeTestCase testCase)
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
    public void StringInEvals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "string-key").In("a", null, "string"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "string-key").In("a", null))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "null-key").In("a", null))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "null-key").In("a", "b"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "string-arr").In("a"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    //----------------------------------------------------------------------------
    //
    // NIN
    //
    //----------------------------------------------------------------------------
    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void StringNinEvals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "string-key").Nin("a", null, "string"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "string-key").Nin("a", null))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "null-key").Nin("a", null))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "null-key").Nin("a", "b"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "string-arr").Nin("a"))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    //----------------------------------------------------------------------------
    //
    // ALL
    //
    //----------------------------------------------------------------------------
    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void IntAllEvals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "int-arr").All(new List<object?> { 0, 1 }))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "int-arr").All(new List<object?> { 0, 7 }))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void StringAllEvals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "string-arr").All(new List<object?> { "a", "b" }))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "string-arr").All(new List<object?> { "a", "x" }))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void NotArrayAllEvals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "string-key").All(new List<object?> { "a", "b" }))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    //----------------------------------------------------------------------------
    //
    // SIZE
    //
    //----------------------------------------------------------------------------
    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void ArraySizeEvals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "string-arr").Size(5))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "string-arr").Size(7))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void StringSizeEvals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "string-key").Size(6))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "string-key").Size(7))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void OtherSizeEvals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "int-key").Size(6))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void NullSizeEvals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "null-key").Size(6))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    //----------------------------------------------------------------------------
    //
    // SUBSETOF
    //
    //----------------------------------------------------------------------------
    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void ArraySubsetofEvals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        // list is a superset
        var list = new ObjectList("a", "b", "c", "d", "e", "f", "g");
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "string-arr").SubsetOf(list))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        // list is exactly the same set (but in a different order)
        list = new ObjectList("e", "d", "b", "c", "a");
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "string-arr").SubsetOf(list))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        // list is missing one element
        list = new ObjectList("a", "b", "c", "d");
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "string-arr").SubsetOf(list))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    //----------------------------------------------------------------------------
    //
    // ANYOF
    //
    //----------------------------------------------------------------------------
    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void ArrayAnyofEvals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var list = new ObjectList("a", "z");
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "string-arr").AnyOf(list))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        list = new ObjectList("z", "b", "a");
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "string-arr").AnyOf(list))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        list = new ObjectList("x", "y", "z");
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "string-arr").AnyOf(list))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    //----------------------------------------------------------------------------
    //
    // NONEOF
    //
    //----------------------------------------------------------------------------
    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void ArrayNoneofEvals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var list = new ObjectList("a", "z");
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "string-arr").NoneOf(list))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        list = new ObjectList("z", "b", "a");
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "string-arr").NoneOf(list))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        list = new ObjectList("x", "y", "z");
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "string-arr").NoneOf(list))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    //----------------------------------------------------------------------------
    //
    // EXISTS
    //
    //----------------------------------------------------------------------------
    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void ExistsEvals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "string-key").Exists(true))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "string-key").Exists(false))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));

        Assert.False(Filter.Create(Criteria.Where(jsonProvider, "missing-key").Exists(true))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "missing-key").Exists(false))
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
    public void TypeEvals(bool expectedValue, string where, Type type)
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
    public void NotEmptyEvals(IProviderTypeTestCase testCase)
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
    public void EmptyEvals(IProviderTypeTestCase testCase)
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
    public void MatchesEvals(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var predicate = SimplePredicate.Create(context =>
        {
            var t = context.Item as IDictionary<string, object?>;
            var i = Convert.ToInt32(t["int-key"]);

            return i == 1;
        });
        Assert.True(Filter.Create(Criteria.Where(jsonProvider, "string-key").Eq("string")
                .And("$").Matches(predicate))
            .Apply(CreatePredicateContext(GetJson(testCase), testCase)));
    }

    //----------------------------------------------------------------------------
    //
    // OR
    //
    //----------------------------------------------------------------------------
    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void OrAndFiltersEvaluates(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var model = new Dictionary<string, object?> { { "foo", true }, { "bar", false } };


        var isFoo = Filter.Create(Criteria.Where(jsonProvider, "foo").Is(true));
        var isBar = Filter.Create(Criteria.Where(jsonProvider, "bar").Is(true));


        var fooOrBar = Filter.Create(Criteria.Where(jsonProvider, "foo").Is(true))
            .Or(Criteria.Where(jsonProvider, "bar").Is(true));
        var fooAndBar = Filter.Create(Criteria.Where(jsonProvider, "foo").Is(true))
            .And(Criteria.Where(jsonProvider, "bar").Is(true));

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
    public void CriteriaCanBeParsed()
    {
        var criteria = Filter.Parse("[?(@.foo == 'baar')]");
        Assert.Equal("[?(@['foo'] == 'baar')]", criteria.ToString());

        criteria = Filter.Parse("[?(@.foo)]");
        Assert.Equal("[?(@['foo'])]", criteria.ToString());
    }


    [Fact]
    public void InlineInCriteriaEvaluates()
    {
        var list = JsonPath.Read(JsonTestData.JsonDocument, "$.store.book[?(@.category in ['reference', 'fiction'])]")
            .AsList();
        Assert.Equal(4, list.Count);
    }
}