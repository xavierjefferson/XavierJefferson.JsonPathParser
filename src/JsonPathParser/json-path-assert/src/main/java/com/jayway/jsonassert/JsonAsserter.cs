namespace com.jayway.jsonassert;

using org.hamcrest.Matcher;

public interface JsonAsserter {
///<summary>
/// Asserts that object specified by path satisfies the condition specified by matcher.
/// If not, an AssertionError is thrown with information about the matcher
/// and failing value. Example:
/// <p/>
/// <code>
/// with(json).assertThat("items[0].name", equalTo("Bobby"))
/// .assertThat("items[0].age" , equalTo(24L))
/// </code>
///</summary>
///<param name="path">the json path specifying the value being compared</param>
     ///<param name="matcher">an expression, built of Matchers, specifying allowed values</param>
     ///<param name="<T>">the static type accepted by the matcher</param>
     ///<returns> this to allow fluent assertion chains</returns>
    <T> JsonAsserter assertThat(String path, Matcher<T> matcher);
///<summary>
     ///<param name="path">the json path specifying the value being compared</param>
     ///<param name="matcher">an expression, built of Matchers, specifying allowed values</param>
     ///<param name="message">the explanation message</param>
     ///<param name="<T>">the static type that should be returned by the path</param>
     ///<returns> this to allow fluent assertion chains</returns>
    <T> JsonAsserter assertThat(String path, Matcher<T> matcher, String message);
///<summary>
/// Asserts that object specified by path is equal to the expected value.
/// If they are not, an AssertionError is thrown with the given message.
///</summary>
///<param name="path">the json path specifying the value being compared</param>
     ///<param name="expected">the expected value</param>
     ///<param name="<T>">the static type that should be returned by the path</param>
     ///<returns> this to allow fluent assertion chains</returns>
    <T> JsonAsserter assertEquals(String path, T expected);

    <T> JsonAsserter assertEquals(String path, T expected, String message);
///<summary>
/// Checks that a path is not defined within a document. If the document contains the
/// given path, an AssertionError is thrown
///</summary>
///<param name="path">the path to make sure not exists</param>
     ///<returns> this</returns>
    JsonAsserter assertNotDefined(String path);

    JsonAsserter assertNotDefined(String path, String message);
///<summary>
/// Asserts that object specified by path is null. If it is not, an AssertionError
/// is thrown with the given message.
///</summary>
///<param name="path">the json path specifying the value that should be null</param>
     ///<returns> this to allow fluent assertion chains</returns>
    JsonAsserter assertNull(String path);
    JsonAsserter assertNull(String path, String message);
///<summary>
/// Asserts that object specified by path is NOT null. If it is, an AssertionError
/// is thrown with the given message.
///</summary>
///<param name="path">the json path specifying the value that should be NOT null</param>
     ///<returns> this to allow fluent assertion chains</returns>
    <T> JsonAsserter assertNotNull(String path);

    <T> JsonAsserter assertNotNull(String path, String message);
///<summary>
/// Syntactic sugar to allow chaining assertions with a separating and() statement
/// <p/>
/// <p/>
/// <code>
/// with(json).assertThat("firstName", is(equalTo("Bobby"))).and().assertThat("lastName", is(equalTo("Ewing")))
/// </code>
///</summary>
///<returns> this to allow fluent assertion chains</returns>
    JsonAsserter and();
}
