using XavierJefferson.JsonPathParser.UnitTests.Extensions;

namespace XavierJefferson.JsonPathParser.UnitTests;

/**
 * test defined in http://jsonpath.googlecode.com/svn/trunk/tests/jsonpath-test-js.html
 */
public class ComplianceTest : TestUtils
{
    [Fact]
    public void test_one()
    {
        var json = "{ \"a\": \"a\",\n" +
                   "           \"b\": \"b\",\n" +
                   "           \"c d\": \"e\" \n" +
                   "         }";

        Assert.Equal("a", JsonPath.Read<string>(json, "$.a"));
        MyAssert.ContainsAll(JsonPath.Read(json, "$.*").AsList(), "a", "b", "e");
        MyAssert.ContainsAll(JsonPath.Read(json, "$[*]").AsList(), "a", "b", "e");
        Assert.Equal("a", JsonPath.Read<string>(json, "$['a']"));
        Assert.Equal("e", JsonPath.Read<string>(json, "$.['c d']"));
        MyAssert.ContainsAll(JsonPath.Read(json, "$[*]").AsList(), "a", "b", "e");
    }

    [Fact]
    public void test_two()
    {
        var json = "[ 1, \"2\", 3.14, true, null ]";

        Assert.Equal(1, JsonPath.Read<int>(json, "$[0]"));
        Assert.Null(JsonPath.Read(json, "$[4]"));
        MyAssert.ContainsAll(JsonPath.Read<List<object?>>(json, "$[*]"), 1d, "2", 3.14d, true, null);

        var res = JsonPath.Read(json, "$[-1:]").AsList();

        Assert.Null(res[0]);
    }

    [Fact]
    public void test_three()
    {
        var json = "{ \"points\": [\n" +
                   "             { \"id\": \"i1\", \"x\":  4, \"y\": -5 },\n" +
                   "             { \"id\": \"i2\", \"x\": -2, \"y\":  2, \"z\": 1 },\n" +
                   "             { \"id\": \"i3\", \"x\":  8, \"y\":  3 },\n" +
                   "             { \"id\": \"i4\", \"x\": -6, \"y\": -1 },\n" +
                   "             { \"id\": \"i5\", \"x\":  0, \"y\":  2, \"z\": 1 },\n" +
                   "             { \"id\": \"i6\", \"x\":  1, \"y\":  4 }\n" +
                   "           ]\n" +
                   "         }";
        var z = JsonPath.Read(json, "$.points[1]") as IDictionary<string, object?>;

        MyAssert.ContainsEntry(z, "id", "i2");
        MyAssert.ContainsEntry(z, "x", -2d);
        MyAssert.ContainsEntry(z, "y", 2d);
        MyAssert.ContainsEntry(z, "z", 1d);

        Assert.Equal(0, JsonPath.Read<int>(json, "$.points[4].x"));
        var y = JsonPath.Read(json, "$.points[?(@.id == 'i4')].x");
        MyAssert.ContainsAll(y.AsList(), -6d);
        MyAssert.ContainsAll(JsonPath.Read(json, "$.points[*].x").AsList(), 4d, -2d, 8d, -6d, 0d, 1d);
        MyAssert.ContainsAll(JsonPath.Read(json, "$.points[?(@.z)].id").AsList(), "i2", "i5");
    }

    [Fact]
    public void test_four()
    {
        var json = "{ \"menu\": {\n" +
                   "                 \"header\": \"SVG Viewer\",\n" +
                   "                 \"items\": [\n" +
                   "                     {\"id\": \"Open\"},\n" +
                   "                     {\"id\": \"OpenNew\", \"label\": \"Open New\"},\n" +
                   "                     null,\n" +
                   "                     {\"id\": \"ZoomIn\", \"label\": \"Zoom In\"},\n" +
                   "                     {\"id\": \"ZoomOut\", \"label\": \"Zoom Out\"},\n" +
                   "                     {\"id\": \"OriginalView\", \"label\": \"Original View\"},\n" +
                   "                     null,\n" +
                   "                     {\"id\": \"Quality\"},\n" +
                   "                     {\"id\": \"Pause\"},\n" +
                   "                     {\"id\": \"Mute\"},\n" +
                   "                     null,\n" +
                   "                     {\"id\": \"Find\", \"label\": \"Find...\"},\n" +
                   "                     {\"id\": \"FindAgain\", \"label\": \"Find Again\"},\n" +
                   "                     {\"id\": \"Copy\"},\n" +
                   "                     {\"id\": \"CopyAgain\", \"label\": \"Copy Again\"},\n" +
                   "                     {\"id\": \"CopySVG\", \"label\": \"Copy SVG\"},\n" +
                   "                     {\"id\": \"ViewSVG\", \"label\": \"View SVG\"},\n" +
                   "                     {\"id\": \"ViewSource\", \"label\": \"View Source\"},\n" +
                   "                     {\"id\": \"SaveAs\", \"label\": \"Save As\"},\n" +
                   "                     null,\n" +
                   "                     {\"id\": \"Help\"},\n" +
                   "                     {\"id\": \"About\", \"label\": \"About Adobe CVG Viewer...\"}\n" +
                   "                 ]\n" +
                   "               }\n" +
                   "             }";

        Assert.NotNull(JsonPath.Read(json, "$.menu.items[?(@)]"));
        MyAssert.ContainsAll(JsonPath.Read(json, "$.menu.items[?(@.id == 'ViewSVG')].id").AsList(), "ViewSVG");
        MyAssert.ContainsAll(JsonPath.Read(json, "$.menu.items[?(@ && @.id == 'ViewSVG')].id").AsList(), "ViewSVG");

        MyAssert.ContainsAll(JsonPath.Read(json, "$.menu.items[?(@ && @.id && !@.label)].id").AsList(), "Open",
            "Quality", "Pause", "Mute", "Copy", "Help"); //low
    }
}