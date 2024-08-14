using XavierJefferson.JsonPathParser.UnitTests.TestData;

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


     

    [Theory]

    [ClassData(typeof(ProviderTypeTestCases))]
    public void TestLengthOfTextArray(IProviderTypeTestCase testCase)
    {
        // The length of JSONArray is an integer
        VerifyFunction(testCase.Configuration, "$['text'].length()", TextSeries, 6);
        VerifyFunction(testCase.Configuration, "$['text'].size()", TextSeries, 6);
    }

    [Theory]

    [ClassData(typeof(ProviderTypeTestCases))]
    public void TestLengthOfNumberArray(IProviderTypeTestCase testCase)
    {
        // The length of JSONArray is an integer
        VerifyFunction(testCase.Configuration, "$.numbers.length()", NumberSeries, 10);
        VerifyFunction(testCase.Configuration, "$.numbers.size()", NumberSeries, 10);
    }


    [Theory]

    [ClassData(typeof(ProviderTypeTestCases))]
    public void TestLengthOfStructure(IProviderTypeTestCase testCase)
    {
        VerifyFunction(testCase.Configuration, "$.batches.length()", BatchJson, 2);
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
    [Theory]

    [ClassData(typeof(ProviderTypeTestCases))]
    public void TestPredicateWithFunctionCallSingleMatch(IProviderTypeTestCase testCase)
    {
        var path = "$.batches.results[?(@.values.length() >= $.batches.minBatchSize)].values.avg()";

        // Its an array because in some use-cases the min size might match more than one batch and thus we'll get
        // the average out for each collection
        var values = new List<object?> { 12.2d };
        VerifyFunction(testCase.Configuration, path, BatchJson, values);
    }

    [Theory]

    [ClassData(typeof(ProviderTypeTestCases))]
    public void TestPredicateWithFunctionCallTwoMatches(IProviderTypeTestCase testCase)
    {
        var path = "$.batches.results[?(@.values.length() >= 3)].values.avg()";

        // Its an array because in some use-cases the min size might match more than one batch and thus we'll get
        // the average out for each collection
        var values = new List<object?>
        {
            12.2d,
            17d
        };
        VerifyFunction(testCase.Configuration, path, BatchJson, values);
    }
}