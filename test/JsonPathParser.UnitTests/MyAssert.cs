using XavierJefferson.JsonPathParser.UnitTests.Extensions;
using Xunit.Sdk;

namespace XavierJefferson.JsonPathParser.UnitTests;

public class MyAssert : Assert
{
    public static void ContainsOnly<T>(IEnumerable<T> actual, params T[] toFind)
    {
        if (!actual.ContainsOnly(toFind)) throw FailException.ForFailure("");
    }

    public static void ContainsAll<T>(IEnumerable<T> actual, params T[] toFind)
    {
        if (!actual.ContainsAll(toFind)) throw FailException.ForFailure("");
    }

    public static void ContainsExactly<T>(IEnumerable<T> actual, params T[] toFind)
    {
        if (!actual.ContainsExactly(toFind)) throw FailException.ForFailure("");
    }

    public static void ContainsEntry<T, TU>(IDictionary<T, TU> dictionary, T key, TU value)
    {
        if (!dictionary.ContainsEntry(key, value)) throw FailException.ForFailure("");
    }

    internal static void ContainsKey<T, U>(IDictionary<T, U> result, T key)
    {
        if (!result.ContainsKey(key)) throw FailException.ForFailure("");
    }

    /// <summary>Shortcut for counting found nodes.</summary>
    /// <param name="json">json to be parsed</param>
    /// <param name="path">path to be evaluated</param>
    /// <param name="expectedResultCount">expected number of nodes to be found</param>
    /// <param name="conf">conf to use during evaluation</param>
    public static void HasResults(string json, string path, int expectedResultCount, Configuration conf)
    {
        var result = JsonPath.Using(conf).Parse(json).Read(path);
        Equal(expectedResultCount, conf.JsonProvider.Length(result));
    }

    /// <summary>Assertion which requires list of one element as a result of indefinite path search.</summary>
    /// <param name="json">json to be parsed</param>
    /// <param name="path">path to be evaluated</param>
    public static void HasOneResult(string json, string path, Configuration conf)
    {
        HasResults(json, path, 1, conf);
    }

    /// @param json json to be parsed
    /// @param path path to be evaluated
    /// @param conf conf to use during evaluation
    /// <summary>Assertion which requires empty list as a result of indefinite path search.</summary>
    /// <param name="json">json to be parsed</param>
    /// <param name="path">path to be evaluated</param>
    /// <param name="conf">conf to use during evaluation</param>
    public static void HasNoResults(string json, string path, Configuration conf)
    {
        HasResults(json, path, 0, conf);
    }

    /// <summary>Shortcut for expected exception testing during path evaluation.</summary>
    /// <typeparam name="T">The type of the exception expected to be thrown</typeparam>
    /// <param name="json">json to parse</param>
    /// <param name="path">jsonpath to evaluate</param>
    /// <param name="conf">conf to use during evaluation</param>
    public static void EvaluationThrows<T>(string json, string path,
        Configuration conf) where T : Exception
    {
        Throws<T>(() => { JsonPath.Using(conf).Parse(json).Read(path); });
    }
}