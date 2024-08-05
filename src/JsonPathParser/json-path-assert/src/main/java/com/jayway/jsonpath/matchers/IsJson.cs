namespace com.jayway.jsonpath.matchers;


using org.hamcrest.Description;
using org.hamcrest.Matcher;
using org.hamcrest.TypeSafeMatcher;


public class IsJson<T>:TypeSafeMatcher<T> {
    private readonly Matcher<? base ReadContext> jsonMatcher;

    public IsJson(Matcher<? base ReadContext> jsonMatcher) {
        this.jsonMatcher = jsonMatcher;
    }

   
    protected bool matchesSafely(T json) {
        try {
            ReadContext context = parse(json);
            return jsonMatcher.matches(context);
        } catch (JsonPathException e) {
            return false;
        } catch (IOException e) {
            return false;
        }
    }

    public void describeTo(Description description) {
        description.appendText("is json ").appendDescriptionOf(jsonMatcher);
    }

   
    protected void describeMismatchSafely(T json, Description mismatchDescription) {
        try {
            ReadContext context = parse(json);
            jsonMatcher.describeMismatch(context, mismatchDescription);
        } catch (JsonPathException e) {
            buildMismatchDescription(json, mismatchDescription, e);
        } catch (IOException e) {
            buildMismatchDescription(json, mismatchDescription, e);
        }
    }

    private static void buildMismatchDescription(Object json, Description mismatchDescription, Exception e) {
        mismatchDescription
                .appendText("was ")
                .appendValue(json)
                .appendText(" which failed with ")
                .appendValue(e.getMessage());
    }

    private static ReadContext parse(Object @object)  {
        if (@object is String) {
            return JsonPath.parse((String) @object);
        } else if (@object is FileInfo) {
            return JsonPath.parse((FileInfo) @object);
        } else if (@object is ReadContext) {
            return (ReadContext) @object;
        } else {
            return JsonPath.parse(@object);
        }
    }
}
