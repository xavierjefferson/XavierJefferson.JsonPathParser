using System.Text.RegularExpressions;
using XavierJefferson.JsonPathParser.Exceptions;
using XavierJefferson.JsonPathParser.Filtering;
using XavierJefferson.JsonPathParser.UnitTests.TestData;

namespace XavierJefferson.JsonPathParser.UnitTests;

public class FilterParseTest
{
    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void AFilterCanBeParsed(IProviderTypeTestCase testCase)
    {
        Filter.Parse("[?(@.foo)]");
        Filter.Parse("[?(@.foo == 1)]");
        Filter.Parse("[?(@.foo == 1 || @['bar'])]");
        Filter.Parse("[?(@.foo == 1 && @['bar'])]");
    }

    [Fact]
    public void AnInvalidFilterCanNotBeParsed()
    {
        Assert.Throws<InvalidPathException>(() => { Filter.Parse("[?(@.foo == 1)"); });
        Assert.Throws<InvalidPathException>(() => { Filter.Parse("[?(@.foo == 1) ||]"); });
        Assert.Throws<InvalidPathException>(() => { Filter.Parse("[(@.foo == 1)]"); });
        Assert.Throws<InvalidPathException>(() => { Filter.Parse("[?@.foo == 1)]"); });
    }


    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void AGteFilterCanBeSerialized(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var filter = Filter.Create(Criteria.Where(jsonProvider, "a").Gte(1)).ToString();
        var parsed = Filter.Parse("[?(@['a'] >= 1)]").ToString();

        Assert.Equal(Filter.Parse(parsed).ToString(), filter);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void ALteFilterCanBeSerialized(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var filter = Filter.Create(Criteria.Where(jsonProvider, "a").Lte(1)).ToString();
        var parsed = Filter.Parse("[?(@['a'] <= 1)]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void AEqFilterCanBeSerialized(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var filter = Filter.Create(Criteria.Where(jsonProvider, "a").Eq(1)).ToString();
        var parsed = Filter.Parse("[?(@['a'] == 1)]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void ANeFilterCanBeSerialized(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var filter = Filter.Create(Criteria.Where(jsonProvider, "a").Ne(1)).ToString();
        var parsed = Filter.Parse("[?(@['a'] != 1)]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void ALtFilterCanBeSerialized(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var filter = Filter.Create(Criteria.Where(jsonProvider, "a").Lt(1)).ToString();
        var parsed = Filter.Parse("[?(@['a'] < 1)]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void AGtFilterCanBeSerialized(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var filter = Filter.Create(Criteria.Where(jsonProvider, "a").Gt(1)).ToString();
        var parsed = Filter.Parse("[?(@['a'] > 1)]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void ANinFilterCanBeSerialized(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var filter = Filter.Create(Criteria.Where(jsonProvider, "a").Nin(1)).ToString();
        var parsed = Filter.Parse("[?(@['a'] NIN [1])]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void AInFilterCanBeSerialized(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var filter = Filter.Create(Criteria.Where(jsonProvider, "a").In("a")).ToString();
        var parsed = Filter.Parse("[?(@['a'] IN ['a'])]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void AContainsFilterCanBeSerialized(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var filter = Filter.Create(Criteria.Where(jsonProvider, "a").Contains("a")).ToString();
        var parsed = Filter.Parse("[?(@['a'] CONTAINS 'a')]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void AAllFilterCanBeSerialized(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var filter = Filter.Create(Criteria.Where(jsonProvider, "a").All(new List<object?> { "a", "b" })).ToString();
        var parsed = Filter.Parse("[?(@['a'] ALL ['a','b'])]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void ASizeFilterCanBeSerialized(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var filter = Filter.Create(Criteria.Where(jsonProvider, "a").Size(5)).ToString();
        var parsed = Filter.Parse("[?(@['a'] SIZE 5)]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void ASubsetofFilterCanBeSerialized(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var filter = Filter.Create(Criteria.Where(jsonProvider, "a").SubsetOf()).ToString();
        var parsed = Filter.Parse("[?(@['a'] SUBSETOF [])]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void AAnyofFilterCanBeSerialized(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var filter = Filter.Create(Criteria.Where(jsonProvider, "a").AnyOf()).ToString();
        var parsed = Filter.Parse("[?(@['a'] ANYOF [])]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void ANoneofFilterCanBeSerialized(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var filter = Filter.Create(Criteria.Where(jsonProvider, "a").NoneOf()).ToString();
        var parsed = Filter.Parse("[?(@['a'] NONEOF [])]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void AExistsFilterCanBeSerialized(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var a = Filter.Create(Criteria.Where(jsonProvider, "a").Exists(true));
        var filter = a.ToString();
        var parsed = Filter.Parse("[?(@['a'])]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void ANotExistsFilterCanBeSerialized(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var filter = Filter.Create(Criteria.Where(jsonProvider, "a").Exists(false)).ToString();
        var parsed = Filter.Parse("[?(!@['a'])]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void ATypeFilterCanBeSerialized(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.Equal($"[?(@['a'] TYPE {typeof(string).FullName})]",
            Filter.Create(Criteria.Where(jsonProvider, "a").Type(typeof(string))).ToString());
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void AMatchesFilterCanBeSerialized(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var a = Filter.Create(Criteria.Where(jsonProvider, "x").Eq(1000));

        Assert.Equal("[?(@['a'] MATCHES [?(@['x'] == 1000)])]",
            Filter.Create(Criteria.Where(jsonProvider, "a").Matches(a)).ToString());
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void ANotEmptyFilterCanBeSerialized(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var filter = Filter.Create(Criteria.Where(jsonProvider, "a").Empty(false)).ToString();
        var parsed = Filter.Parse("[?(@['a'] EMPTY false)]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void AndFilterCanBeSerialized(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var filter = Filter.Create(Criteria.Where(jsonProvider, "a").Eq(1).And("b").Eq(2)).ToString();
        var parsed = Filter.Parse("[?(@['a'] == 1 && @['b'] == 2)]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void InStringFilterCanBeSerialized(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var filter = Filter.Create(Criteria.Where(jsonProvider, "a").In("1", "2")).ToString();
        var parsed = Filter.Parse("[?(@['a'] IN ['1','2'])]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void ADeepPathFilterCanBeSerialized(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var filter = Filter.Create(Criteria.Where(jsonProvider, "a.b.c").In("1", "2")).ToString();
        var parsed = Filter.Parse("[?(@['a']['b']['c'] IN ['1','2'])]").ToString();

        Assert.Equal(parsed, filter);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void ARegexFilterCanBeSerialized(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        Assert.Equal("[?(@['a'] =~ /.*?/i)]",
            Filter.Create(Criteria.Where(jsonProvider, "a").Regex(new Regex(".*?", RegexOptions.IgnoreCase)))
                .ToString());
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void ADocRefFilterCanBeSerialized(IProviderTypeTestCase testCase)
    {
        var f = Filter.Parse("[?(@.display-price <= $.max-price)]");
        Assert.Equal("[?(@['display-price'] <= $['max-price'])]", f.ToString());
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void AndCombinedFiltersCanBeSerialized(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var a = Filter.Create(Criteria.Where(jsonProvider, "a").Eq(1));
        var b = Filter.Create(Criteria.Where(jsonProvider, "b").Eq(2));
        var c = a.And(b);


        var parsed = Filter.Parse("[?(@['a'] == 1 && @['b'] == 2)]").ToString();
        var filter = c.ToString();

        Assert.Equal(parsed, filter);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void OrCombinedFiltersCanBeSerialized(IProviderTypeTestCase testCase)
    {
        var jsonProvider = testCase.Configuration.JsonProvider;
        var a = Filter.Create(Criteria.Where(jsonProvider, "a").Eq(1));
        var b = Filter.Create(Criteria.Where(jsonProvider, "b").Eq(2));
        var c = a.Or(b);


        var d = Filter.Parse("[?(@['a'] == 1 || @['b'] == 2)]");
        var parsed = d.ToString();
        var filter = c.ToString();
        Assert.Equal(parsed, filter);
    }
}