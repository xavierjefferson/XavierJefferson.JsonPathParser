 
namespace com.jayway.jsonassert.impl.matcher;

using org.hamcrest.BaseMatcher;


public abstract class MapTypeSafeMatcher<M:Dictionary<?, ?>>:BaseMatcher<M> {
        public bool matches(Object item) {
        return item is Dictionary && matchesSafely((M) item);
    }

    protected abstract bool matchesSafely(M map);
}