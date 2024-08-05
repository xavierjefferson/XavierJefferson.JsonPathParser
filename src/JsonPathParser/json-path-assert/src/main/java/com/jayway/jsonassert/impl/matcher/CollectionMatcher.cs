
namespace com.jayway.jsonassert.impl.matcher;

using org.hamcrest.BaseMatcher;


public abstract class CollectionMatcher<C> : BaseMatcher<C> where C : ICollection<object>
{
    public bool matches(Object item)
    {
        if (!(item is ICollection))
        {
            return false;
        }
        return matchesSafely((C)item);
    }

    protected abstract bool matchesSafely(C collection);
}