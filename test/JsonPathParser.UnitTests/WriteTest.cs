using XavierJefferson.JsonPathParser.Exceptions;
using XavierJefferson.JsonPathParser.UnitTests.Extensions;
using XavierJefferson.JsonPathParser.UnitTests.TestData;

namespace XavierJefferson.JsonPathParser.UnitTests;

//test
public class WriteTest : TestUtils
{
    private static readonly Dictionary<string, object?> EmptyMap = new();

    private readonly MapDelegate _toStringMapFunction = (currentValue, configuration) =>
    {
        return currentValue + "converted";
    };

    [Fact]
    public void AnArrayChildPropertyCanBeUpdated()
    {
        var o = JsonPath.Parse(JsonTestData.JsonDocument).Set("$.store.book[*].display-price", 1).Json;

        var result = JsonPath.Parse(o).Read("$.store.book[*].display-price").AsList();

        MyAssert.ContainsExactly(result, 1, 1, 1, 1);
    }


    [Fact]
    public void AnRootPropertyCanBeUpdated()
    {
        var o = JsonPath.Parse(JsonTestData.JsonDocument).Set("$.int-max-property", 1).Json;

        var result = JsonPath.Parse(o).Read("$.int-max-property");

        Assert.Equal(1, result);
    }

    [Fact]
    public void AnDeepScanCanUpdate()
    {
        var o = JsonPath.Parse(JsonTestData.JsonDocument).Set("$..display-price", 1).Json;

        var result = JsonPath.Parse(o).Read("$..display-price").AsList();

        MyAssert.ContainsExactly(result, 1, 1, 1, 1, 1);
    }


    [Fact]
    public void AnFilterCanUpdate()
    {
        var o = JsonPath.Parse(JsonTestData.JsonDocument).Set("$.store.book[?(@.display-price)].display-price", 1).Json;

        var result = JsonPath.Parse(o).Read("$.store.book[?(@.display-price)].display-price").AsList();

        MyAssert.ContainsExactly(result, 1, 1, 1, 1);
    }

    [Fact]
    public void APathCanBeDeleted()
    {
        var o = JsonPath.Parse(JsonTestData.JsonDocument).Delete("$.store.book[*].display-price").Json;

        var result = JsonPath.Parse(o).Read("$.store.book[*].display-price").AsList();

        Assert.Empty(result);
    }

    [Fact]
    public void OperationsCanChained()
    {
        var o = JsonPath.Parse(JsonTestData.JsonDocument)
            .Delete("$.store.book[*].display-price")
            .Set("$.store.book[*].category", "A")
            .Json;

        var prices = JsonPath.Parse(o).Read("$.store.book[*].display-price").AsList();
        var categories = JsonPath.Parse(o).Read("$.store.book[*].category").AsList();

        Assert.Empty(prices);
        MyAssert.ContainsExactly(categories, "A", "A", "A", "A");
    }

    [Fact]
    public void AnArrayCanBeUpdated()
    {
        var ints = JsonPath.Parse("[0,1,2,3]").Set("$[?(@ == 1)]", 9).Json.AsList();

        MyAssert.ContainsExactly(ints, 0d, 9, 2d, 3d);
    }

    [Fact]
    public void AnArrayIndexCanBeUpdated()
    {
        var res = JsonPath.Parse(JsonTestData.JsonDocument).Set("$.store.book[0]", "a").Read("$.store.book[0]");

        Assert.Equal("a", res);
    }

    [Fact]
    public void AnArraySliceCanBeUpdated()
    {
        var res = JsonPath.Parse(JsonTestData.JsonDocument).Set("$.store.book[0:2]", "a").Read("$.store.book[0:2]")
            .AsList();

        MyAssert.ContainsExactly(res, "a", "a");
    }

