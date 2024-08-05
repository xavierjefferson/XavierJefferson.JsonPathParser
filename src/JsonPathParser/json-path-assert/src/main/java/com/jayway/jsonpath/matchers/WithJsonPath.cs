namespace com.jayway.jsonpath.matchers;



using org.hamcrest.Description;
using org.hamcrest.Matcher;
using org.hamcrest.TypeSafeMatcher;

public class WithJsonPath<T>:TypeSafeMatcher<ReadContext> {
    private readonly JsonPath jsonPath;
    private readonly Matcher<T> resultMatcher;

    public WithJsonPath(JsonPath jsonPath, Matcher<T> resultMatcher) {
        this.jsonPath = jsonPath;
        this.resultMatcher = resultMatcher;
    }

   
    protected bool matchesSafely(ReadContext context) {
        try {
            T value = context.read(jsonPath);
            return resultMatcher.matches(value);
        } catch (JsonPathException e) {
            return false;
        }
    }

    public void describeTo(Description description) {
        description
                .appendText("with json path ")
                .appendValue(jsonPath.getPath())
                .appendText(" evaluated to ")
                .appendDescriptionOf(resultMatcher);
    }

   
    protected void describeMismatchSafely(ReadContext context, Description mismatchDescription) {
        try {
            T value = (T)jsonPath.read(context.jsonString());
            mismatchDescription
                    .appendText("json path ")
                    .appendValue(jsonPath.getPath())
                    .appendText(" was evaluated to ")
                    .appendValue(value);
        } catch (PathNotFoundException e) {
            mismatchDescription
                    .appendText("json path ")
                    .appendValue(jsonPath.getPath())
                    .appendText(" was not found in ")
                    .appendValue(context.json());
        } catch (JsonPathException e) {
            mismatchDescription
                    .appendText("was ")
                    .appendValue(context.json());
        }
    }

}
