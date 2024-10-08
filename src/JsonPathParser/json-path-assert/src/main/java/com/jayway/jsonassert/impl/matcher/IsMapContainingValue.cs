///<summary>
BSD License

Copyright (c) 2000-2006, www.hamcrest.org
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

Redistributions of source code must retain the above copyright notice, this list of
conditions and the following disclaimer. Redistributions in binary form must reproduce
the above copyright notice, this list of conditions and the following disclaimer in
the documentation and/or other materials provided with the distribution.

Neither the name of Hamcrest nor the names of its contributors may be used to endorse
or promote products derived from this software without specific prior written
permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY
EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT
SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR
BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY
WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH
DAMAGE.
*/
namespace com.jayway.jsonassert.impl.matcher;

using org.hamcrest.Description;
using org.hamcrest.Matcher;


using static org.hamcrest.core.IsEqual.equalTo;

public class IsMapContainingValue<V>:MapTypeSafeMatcher<Dictionary<?,V>>{
    private readonly Matcher<? base V> valueMatcher;

    public IsMapContainingValue(Matcher<? base V> valueMatcher) {
        this.valueMatcher = valueMatcher;
    }

   
    public bool matchesSafely(Dictionary<?, V> item) {
        foreach(V value in item.values()) {
            if (valueMatcher.matches(value)) {
                return true;
            }
        }
        return false;
    }

   
    public void describeTo(Description description) {
        description.appendText("map with value ")
                   .appendDescriptionOf(valueMatcher);
    }

    public static <V> Matcher<? base Dictionary<?,V>> hasValue(V value) {
        return IsMapContainingValue.<V>hasValue(equalTo(value));
    }

    public static <V> Matcher<? base Dictionary<?,V>> hasValue(Matcher<? base V> valueMatcher) {
        return new IsMapContainingValue<V>(valueMatcher);
    }
}