    [Fact]
    public void AnArrayCriteriaCanBeUpdated()
    {
        var res = JsonPath.Parse(JsonTestData.JsonDocument)
            .Set("$.store.book[?(@.category == 'fiction')]", "a")
            .Read("$.store.book[?(@ == 'a')]").AsList();

        MyAssert.ContainsExactly(res, "a", "a", "a");
    }

    [Fact]
    public void AnArrayCriteriaCanBeDeleted()
    {
        var res = JsonPath.Parse(JsonTestData.JsonDocument)
            .Delete("$.store.book[?(@.category == 'fiction')]")
            .Read("$.store.book[*].category").AsList();

        MyAssert.ContainsExactly(res, "reference");
    }

    [Fact]
    public void AnArrayCriteriaWithMultipleResultsCanBeDeleted()
    {
        using (var stream = GetResourceAsStream("json_array_multiple_delete.json"))
        {
            var deletePath = "$._embedded.mandates[?(@.count=~/0/)]";
            var documentContext = JsonPath.Parse(stream);
            documentContext.Delete(deletePath);
            var result = documentContext.Read(deletePath).AsList();
            Assert.Empty(result);
        }
    }


    [Fact]
    public void MultiPropDelete()
    {
        var res = JsonPath.Parse(JsonTestData.JsonDocument).Delete("$.store.book[*]['author', 'category']")
            .Read("$.store.book[*]['author', 'category']").AsListOfMap();

        MyAssert.ContainsExactly(res, EmptyMap, EmptyMap, EmptyMap, EmptyMap);
    }

    [Fact]
    public void MultiPropUpdate()
    {
        var expected = new Dictionary<string, object?>
        {
            { "author", "a" },
            { "category", "a" }
        };

        var res = JsonPath.Parse(JsonTestData.JsonDocument).Set("$.store.book[*]['author', 'category']", "a")
            .Read("$.store.book[*]['author', 'category']").AsListOfMap();

        MyAssert.ContainsExactly(res, expected, expected, expected, expected);
    }


    [Fact]
    public void MultiPropUpdateNotAllDefined()
    {
        var expected = new Dictionary<string, object?>
        {
            { "author", "a" },
            { "isbn", "a" }
        };

        var res = JsonPath.Parse(JsonTestData.JsonDocument).Set("$.store.book[*]['author', 'isbn']", "a")
            .Read("$.store.book[*]['author', 'isbn']").AsListOfMap();

        MyAssert.ContainsExactly(res, expected, expected, expected, expected);
    }

    [Fact]
    public void AddToArray()
    {
        var res = JsonPath.Parse(JsonTestData.JsonDocument).Add("$.store.book", 1).Read("$.store.book[4]");
        Assert.Equal(1, res);
    }

    [Fact]
    public void AddToObject()
    {
        var res = JsonPath.Parse(JsonTestData.JsonDocument).Put("$.store.book[0]", "new-key", "new-value")
            .Read("$.store.book[0].new-key");
        Assert.Equal("new-value", res);
    }

    [Fact]
    public void ItemCanBeAddedToRootArray()
    {
        var model = new List<object?>();
        model.Add(1);
        model.Add(2);

        var ints = JsonPath.Parse(model).Add("$", 3).Read("$").AsList();

        MyAssert.ContainsExactly(ints, 1, 2, 3);
    }

    [Fact]
    public void KeyValCanBeAddedToRootObject()
    {
        var model = new Dictionary<string, object?>();
        model["a"] = "a-val";

        var newVal = JsonPath.Parse(model).Put("$", "new-key", "new-val").Read<string>("$.new-key");

        Assert.Equal("new-val", newVal);
    }

    [Fact]
    public void AddToObjectOnArray()
    {
        Assert.Throws<InvalidModificationException>(() =>
            JsonPath.Parse(JsonTestData.JsonDocument).Put("$.store.book", "new-key", "new-value"));
    }

    [Fact]
    public void AddToArrayOnObject()
    {
        Assert.Throws<InvalidModificationException>(() =>
            JsonPath.Parse(JsonTestData.JsonDocument).Add("$.store.book[0]", "new-value"));
    }


