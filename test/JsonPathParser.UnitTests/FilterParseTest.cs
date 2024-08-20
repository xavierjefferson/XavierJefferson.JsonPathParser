using System.Text.RegularExpressions;
using XavierJefferson.JsonPathParser.Exceptions;
using XavierJefferson.JsonPathParser.Filtering;
using XavierJefferson.JsonPathParser.UnitTests.TestData;

namespace XavierJefferson.JsonPathParser.UnitTests;

public class FilterParseTest
{
    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void a_filter_can_be_parsed(IProviderTypeTestCase testCase)
    {
        Filter.Parse("[?(@.foo)]");
        Filter.Parse("[?(@.foo == 1)]");
        Filter.Parse("[?(@.foo == 1 || @['bar'])]");
        Filter.Parse("[?(@.foo == 1 && @['bar'])]");
    }

    [Fact]
    public void an_invalid_filter_can_not_be_parsed()
    {
        Assert.Throws<InvalidPathException>(() => { Filter.Parse("[?(@.foo == 1)"); });
        Assert.Throws<InvalidPathException>(() => { Filter.Parse("[?(@.foo == 1) ||]"); });
        Assert.Throws<InvalidPathException>(() => { Filter.Parse("[(@.foo == 1)]"); });
        Assert.Throws<InvalidPathException>(() => { Filter.Parse("[?@.foo == 1)]"); });
    }


    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void a_gte_filter_can_be_serialized(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var filter = Filter.Create(Criteria.Where(jsonProvider, "a").Gte(jsonProvider, 1)).ToString();
        var parsed = Filter.Parse("[?(@['a'] >= 1)]").ToString();

        Assert.Equal(Filter.Parse(parsed).ToString(), filter);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void a_lte_filter_can_be_serialized(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var filter = Filter.Create(Criteria.Where(jsonProvider, "a").Lte(jsonProvider, 1)).ToString();
        var parsed = Filter.Parse("[?(@['a'] <= 1)]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void a_eq_filter_can_be_serialized(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var filter = Filter.Create(Criteria.Where(jsonProvider, "a").Eq(jsonProvider, 1)).ToString();
        var parsed = Filter.Parse("[?(@['a'] == 1)]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void a_ne_filter_can_be_serialized(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var filter = Filter.Create(Criteria.Where(jsonProvider, "a").Ne(jsonProvider, 1)).ToString();
        var parsed = Filter.Parse("[?(@['a'] != 1)]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void a_lt_filter_can_be_serialized(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var filter = Filter.Create(Criteria.Where(jsonProvider, "a").Lt(jsonProvider, 1)).ToString();
        var parsed = Filter.Parse("[?(@['a'] < 1)]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void a_gt_filter_can_be_serialized(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var filter = Filter.Create(Criteria.Where(jsonProvider, "a").Gt(jsonProvider, 1)).ToString();
        var parsed = Filter.Parse("[?(@['a'] > 1)]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void a_nin_filter_can_be_serialized(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var filter = Filter.Create(Criteria.Where(jsonProvider, "a").Nin(jsonProvider, 1)).ToString();
        var parsed = Filter.Parse("[?(@['a'] NIN [1])]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void a_in_filter_can_be_serialized(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var filter = Filter.Create(Criteria.Where(jsonProvider, "a").In(jsonProvider, "a")).ToString();
        var parsed = Filter.Parse("[?(@['a'] IN ['a'])]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void a_contains_filter_can_be_serialized(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var filter = Filter.Create(Criteria.Where(jsonProvider, "a").Contains(jsonProvider, "a")).ToString();
        var parsed = Filter.Parse("[?(@['a'] CONTAINS 'a')]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void a_all_filter_can_be_serialized(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var filter = Filter.Create(Criteria.Where(jsonProvider, "a").All(jsonProvider, new List<object?> { "a", "b" })).ToString();
        var parsed = Filter.Parse("[?(@['a'] ALL ['a','b'])]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void a_size_filter_can_be_serialized(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var filter = Filter.Create(Criteria.Where(jsonProvider, "a").Size(jsonProvider, 5)).ToString();
        var parsed = Filter.Parse("[?(@['a'] SIZE 5)]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void a_subsetof_filter_can_be_serialized(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var filter = Filter.Create(Criteria.Where(jsonProvider, "a").SubsetOf(jsonProvider)).ToString();
        var parsed = Filter.Parse("[?(@['a'] SUBSETOF [])]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void a_anyof_filter_can_be_serialized(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var filter = Filter.Create(Criteria.Where(jsonProvider, "a").AnyOf(jsonProvider)).ToString();
        var parsed = Filter.Parse("[?(@['a'] ANYOF [])]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void a_noneof_filter_can_be_serialized(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var filter = Filter.Create(Criteria.Where(jsonProvider, "a").NoneOf(jsonProvider)).ToString();
        var parsed = Filter.Parse("[?(@['a'] NONEOF [])]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void a_exists_filter_can_be_serialized(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var a = Filter.Create(Criteria.Where(jsonProvider, "a").Exists(jsonProvider, true));
        var filter = a.ToString();
        var parsed = Filter.Parse("[?(@['a'])]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void a_not_exists_filter_can_be_serialized(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var filter = Filter.Create(Criteria.Where(jsonProvider, "a").Exists(jsonProvider, false)).ToString();
        var parsed = Filter.Parse("[?(!@['a'])]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void a_type_filter_can_be_serialized(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.Equal($"[?(@['a'] TYPE {typeof(string).FullName})]",
            Filter.Create(Criteria.Where(jsonProvider, "a").Type(typeof(string))).ToString());
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void a_matches_filter_can_be_serialized(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var a = Filter.Create(Criteria.Where(jsonProvider, "x").Eq(jsonProvider, 1000));

        Assert.Equal("[?(@['a'] MATCHES [?(@['x'] == 1000)])]",
            Filter.Create(Criteria.Where(jsonProvider, "a").Matches(a)).ToString());
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void a_not_empty_filter_can_be_serialized(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var filter = Filter.Create(Criteria.Where(jsonProvider, "a").Empty(false)).ToString();
        var parsed = Filter.Parse("[?(@['a'] EMPTY false)]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void and_filter_can_be_serialized(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var filter = Filter.Create(Criteria.Where(jsonProvider, "a").Eq(jsonProvider, 1).And(jsonProvider, "b").Eq(jsonProvider, 2)).ToString();
        var parsed = Filter.Parse("[?(@['a'] == 1 && @['b'] == 2)]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void in_string_filter_can_be_serialized(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var filter = Filter.Create(Criteria.Where(jsonProvider, "a").In(jsonProvider, "1", "2")).ToString();
        var parsed = Filter.Parse("[?(@['a'] IN ['1','2'])]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void a_deep_path_filter_can_be_serialized(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var filter = Filter.Create(Criteria.Where(jsonProvider, "a.b.c").In(jsonProvider, "1", "2")).ToString();
        var parsed = Filter.Parse("[?(@['a']['b']['c'] IN ['1','2'])]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void a_regex_filter_can_be_serialized(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.Equal("[?(@['a'] =~ /.*?/i)]",
            Filter.Create(Criteria.Where(jsonProvider, "a").Regex(jsonProvider, new Regex(".*?", RegexOptions.IgnoreCase))).ToString());
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void a_doc_ref_filter_can_be_serialized(IProviderTypeTestCase testCase)
    {
        var f = Filter.Parse("[?(@.display-price <= $.max-price)]");
        Assert.Equal("[?(@['display-price'] <= $['max-price'])]", f.ToString());
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void and_combined_filters_can_be_serialized(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var a = Filter.Create(Criteria.Where(jsonProvider, "a").Eq(jsonProvider, 1));
        var b = Filter.Create(Criteria.Where(jsonProvider, "b").Eq(jsonProvider, 2));
        var c = a.And(b);


        var parsed = Filter.Parse("[?(@['a'] == 1 && @['b'] == 2)]").ToString();
        var filter = c.ToString();

        Assert.Equal(parsed, filter);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void or_combined_filters_can_be_serialized(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var a = Filter.Create(Criteria.Where(jsonProvider, "a").Eq(jsonProvider, 1));
        var b = Filter.Create(Criteria.Where(jsonProvider, "b").Eq(jsonProvider, 2));
        var c = a.Or(b);


        var d = Filter.Parse("[?(@['a'] == 1 || @['b'] == 2)]");
        var parsed = d.ToString();
        var filter = c.ToString();
        Assert.Equal(parsed, filter);
    }
}