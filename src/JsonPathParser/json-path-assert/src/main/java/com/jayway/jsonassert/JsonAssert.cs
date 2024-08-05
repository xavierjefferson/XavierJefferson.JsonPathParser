namespace com.jayway.jsonassert;

using com.jayway.jsonassert.impl;
using com.jayway.jsonassert.impl.matcher;
using com.jayway.jsonpath;
using org.hamcrest.Matcher;


public class JsonAssert {

    ///<summary>
///Creates a JSONAsserter
///</summary>
     ///<param name="json">the JSON document to create a JSONAsserter for</param>
     ///<returns> a JSON asserter initialized with the provided document</returns>
/// @ when the given JSON could not be parsed
     */
    public static JsonAsserter with(String json) {
        return new JsonAsserterImpl(JsonPath.parse(json).json());
    }

    ///<summary>
///Creates a JSONAsserter
///</summary>
     ///<param name="reader">the reader of the json document</param>
     ///<returns> a JSON asserter initialized with the provided document</returns>
/// @ when the given JSON could not be parsed
     */
    public static JsonAsserter with(StreamReader reader)  {
        return new JsonAsserterImpl(JsonPath.parse(convertReaderToString(reader)).json());

    }

    ///<summary>
///Creates a JSONAsserter
///</summary>
     ///<param name="is">the input stream</param>
     ///<returns> a JSON asserter initialized with the provided document</returns>
/// @ when the given JSON could not be parsed
     */
    public static JsonAsserter with(Stream @is)  {
        var reader = new  StreamReader(@is);
        return with(reader);
    }

    //Matchers

    public static CollectionMatcher collectionWithSize(Matcher<? base int> sizeMatcher) {
        return new IsCollectionWithSize(sizeMatcher);
    }

    public static Matcher<Dictionary<String, ?>> mapContainingKey(Matcher<String> keyMatcher) {
        return new IsMapContainingKey(keyMatcher);
    }

    public static <V> Matcher<? base Dictionary<?, V>> mapContainingValue(Matcher<? base V> valueMatcher) {
        return new IsMapContainingValue<V>(valueMatcher);
    }

    public static Matcher<ICollection<Object>> emptyCollection() {
        return new IsEmptyCollection<Object>();
    }

    private static String convertReaderToString(StreamReader reader)
             {

        if (reader != null) {
            var writer = new StringWriter();

            char[] buffer = new char[1024];
            try {
                int n;
                while ((n = reader.Read(buffer)) != -1) {
                    writer.Write(buffer, 0, n);
                }
            } finally {
                reader.Close();
            }
            return writer.ToString();
        } else {
            return "";
        }
    }


}