    [Fact]
    public void RootObjectCanNotBeUpdated()
    {
        var model = new Dictionary<string, object?>();
        model["a"] = "a-val";

        Assert.Throws<InvalidModificationException>(() => JsonPath.Parse(model).Set("$[?(@.a == 'a-val')]", 1));
    }

    [Fact]
    public void APathCanBeRenamed()
    {
        var o = JsonPath.Parse(JsonTestData.JsonDocument).RenameKey("$.store", "book", "updated-book").Json;
        var result = JsonPath.Parse(o).Read("$.store.updated-book").AsList();

        Assert.True(result.Any());
    }

    [Fact]
    public void KeysInRootContainingMapCanBeRenamed()
    {
        var o = JsonPath.Parse(JsonTestData.JsonDocument).RenameKey("$", "store", "new-store").Json;
        var result = JsonPath.Parse(o).Read("$.new-store[*]").AsList();
        Assert.True(result.Any());
    }

    [Fact]
    public void MapArrayItemsCanBeRenamed()
    {
        var o = JsonPath.Parse(JsonTestData.JsonDocument).RenameKey("$.store.book[*]", "category", "renamed-category")
            .Json;
        var result = JsonPath.Parse(o).Read("$.store.book[*].renamed-category").AsList();
        Assert.True(result.Any());
    }

    [Fact]
    public void NonMapArrayItemsCannotBeRenamed()
    {
        var model = new List<int>();
        model.Add(1);
        model.Add(2);
        Assert.Throws<InvalidModificationException>(() => JsonPath.Parse(model).RenameKey("$[*]", "oldKey", "newKey"));
    }

    [Fact]
    public void MultiplePropertiesCannotBeRenamed()
    {
        Assert.Throws<InvalidModificationException>(() =>
            JsonPath.Parse(JsonTestData.JsonDocument)
                .RenameKey("$.store.book[*]['author', 'category']", "old-key", "new-key"));
    }

    [Fact]
    public void NonExistentKeyRenameNotAllowed()
    {
        Assert.Throws<PathNotFoundException>(() =>
            JsonPath.Parse(JsonTestData.JsonDocument).RenameKey("$", "fake", "new-fake").Json);
    }

    [Fact]
    public void RootCannotBeMapped()
    {
        Assert.Throws<InvalidModificationException>(() =>
            JsonPath.Parse(JsonTestData.JsonDocument).Map("$", _toStringMapFunction).Json);
    }

    [Fact]
    public void SingleMatchValueCanBeMapped()
    {
        var stringResult = JsonPath.Parse(JsonTestData.JsonDocument).Map("$.string-property", _toStringMapFunction)
            .Read<string>("$.string-property");
        Assert.EndsWith("converted", stringResult);
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void ObjectCanBeMapped(IProviderTypeTestCase testCase)
    {
        var documentContext = JsonPath.Using(testCase.Configuration)
            .Parse(JsonTestData.JsonDocument);
        var list = documentContext.Read("$..book");
        Assert.True(list is List<object?>);
        var result = documentContext.Map("$..book", _toStringMapFunction).Read("$..book").AsList()
            .Select(i => i.ToString()).First();
        Assert.True(result.EndsWith("converted"));
    }

    [Fact]
    public void MultiMatchPathCanBeMapped()
    {
        var doubleResult = JsonPath.Parse(JsonTestData.JsonDocument).Read("$..display-price").AsList();
        Assert.True(doubleResult.All(i => i is double));
        var stringResult = JsonPath.Parse(JsonTestData.JsonDocument).Map("$..display-price", _toStringMapFunction)
            .Read("$..display-price").AsList();

        Assert.True(stringResult.All(i => i is string));
        Assert.True(stringResult.All(i => i.ToString().EndsWith("converted")));
    }
}