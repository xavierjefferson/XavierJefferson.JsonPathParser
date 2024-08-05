namespace XavierJefferson.JsonPathParser.UnitTests.Function;

/**
 * Verifies methods that are helper implementations of functions for manipulating JSON entities, i.e.
 * length, etc.
 * 
 * Created by mattg on 6/27/15.
 */
public class JsonEntityPathFunctionTest : BaseFunctionTest
{
    private static readonly string BatchJson = "{\n" +
                                               "  \"batches\": {\n" +
                                               "    \"minBatchSize\": 10,\n" +
                                               "    \"results\": [\n" +
                                               "      {\n" +
                                               "        \"productId\": 23,\n" +
                                               "        \"values\": [\n" +
                                               "          2,\n" +
                                               "          45,\n" +
                                               "          34,\n" +
                                               "          23,\n" +
                                               "          3,\n" +
                                               "          5,\n" +
                                               "          4,\n" +
                                               "          3,\n" +
                                               "          2,\n" +
                                               "          1,\n" +
                                               "        ]\n" +
                                               "      },\n" +
                                               "      {\n" +
                                               "        \"productId\": 23,\n" +
                                               "        \"values\": [\n" +
                                               "          52,\n" +
                                               "          3,\n" +
                                               "          12,\n" +
                                               "          11,\n" +
                                               "          18,\n" +
                                               "          22,\n" +
                                               "          1\n" +
                                               "        ]\n" +
                                               "      }\n" +
                                               "    ]\n" +
                                               "  }\n" +
                                               "}";


    private readonly Configuration _conf = ConfigurationData.NewtonsoftJsonConfiguration;

    [Fact]
    public void TestLengthOfTextArray()
    {
        // The length of JSONArray is an integer
        VerifyFunction(_conf, "$['text'].length()", TextSeries, 6);
        VerifyFunction(_conf, "$['text'].size()", TextSeries, 6);
    }

    [Fact]
    public void TestLengthOfNumberArray()
    {
        // The length of JSONArray is an integer
        VerifyFunction(_conf, "$.numbers.length()", NumberSeries, 10);
        VerifyFunction(_conf, "$.numbers.size()", NumberSeries, 10);
    }


    [Fact]
    public void TestLengthOfStructure()
    {
        VerifyFunction(_conf, "$.batches.length()", BatchJson, 2);
    }

    /**
     * The fictitious use-case/story - is we have a collection of batches with values indicating some quality metric.
     * We want to determine the average of the values for only the batch's values where the number of items in the batch
     * is greater than the min batch size which is encoded in the JSON document.
     * 
     * We use the length function in the predicate to determine the number of values in each batch and then for those
     * batches where the count is greater than min we calculate the average batch value.
     * 
     * Its completely contrived example, however, this test exercises functions within predicates.
     */
    [Fact]
    public void TestPredicateWithFunctionCallSingleMatch()
    {
        var path = "$.batches.results[?(@.values.length() >= $.batches.minBatchSize)].values.avg()";

        // Its an array because in some use-cases the min size might match more than one batch and thus we'll get
        // the average out for each collection
        var values = new JpObjectList { 12.2d };
        VerifyFunction(_conf, path, BatchJson, values);
    }

    [Fact]
    public void TestPredicateWithFunctionCallTwoMatches()
    {
        var path = "$.batches.results[?(@.values.length() >= 3)].values.avg()";

        // Its an array because in some use-cases the min size might match more than one batch and thus we'll get
        // the average out for each collection
        var values = new JpObjectList
        {
            12.2d,
            17d
        };
        VerifyFunction(_conf, path, BatchJson, values);
    }
}