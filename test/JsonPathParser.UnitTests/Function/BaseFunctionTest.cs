namespace XavierJefferson.JsonPathParser.UnitTests.Function;

/**
 * Created by mattg on 6/27/15.
 */
public class BaseFunctionTest : TestUtils
{
    protected static string NumberSeries = "{\"empty\": [], \"numbers\" : [ 1, 2, 3, 4, 5, 6, 7, 8, 9, 10]}";

    protected static string TextSeries =
        "{\"urls\": [\"http://api.worldbank.org/countries/all/?format=json\", \"http://api.worldbank.org/countries/all/?format=json\"], \"text\" : [ \"a\", \"b\", \"c\", \"d\", \"e\", \"f\" ]}";

    protected static string TextAndNumberSeries =
        "{\"text\" : [ \"a\", \"b\", \"c\", \"d\", \"e\", \"f\" ], \"numbers\" : [ 1, 2, 3, 4, 5, 6, 7, 8, 9, 10]}";

    /**
     * Verify the function returns the correct result based on the input expectedValue
     * 
     * @param pathExpr
     * The path expression to execute
     * 
     * @param json
     * The json document (actual content) to parse
     * 
     * @param expectedValue
     * The expected value to be returned from the test
     */
    protected void VerifyFunction(Configuration conf, string pathExpr, string json, object expectedValue)
    {
        var result = JsonPath.Using(conf).Parse(json).Read(pathExpr);
        Assert.Equal(expectedValue, conf.JsonProvider.Unwrap(result));
    }

    protected void VerifyMathFunction(Configuration conf, string pathExpr, object expectedValue)
    {
        VerifyFunction(conf, pathExpr, NumberSeries, expectedValue);
    }

    protected void VerifyTextFunction(Configuration conf, string pathExpr, object expectedValue)
    {
        VerifyFunction(conf, pathExpr, TextSeries, expectedValue);
    }

    protected void VerifyTextAndNumberFunction(Configuration conf, string pathExpr, object expectedValue)
    {
        VerifyFunction(conf, pathExpr, TextAndNumberSeries, expectedValue);
    }

    protected string GetResourceAsText(string resourceName)
    {
        using (var streamReader = new StreamReader(GetResourceAsStream(resourceName)))
        {
            var a = streamReader.ReadToEnd();
            var b = a.Split("\\A");
            return b[0];
        }
    }
}