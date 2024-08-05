namespace com.jayway.jsonpath.matchers;

using org.hamcrest.Matcher;


using static org.hamcrest.Matchers.*;

public class JsonPathMatchers {

    private JsonPathMatchers() {
        throw new AssertionError("prevent instantiation");
    }

    public static Matcher<T> hasJsonPath<T>(String jsonPath) where T:class {
        return describedAs("has json path %0",
                isJson(withJsonPath(jsonPath)),
                jsonPath);
    }

    public static Matcher<T> hasJsonPath<T>(String jsonPath, Matcher<T> resultMatcher) where T:class {
        return isJson(withJsonPath(jsonPath, resultMatcher));
    }

    public static Matcher<T> hasNoJsonPath<T>(String jsonPath) where T : class
    {
        return isJson(withoutJsonPath(jsonPath));
    }

    public static Matcher<Object> isJson() {
        return isJson(withJsonPath("$", anyOf(instanceOf(typeof(Dictionary)), instanceOf(typeof(List)))));
    }

    public static Matcher<Object> isJson<T, U>(Matcher<U> matcher) where U:ReadContext {
        return new IsJson<Object>(matcher);
    }

    public static Matcher<String> isJsonString<T>(Matcher<T> matcher) where T:ReadContext {
        return new IsJson<String>(matcher);
    }

    public static Matcher<FileInfo> isJsonFile(Matcher<? base ReadContext> matcher) {
        return new IsJson<FileInfo>(matcher);
    }

    public static Matcher<? base ReadContext> withJsonPath(String jsonPath, params Predicate[]  filters) {
        return withJsonPath(JsonPath.compile(jsonPath, filters));
    }

    public static Matcher<? base ReadContext> withJsonPath(JsonPath jsonPath) {
        return describedAs("with json path %0",
                withJsonPath(jsonPath, anything()),
                jsonPath.getPath());
    }

    public static Matcher<? base ReadContext> withoutJsonPath(String jsonPath, params Predicate[]  filters) {
        return withoutJsonPath(JsonPath.compile(jsonPath, filters));
    }

    public static Matcher<? base ReadContext> withoutJsonPath(JsonPath jsonPath) {
        return new WithoutJsonPath(jsonPath);
    }

    public static <T> Matcher<? base ReadContext> withJsonPath(String jsonPath, Matcher<T> resultMatcher) {
        return withJsonPath(JsonPath.compile(jsonPath), resultMatcher);
    }

    public static <T> Matcher<? base ReadContext> withJsonPath(JsonPath jsonPath, Matcher<T> resultMatcher) {
        return new WithJsonPath<T>(jsonPath, resultMatcher);
    }
}
