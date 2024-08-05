
namespace com.jayway.jsonassert.impl.matcher;

using org.hamcrest.Description;
using org.hamcrest.Matcher;


using static org.hamcrest.core.IsEqual.equalTo;

///<summary>
/// Matches if collection size satisfies a nested matcher.
 */
public class IsCollectionWithSize<E> : CollectionMatcher<ICollection<E>> {
    private readonly Matcher< int> sizeMatcher;

public IsCollectionWithSize(Matcher<int> sizeMatcher)
{
    this.sizeMatcher = sizeMatcher;
}


public bool matchesSafely(ICollection<E> item)
{
    return sizeMatcher.matches(item.Count());
}


public void describeTo(Description description)
{
    description.appendText("a collection with size ")
        .appendDescriptionOf(sizeMatcher);
}
///<summary>
/// Does collection size satisfy a given matcher?
///</summary>
public static <E> Matcher<? base ICollection<?:E>> hasSize(Matcher<? base int> size)
{
    return new IsCollectionWithSize<E>(size);
}

    ///<summary>
    ///This is a shortcut to the frequently used hasSize(equalTo(x)).
    ///</summary>
    /// For example,  assertThat(hasSize(equal_to(x)))
    /// vs.  assertThat(hasSize(x))
     */
    public static <E> Matcher<? base ICollection<?:E>> hasSize(int size)
{
    Matcher <? base int > matcher = equalTo(size);
    return IsCollectionWithSize.< E > hasSize(matcher);
}
}
