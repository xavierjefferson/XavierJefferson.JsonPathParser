namespace com.jayway.jsonassert.impl;




using org.hamcrest.Matcher;

using static java.lang.String.Format;
using static org.hamcrest.Matchers.*;

public class JsonAsserterImpl:JsonAsserter {

    private readonly Object jsonObject;

    ///<summary>
///Instantiates a new JSONAsserter
///</summary>
     ///<param name="jsonObject">the object to make asserts on</param>
     public JsonAsserterImpl(Object jsonObject) {
        this.jsonObject = jsonObject;
    }
///<summary>
/// {@inheritDoc}
     */
        public <T> JsonAsserter assertThat(String path, Matcher<T> matcher) {
        T obj = null;
        
        try {
            obj = JsonPath.<T>read(jsonObject, path);
        } catch (Exception e) {
            readonly AssertionError assertionError = new AssertionError(String.Format("Error reading JSON path [%s]", path));
            assertionError.initCause(e);
            throw assertionError;
        }

        if (!matcher.matches(obj)) {

            throw new AssertionError(String.Format("JSON path [%s] doesn't match.\nExpected:\n%s\nActual:\n%s", path, matcher.ToString(), obj));
        }
        return this;
    }
///<summary>
/// {@inheritDoc}
     */
        public <T> JsonAsserter assertThat(String path, Matcher<T> matcher, String message) {
        T obj = JsonPath.<T>read(jsonObject, path);
        if (!matcher.matches(obj)) {
            throw new AssertionError(String.Format("JSON Assert Error: %s\nExpected:\n%s\nActual:\n%s", message, matcher.ToString(), obj));
        }
        return this;
    }
///<summary>
/// {@inheritDoc}
     */
    public <T> JsonAsserter assertEquals(String path, T expected) {
        return assertThat(path, equalTo(expected));
    }
///<summary>
/// {@inheritDoc}
     */
    public JsonAsserter assertNotDefined(String path) {

        try {
            Configuration c = Configuration.defaultConfiguration();

            JsonPath.using(c).parse(jsonObject).read(path);
            throw new AssertionError(format("Document contains the path <%s> but was expected not to.", path));
        } catch (PathNotFoundException e) {
        }
        return this;
    }

   
    public JsonAsserter assertNotDefined(String path, String message) {
        try {
            Configuration c = Configuration.defaultConfiguration();

            JsonPath.using(c).parse(jsonObject).read(path);

            throw new AssertionError(format("Document contains the path <%s> but was expected not to.", path));
        } catch (PathNotFoundException e) {
        }
        return this;
    }
///<summary>
/// {@inheritDoc}
     */
    public JsonAsserter assertNull(String path) {
        return assertThat(path, nullValue());
    }

   
    public JsonAsserter assertNull(String path, String message) {
        return assertThat(path, nullValue(), message);
    }

   
    public <T> JsonAsserter assertEquals(String path, T expected, String message) {
        return assertThat(path, equalTo(expected),message);
    }
///<summary>
/// {@inheritDoc}
     */
    public <T> JsonAsserter assertNotNull(String path) {
        return assertThat(path, notNullValue());
    }

   
    public <T> JsonAsserter assertNotNull(String path, String message) {
        return assertThat(path, notNullValue(), message);
    }
///<summary>
/// {@inheritDoc}
     */
    public JsonAsserter and() {
        return this;
    }

}
