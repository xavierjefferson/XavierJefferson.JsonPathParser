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
    public void an_array_child_property_can_be_updated()
    {
        var o = JsonPath.Parse(JsonTestData.JsonDocument).Set("$.store.book[*].display-price", 1).Json;

        var result = JsonPath.Parse(o).Read("$.store.book[*].display-price").AsList();

        MyAssert.ContainsExactly(result, 1, 1, 1, 1);
    }


    [Fact]
    public void an_root_property_can_be_updated()
    {
        var o = JsonPath.Parse(JsonTestData.JsonDocument).Set("$.int-max-property", 1).Json;

        var result = JsonPath.Parse(o).Read("$.int-max-property");

        Assert.Equal(1, result);
    }

    [Fact]
    public void an_deep_scan_can_update()
    {
        var o = JsonPath.Parse(JsonTestData.JsonDocument).Set("$..display-price", 1).Json;

        var result = JsonPath.Parse(o).Read("$..display-price").AsList();

        MyAssert.ContainsExactly(result, 1, 1, 1, 1, 1);
    }


    [Fact]
    public void an_filter_can_update()
    {
        var o = JsonPath.Parse(JsonTestData.JsonDocument).Set("$.store.book[?(@.display-price)].display-price", 1).Json;

        var result = JsonPath.Parse(o).Read("$.store.book[?(@.display-price)].display-price").AsList();

        MyAssert.ContainsExactly(result, 1, 1, 1, 1);
    }

    [Fact]
    public void a_path_can_be_deleted()
    {
        var o = JsonPath.Parse(JsonTestData.JsonDocument).Delete("$.store.book[*].display-price").Json;

        var result = JsonPath.Parse(o).Read("$.store.book[*].display-price").AsList();

        Assert.Empty(result);
    }

    [Fact]
    public void operations_can_chained()
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
    public void an_array_can_be_updated()
    {
        var ints = JsonPath.Parse("[0,1,2,3]").Set("$[?(@ == 1)]", 9).Json.AsList();

        MyAssert.ContainsExactly(ints, 0d, 9, 2d, 3d);
    }

    [Fact]
    public void an_array_index_can_be_updated()
    {
        var res = JsonPath.Parse(JsonTestData.JsonDocument).Set("$.store.book[0]", "a").Read("$.store.book[0]");

        Assert.Equal("a", res);
    }

    [Fact]
    public void an_array_slice_can_be_updated()
    {
        var res = JsonPath.Parse(JsonTestData.JsonDocument).Set("$.store.book[0:2]", "a").Read("$.store.book[0:2]")
            .AsList();

        MyAssert.ContainsExactly(res, "a", "a");
    }

    [Fact]
    public void an_array_criteria_can_be_updated()
    {
        var res = JsonPath.Parse(JsonTestData.JsonDocument)
            .Set("$.store.book[?(@.category == 'fiction')]", "a")
            .Read("$.store.book[?(@ == 'a')]").AsList();

        MyAssert.ContainsExactly(res, "a", "a", "a");
    }

    [Fact]
    public void an_array_criteria_can_be_deleted()
    {
        var res = JsonPath.Parse(JsonTestData.JsonDocument)
            .Delete("$.store.book[?(@.category == 'fiction')]")
            .Read("$.store.book[*].category").AsList();

        MyAssert.ContainsExactly(res, "reference");
    }

    [Fact]
    public void an_array_criteria_with_multiple_results_can_be_deleted()
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
    public void multi_prop_delete()
    {
        var res = JsonPath.Parse(JsonTestData.JsonDocument).Delete("$.store.book[*]['author', 'category']")
            .Read("$.store.book[*]['author', 'category']").AsListOfMap();

        MyAssert.ContainsExactly(res, EmptyMap, EmptyMap, EmptyMap, EmptyMap);
    }

    [Fact]
    public void multi_prop_update()
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
    public void multi_prop_update_not_all_defined()
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
    public void add_to_array()
    {
        var res = JsonPath.Parse(JsonTestData.JsonDocument).Add("$.store.book", 1).Read("$.store.book[4]");
        Assert.Equal(1, res);
    }

    [Fact]
    public void add_to_object()
    {
        var res = JsonPath.Parse(JsonTestData.JsonDocument).Put("$.store.book[0]", "new-key", "new-value")
            .Read("$.store.book[0].new-key");
        Assert.Equal("new-value", res);
    }

    [Fact]
    public void item_can_be_added_to_root_array()
    {
        var model = new List<object?>();
        model.Add(1);
        model.Add(2);

        var ints = JsonPath.Parse(model).Add("$", 3).Read("$").AsList();

        MyAssert.ContainsExactly(ints, 1, 2, 3);
    }

    [Fact]
    public void key_val_can_be_added_to_root_object()
    {
        var model = new Dictionary<string, object?>();
        model["a"] = "a-val";

        var newVal = JsonPath.Parse(model).Put("$", "new-key", "new-val").Read<string>("$.new-key");

        Assert.Equal("new-val", newVal);
    }

    [Fact]
    public void add_to_object_on_array()
    {
        Assert.Throws<InvalidModificationException>(() =>
            JsonPath.Parse(JsonTestData.JsonDocument).Put("$.store.book", "new-key", "new-value"));
    }

    [Fact]
    public void add_to_array_on_object()
    {
        Assert.Throws<InvalidModificationException>(() =>
            JsonPath.Parse(JsonTestData.JsonDocument).Add("$.store.book[0]", "new-value"));
    }


    [Fact]
    public void root_object_can_not_be_updated()
    {
        var model = new Dictionary<string, object?>();
        model["a"] = "a-val";

        Assert.Throws<InvalidModificationException>(() => JsonPath.Parse(model).Set("$[?(@.a == 'a-val')]", 1));
    }

    [Fact]
    public void a_path_can_be_renamed()
    {
        var o = JsonPath.Parse(JsonTestData.JsonDocument).RenameKey("$.store", "book", "updated-book").Json;
        var result = JsonPath.Parse(o).Read("$.store.updated-book").AsList();

        Assert.True(result.Any());
    }

    [Fact]
    public void keys_in_root_containing_map_can_be_renamed()
    {
        var o = JsonPath.Parse(JsonTestData.JsonDocument).RenameKey("$", "store", "new-store").Json;
        var result = JsonPath.Parse(o).Read("$.new-store[*]").AsList();
        Assert.True(result.Any());
    }

    [Fact]
    public void map_array_items_can_be_renamed()
    {
        var o = JsonPath.Parse(JsonTestData.JsonDocument).RenameKey("$.store.book[*]", "category", "renamed-category")
            .Json;
        var result = JsonPath.Parse(o).Read("$.store.book[*].renamed-category").AsList();
        Assert.True(result.Any());
    }

    [Fact]
    public void non_map_array_items_cannot_be_renamed()
    {
        var model = new List<int>();
        model.Add(1);
        model.Add(2);
        Assert.Throws<InvalidModificationException>(() => JsonPath.Parse(model).RenameKey("$[*]", "oldKey", "newKey"));
    }

    [Fact]
    public void multiple_properties_cannot_be_renamed()
    {
        Assert.Throws<InvalidModificationException>(() =>
            JsonPath.Parse(JsonTestData.JsonDocument)
                .RenameKey("$.store.book[*]['author', 'category']", "old-key", "new-key"));
    }

    [Fact]
    public void non_existent_key_rename_not_allowed()
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
    public void single_match_value_can_be_mapped()
    {
        var stringResult = JsonPath.Parse(JsonTestData.JsonDocument).Map("$.string-property", _toStringMapFunction)
            .Read<string>("$.string-property");
        Assert.True(stringResult.EndsWith("converted"));
    }

    [Theory]
    [ClassData(typeof(ProviderTypeTestCases))]
    public void object_can_be_mapped(IProviderTypeTestCase testCase)
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
    public void multi_match_path_can_be_mapped()
    {
        var doubleResult = JsonPath.Parse(JsonTestData.JsonDocument).Read("$..display-price").AsList();
        Assert.True(doubleResult.All(i => i is double));
        var stringResult = JsonPath.Parse(JsonTestData.JsonDocument).Map("$..display-price", _toStringMapFunction)
            .Read("$..display-price").AsList();

        Assert.True(stringResult.All(i => i is string));
        Assert.True(stringResult.All(i => i.ToString().EndsWith("converted")));
    }
}