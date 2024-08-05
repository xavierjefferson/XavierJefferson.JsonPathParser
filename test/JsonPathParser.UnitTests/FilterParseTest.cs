using System.Text.RegularExpressions;
using XavierJefferson.JsonPathParser.Exceptions;

namespace XavierJefferson.JsonPathParser.UnitTests;

public class FilterParseTest
{
    [Fact]
    public void a_filter_can_be_parsed()
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


    [Fact]
    public void a_gte_filter_can_be_serialized()
    {
        var filter = Filter.Create(Criteria.Where("a").Gte(1)).ToString();
        var parsed = Filter.Parse("[?(@['a'] >= 1)]").ToString();

        Assert.Equal(Filter.Parse(parsed).ToString(), filter);
    }

    [Fact]
    public void a_lte_filter_can_be_serialized()
    {
        var filter = Filter.Create(Criteria.Where("a").Lte(1)).ToString();
        var parsed = Filter.Parse("[?(@['a'] <= 1)]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Fact]
    public void a_eq_filter_can_be_serialized()
    {
        var filter = Filter.Create(Criteria.Where("a").Eq(1)).ToString();
        var parsed = Filter.Parse("[?(@['a'] == 1)]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Fact]
    public void a_ne_filter_can_be_serialized()
    {
        var filter = Filter.Create(Criteria.Where("a").Ne(1)).ToString();
        var parsed = Filter.Parse("[?(@['a'] != 1)]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Fact]
    public void a_lt_filter_can_be_serialized()
    {
        var filter = Filter.Create(Criteria.Where("a").Lt(1)).ToString();
        var parsed = Filter.Parse("[?(@['a'] < 1)]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Fact]
    public void a_gt_filter_can_be_serialized()
    {
        var filter = Filter.Create(Criteria.Where("a").Gt(1)).ToString();
        var parsed = Filter.Parse("[?(@['a'] > 1)]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Fact]
    public void a_nin_filter_can_be_serialized()
    {
        var filter = Filter.Create(Criteria.Where("a").Nin(1)).ToString();
        var parsed = Filter.Parse("[?(@['a'] NIN [1])]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Fact]
    public void a_in_filter_can_be_serialized()
    {
        var filter = Filter.Create(Criteria.Where("a").In("a")).ToString();
        var parsed = Filter.Parse("[?(@['a'] IN ['a'])]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Fact]
    public void a_contains_filter_can_be_serialized()
    {
        var filter = Filter.Create(Criteria.Where("a").Contains("a")).ToString();
        var parsed = Filter.Parse("[?(@['a'] CONTAINS 'a')]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Fact]
    public void a_all_filter_can_be_serialized()
    {
        var filter = Filter.Create(Criteria.Where("a").All(new JpObjectList { "a", "b" })).ToString();
        var parsed = Filter.Parse("[?(@['a'] ALL ['a','b'])]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Fact]
    public void a_size_filter_can_be_serialized()
    {
        var filter = Filter.Create(Criteria.Where("a").Size(5)).ToString();
        var parsed = Filter.Parse("[?(@['a'] SIZE 5)]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Fact]
    public void a_subsetof_filter_can_be_serialized()
    {
        var filter = Filter.Create(Criteria.Where("a").SubsetOf()).ToString();
        var parsed = Filter.Parse("[?(@['a'] SUBSETOF [])]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Fact]
    public void a_anyof_filter_can_be_serialized()
    {
        var filter = Filter.Create(Criteria.Where("a").AnyOf()).ToString();
        var parsed = Filter.Parse("[?(@['a'] ANYOF [])]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Fact]
    public void a_noneof_filter_can_be_serialized()
    {
        var filter = Filter.Create(Criteria.Where("a").NoneOf()).ToString();
        var parsed = Filter.Parse("[?(@['a'] NONEOF [])]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Fact]
    public void a_exists_filter_can_be_serialized()
    {
        var a = Filter.Create(Criteria.Where("a").Exists(true));
        var filter = a.ToString();
        var parsed = Filter.Parse("[?(@['a'])]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Fact]
    public void a_not_exists_filter_can_be_serialized()
    {
        var filter = Filter.Create(Criteria.Where("a").Exists(false)).ToString();
        var parsed = Filter.Parse("[?(!@['a'])]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Fact]
    public void a_type_filter_can_be_serialized()
    {
        Assert.Equal($"[?(@['a'] TYPE {typeof(string).FullName})]",
            Filter.Create(Criteria.Where("a").Type(typeof(string))).ToString());
    }

    [Fact]
    public void a_matches_filter_can_be_serialized()
    {
        var a = Filter.Create(Criteria.Where("x").Eq(1000));

        Assert.Equal("[?(@['a'] MATCHES [?(@['x'] == 1000)])]",
            Filter.Create(Criteria.Where("a").Matches(a)).ToString());
    }

    [Fact]
    public void a_not_empty_filter_can_be_serialized()
    {
        var filter = Filter.Create(Criteria.Where("a").Empty(false)).ToString();
        var parsed = Filter.Parse("[?(@['a'] EMPTY false)]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Fact]
    public void and_filter_can_be_serialized()
    {
        var filter = Filter.Create(Criteria.Where("a").Eq(1).And("b").Eq(2)).ToString();
        var parsed = Filter.Parse("[?(@['a'] == 1 && @['b'] == 2)]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Fact]
    public void in_string_filter_can_be_serialized()
    {
        var filter = Filter.Create(Criteria.Where("a").In("1", "2")).ToString();
        var parsed = Filter.Parse("[?(@['a'] IN ['1','2'])]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Fact]
    public void a_deep_path_filter_can_be_serialized()
    {
        var filter = Filter.Create(Criteria.Where("a.b.c").In("1", "2")).ToString();
        var parsed = Filter.Parse("[?(@['a']['b']['c'] IN ['1','2'])]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Fact]
    public void a_regex_filter_can_be_serialized()
    {
        Assert.Equal("[?(@['a'] =~ /.*?/i)]",
            Filter.Create(Criteria.Where("a").Regex(new Regex(".*?", RegexOptions.IgnoreCase))).ToString());
    }

    [Fact]
    public void a_doc_ref_filter_can_be_serialized()
    {
        var f = Filter.Parse("[?(@.display-price <= $.max-price)]");
        Assert.Equal("[?(@['display-price'] <= $['max-price'])]", f.ToString());
    }

    [Fact]
    public void and_combined_filters_can_be_serialized()
    {
        var a = Filter.Create(Criteria.Where("a").Eq(1));
        var b = Filter.Create(Criteria.Where("b").Eq(2));
        var c = a.And(b);


        var parsed = Filter.Parse("[?(@['a'] == 1 && @['b'] == 2)]").ToString();
        var filter = c.ToString();

        Assert.Equal(parsed, filter);
    }

    [Fact]
    public void or_combined_filters_can_be_serialized()
    {
        var a = Filter.Create(Criteria.Where("a").Eq(1));
        var b = Filter.Create(Criteria.Where("b").Eq(2));
        var c = a.Or(b);


        var d = Filter.Parse("[?(@['a'] == 1 || @['b'] == 2)]");
        var parsed = d.ToString();
        var filter = c.ToString();
        Assert.Equal(parsed, filter);
    }
}