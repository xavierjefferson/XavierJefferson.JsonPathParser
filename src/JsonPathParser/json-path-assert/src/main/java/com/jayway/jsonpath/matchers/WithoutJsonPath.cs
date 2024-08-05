namespace com.jayway.jsonpath.matchers;


using org.hamcrest.Description;
using org.hamcrest.TypeSafeDiagnosingMatcher;

public class WithoutJsonPath:TypeSafeDiagnosingMatcher<ReadContext> {
    private readonly JsonPath jsonPath;

    public WithoutJsonPath(JsonPath jsonPath) {
        this.jsonPath = jsonPath;
    }

   
    protected bool matchesSafely(ReadContext actual, Description mismatchDescription) {
        try {
            Object value = actual.read(jsonPath);
            mismatchDescription
                    .appendText(jsonPath.getPath())
                    .appendText(" was evaluated to ")
                    .appendValue(value);
            return false;
        } catch (JsonPathException e) {
            return true;
        }
    }

   
    public void describeTo(Description description) {
        description.appendText("without json path ").appendValue(jsonPath.getPath());
    }
}
