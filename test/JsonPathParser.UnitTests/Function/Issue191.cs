using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using XavierJefferson.JsonPathParser.UnitTests.Internal;

namespace XavierJefferson.JsonPathParser.UnitTests.Function;

/**
 * TDD for Issue 191
 * <p>
 *     Shows aggregation across fields rather than within a single entity.
 */
public class Issue191 : TestBase
{
    private static readonly JObject Jobject1;


    static Issue191()
    {
        using (var streamReader = new StreamReader(GetResourceAsStream("issue_191.json")))
        {
            Jobject1 = JsonConvert.DeserializeObject<JObject>(streamReader.ReadToEnd());
        }
    }

    private List<JToken> FindAll(string name, JObject jObject)
    {
        var result = new List<JToken>();
        foreach (var jProperty in jObject.Properties())
        {
            if (!jObject.TryGetValue(jProperty.Name, out var jToken)) continue;
            if (jProperty.Name == name)
                result.Add(jToken);
            else
                switch (jToken)
                {
                    case JArray jArray:
                    {
                        foreach (var jObject2 in jArray.OfType<JObject>())
                            result.AddRange(FindAll(name, jObject2));
                        break;
                    }
                    case JToken jToken2 when jToken2.Type == JTokenType.Object:
                        result.AddRange(FindAll(name, jToken2.Value<JObject>()));
                        break;
                }
        }

        return result;
    }

    [Fact]
    public void TestResultSetNumericComputation()
    {
        var foundTokens = FindAll("timestamp", Jobject1);
        var sum = foundTokens.Sum(i => i.Value<double>());
        using (var stream = GetResourceAsStream("issue_191.json"))
        {
            object value = JsonPath.Parse(stream).Read<double>("$.sum($..timestamp)");
            Assert.Equal(sum,
                value); // "Expected the max function to consume the aggregation parameters and calculate the max over the result set");
            //Assert.Equal(Convert.ToInt64(sum), value);
        }
    }

    [Fact]
    public void TestResultSetNumericComputationTail()
    {
        using (var stream = GetResourceAsStream("issue_191.json"))
        {
            var value = JsonPath.Parse(stream).Read<long>("$..timestamp.sum()");
            Assert.Equal(35679716813L,
                value); // "Expected the max function to consume the aggregation parameters and calculate the max over the result set");
        }
    }

    [Fact]
    public void TestResultSetNumericComputationRecursiveReplacement()
    {
        using (var stream = GetResourceAsStream("issue_191.json"))
        {
            var documentContext = JsonPath.Parse(stream);
            var value = documentContext.Read<long>("$.max($..timestamp.avg(), $..timestamp.stddev())");


            Assert.Equal(1427188673L,
                value); // "Expected the max function to consume the aggregation parameters and calculate the max over the result set");
        }
    }

    [Fact]
    public void TestMultipleResultSetSums()
    {
        using (var stream = GetResourceAsStream("issue_191.json"))
        {
            var value = JsonPath.Parse(stream).Read<long>("$.sum($..timestamp, $..cpus)");
            Assert.Equal(35679716835L, value); //
        }
    }

    [Fact]
    public void TestConcatResultSet()
    {
        var foundTokens = FindAll("state", Jobject1);
        var length = foundTokens.Sum(i => i.Value<string>().Length);
        using (var stream = GetResourceAsStream("issue_191.json"))
        {
            var concatResult = JsonPath.Parse(stream).Read<string>("$.concat($..state)");
            Assert.Equal(length,
                concatResult.Length); // "Expected a string length to be a concat of all of the states");
        }
    }

    [Fact]
    public void TestConcatWithNumericValueAsString()
    {
        var foundTokens = FindAll("cpus", Jobject1);
        var length = foundTokens.Sum(i => i.Value<double>().ToString().Length);
        using (var stream = GetResourceAsStream("issue_191.json"))
        {
            var concatResult = JsonPath.Parse(stream).Read<string>("$.concat($..cpus)");
            Assert.Equal(length, concatResult.Length); // "Expected a string length to be a concat of all of the cpus");
        }
    }
}