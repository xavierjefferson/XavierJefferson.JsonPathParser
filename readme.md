
Xavier Jefferson's JsonPathParser
===========================

**A .NET domain specific language for reading JSON documents.**

[![Build Status](https://travis-ci.org/json-path/JsonPath.svg?branch=master)](https://travis-ci.org/json-path/JsonPath)
[![Maven Central](https://maven-badges.herokuapp.com/maven-central/com.jayway.jsonpath/json-path/badge.svg)](https://maven-badges.herokuapp.com/maven-central/com.jayway.jsonpath/json-path)
[![Javadoc](https://www.javadoc.io/badge/com.jayway.jsonpath/json-path.svg)](http://www.javadoc.io/doc/com.jayway.jsonpath/json-path)

This library is a .NET port of [Jayway JsonPath](https://github.com/json-path/JsonPath), which is a Java port of [Stefan Goessner JsonPath implementation](http://goessner.net/articles/JsonPath/). 

Getting Started
---------------

This library is available at the Central Maven Repository. Maven users add this to your POM.

```xml
<dependency>
    <groupId>com.jayway.jsonpath</groupId>
    <artifactId>json-path</artifactId>
    <version>2.9.0</version>
</dependency>
```

If you need help ask questions at [Stack Overflow](http://stackoverflow.com/questions/tagged/jsonpath). Tag the question 'jsonpath' and 'C#'.

JsonPath expressions always refer to a JSON structure in the same way as XPath expression are used in combination 
with an XML document. The "root member object?" in JsonPath is always referred to as `$` regardless if it is an 
object? or array.

JsonPath expressions can use the dot–notation

`$.store.book[0].title`

or the bracket–notation

`$['store']['book'][0]['title']`

Operators
---------

| Operator                  | Description                                                        |
| :------------------------ | :----------------------------------------------------------------- |
| `$`                       | The root element to query. This starts all path expressions.       |
| `@`                       | The current node being processed by a filter predicate.            |
| `*`                       | Wildcard. Available anywhere a name or numeric are required.       |
| `..`                      | Deep scan. Available anywhere a name is required.                  |
| `.<name>`                 | Dot-notated child                                                  |
| `['<name>' (, '<name>')]` | Bracket-notated child or children                                  |
| `[<number> (, <number>)]` | Array index or indexes                                             |
| `[start:end]`             | Array slice operator                                               |
| `[?(<expression>)]`       | Filter expression. Expression must evaluate to a boolean value.    |


Functions
---------

Functions can be invoked at the tail end of a path - the input to a function is the output of the path expression.
The function output is dictated by the function itself.

| Function    | Description                                                                          | Output type          |
|:------------|:-------------------------------------------------------------------------------------|:---------------------|
| `min()`     | Provides the min value of an array of numbers                                        | Double               |
| `max()`     | Provides the max value of an array of numbers                                        | Double               |
| `avg()`     | Provides the average value of an array of numbers                                    | Double               | 
| `stddev()`  | Provides the standard deviation value of an array of numbers                         | Double               | 
| `length()`  | Provides the length of an array                                                      | Integer              |
| `sum()`     | Provides the sum value of an array of numbers                                        | Double               |
| `keys()`    | Provides the property keys (An alternative for terminal tilde `~`)                   | `Set<E>`             |
| `concat(X)` | Provides a concatenated version of the path output with a new item                   | like input           |
| `append(X)` | add an item to the json path output array                                            | like input           |
| `first()`   | Provides the first item of an array                                                  | Depends on the array |
| `last()`    | Provides the last item of an array                                                   | Depends on the array |
| `index(X)`  | Provides the item of an array of index: X, if the X is negative, take from backwards | Depends on the array |

Filter Operators
-----------------

Filters are logical expressions used to filter arrays. A typical filter would be `[?(@.age > 18)]` where `@` represents the current item being processed. More complex filters can be created with logical operators `&&` and `||`. string literals must be enclosed by single or double quotes (`[?(@.color == 'blue')]` or `[?(@.color == "blue")]`).   

| Operator                 | Description                                                           |
| :----------------------- | :-------------------------------------------------------------------- |
| `==`                     | left is equal to right (note that 1 is not equal to '1')              |
| `!=`                     | left is not equal to right                                            |
| `<`                      | left is less than right                                               |
| `<=`                     | left is less or equal to right                                        |
| `>`                      | left is greater than right                                            |
| `>=`                     | left is greater than or equal to right                                |
| `=~`                     | left matches regular expression  [?(@.name =~ /foo.*?/i)]             |
| `in`                     | left exists in right [?(@.size in ['S', 'M'])]                        |
| `nin`                    | left does not exists in right                                         |
| `subsetof`               | left is a subset of right [?(@.sizes subsetof ['S', 'M', 'L'])]       |
| `anyof`                  | left has an intersection with right [?(@.sizes anyof ['M', 'L'])]     |
| `noneof`                 | left has no intersection with right [?(@.sizes noneof ['M', 'L'])]    |
| `size`                   | size of left (array or string) should match right                     |
| `empty`                  | left (array or string) should be empty                                |


Path Examples
-------------

Given the json

```javascript
{
    "store": {
        "book": [
            {
                "category": "reference",
                "author": "Nigel Rees",
                "title": "Sayings of the Century",
                "price": 8.95
            },
            {
                "category": "fiction",
                "author": "Evelyn Waugh",
                "title": "Sword of Honour",
                "price": 12.99
            },
            {
                "category": "fiction",
                "author": "Herman Melville",
                "title": "Moby Dick",
                "isbn": "0-553-21311-3",
                "price": 8.99
            },
            {
                "category": "fiction",
                "author": "J. R. R. Tolkien",
                "title": "The Lord of the Rings",
                "isbn": "0-395-19395-8",
                "price": 22.99
            }
        ],
        "bicycle": {
            "color": "red",
            "price": 19.95
        }
    },
    "expensive": 10
}
```

| JsonPath                                       | Result |
|:-------------------------------------------------------------------| :----- |
| `$.store.book[*].author` | The authors of all books     |
| `$..author`                           | All authors                         |
| `$.store.*`                           | All things, both books and bicycles  |
| `$.store..price`                 | The price of everything         |
| `$..book[2]`                         | The third book                      |
| `$..book[-2]`                       | The second to last book            |
| `$..book[0,1]`                     | The first two books               |
| `$..book[:2]`                       | All books from index 0 (inclusive) until index 2 (exclusive) |
| `$..book[1:2]`                     | All books from index 1 (inclusive) until index 2 (exclusive) |
| `$..book[-2:]`                     | Last two books                   |
| `$..book[2:]`                     | All books from index 2 (inclusive) to last  |
| `$..book[?(@.isbn)]`                                                 | All books with an ISBN number         |
| `$.store.book[?(@.price < 10)]`                                      | All books in store cheaper than 10  |
| `$..book[?(@.price <= $['expensive'])]`                              | All books in store that are not "expensive"  |
| `$..book[?(@.author =~ /.*REES/i)]`                                  | All books matching regex (ignore case)  |
| `$..*`                                                               | Give me every thing   
| `$..book.length()`                                                   | The number of books                      |

Reading a Document
------------------
The simplest most straight forward way to use JsonPath is via the static read API, using the Read method with generic arguments:

```C#
string json = "...";

var authors = JsonPath.Read<List<string>>(json, "$.store.book[*].author");
```

If you only want to read once this is OK. In case you need to read an other path as well this is not the way 
to go since the document will be parsed every time you call JsonPath.Read(...). To avoid the problem you can 
parse the json first.

```C#
string json = "...";
var document = Configuration.DefaultConfiguration.JsonProvider.Parse(json);

var author0 = JsonPath.Read<string>(document, "$.store.book[0].author");
var author1 = JsonPath.Read<string>(document, "$.store.book[1].author");
```
JsonPath also provides a fluent API. This is also the most flexible one.

```C#
string json = "...";

ReadContext ctx = JsonPath.Parse(json);

List<string> authorsOfBooksWithISBN = ctx.Read<List<string>>("$.store.book[?(@.isbn)].author");


List<Dictionary<string, object??>> expensiveBooks = JsonPath
                            .Using(configuration)
                            .Parse(json)
                            .Read<List<Dictionary<string, object??>>("$.store.book[?(@.price > 10)]");
```

What is Returned When?
----------------------
When using JsonPath in .NET, it's important to know what type you expect in your result. JsonPath will automatically 
try to cast the result to the type expected by the invoker.

```C#
//Will throw an C#.lang.ClassCastException    
List<string> list = JsonPath.Parse(json).Read<List<string>>("$.store.book[0].author");

//Works fine
string author = JsonPath.Parse(json).Read<string>("$.store.book[0].author");
```

When evaluating a path you need to understand the concept of when a path is `definite`. A path is `indefinite` if it contains:

* `..` - a deep scan operator
* `?(<expression>)` - an expression
* `[<number>, <number> (, <number>)]` - multiple array indexes

`Indefinite` paths always returns a list (as represented by current JsonProvider). 

By default a simple object? mapper is provided by the MappingProvider SPI. This allows you to specify the return type you want and the MappingProvider will
try to perform the mapping. In the example below mapping between `Long` and `Date` is demonstrated. 

```C#
string json = "{\"date_as_long\" : 1411455611975}";

Date date = JsonPath.Parse(json).Read("$['date_as_long']", Date.class);
```

If you configure JsonPath to use `JacksonMappingProvider`, `GsonMappingProvider`, or `JakartaJsonProvider` you can even map your JsonPath output directly into POJO's.

```C#
Book book = JsonPath.Parse(json).Read<Book>("$.store.book[0]");
```

To obtain full generics type information, use TypeRef.

```C#
List<string> titles = JsonPath.Parse(JSON_DOCUMENT).Read<List<string>>("$.store.book[*].title");
```

Predicates
----------
There are three different ways to create filter predicates in JsonPath.

### Inline Predicates

Inline predicates are the ones defined in the path.

```C#
List<Dictionary<string, object?>> books =  JsonPath.Parse(json)
                                     .Read<List<Dictionary<string, object?>>>("$.store.book[?(@.price < 10)]");
```

You can use `&&` and `||` to combine multiple predicates `[?(@.price < 10 && @.category == 'fiction')]` , 
`[?(@.category == 'reference' || @.price > 10)]`.
 
You can use `!` to negate a predicate `[?(!(@.price < 10 && @.category == 'fiction'))]`.

### Filter Predicates
 
Predicates can be built using the Filter API as shown below:

```C#

Filter cheapFictionFilter = Filter.Create(
   Criteria.Where(jsonProvider, "category").Is("fiction").And("price").Lte(10D)
);

List<Dictionary<string, object?>> books =  
   JsonPath.Parse(json).Read<List<Dictionary<string, object?>>>("$.store.book[?]", cheapFictionFilter);

```
Notice the placeholder `?` for the filter in the path. When multiple filters are provided they are applied in order where the number of placeholders must match 
the number of provided filters. You can specify multiple predicate placeholders in one filter operation `[?, ?]`, both predicates must match. 

Filters can also be combined with 'OR' and 'AND'
```C#
IJsonProvider jsonProvider = ...;
var fooOrBar = Filter.Create(
   Criteria.Where(jsonProvider, "foo").Exists(true)).Or(Criteria.Where(jsonProvider, "bar").Exists(true)
);
   
var fooAndBar = Filter.Create(
   Criteria.Where(jsonProvider, "foo").Exists(true)).And(Criteria.Where(jsonProvider, "bar").Exists(true)
);
```

### Roll Your Own
 
Third option is to implement your own predicates.  You can either use the IPredicate interface directory, 
or use the SimplePredicate class.
 
```C# 
IPredicate booksWithISBN = new SimplePredicate(ctx=> ctx.GetItem<IDictionary<string, object??>>().ContainsKey("isbn"));

List<Dictionary<string, object?>> books = 
   reader.Read<List<Dictionary<string, object?>>>("$.store.book[?].isbn", booksWithISBN);
```

Path vs Value
-------------
In the Goessner implementation a JsonPath can return either `Path` or `Value`. `Value` is the default and what all the examples above are returning. If you rather have the path of the elements our query is hitting this can be achieved with an option.

```C#
Configuration conf = Configuration.CreateBuilder()
   .WithOptions(ConfigurationOptionEnum.AsPathList).Build();

List<string> pathList = JsonPath.Using(conf).Parse(json).Read<List<string>>("$..author");

Assert.Contains("$['store']['book'][0]['author']", pathList);
Assert.Contains("$['store']['book'][1]['author']", pathList);
Assert.Contains("$['store']['book'][2]['author']", pathList);
Assert.Contains("$['store']['book'][3]['author']", pathList);
```

Set a value 
-----------
The library offers the possibility to set a value.

```C#
string newJson = JsonPath.Parse(json).Set("$['store']['book'][0]['author']", "Paul").JsonString();
```



Tweaking Configuration
----------------------

### Options
When creating your Configuration there are a few option flags that can alter the default behavior.

**ConfigurationOptionEnum.DefaultPathLeafToNull**
<br/>
This option makes JsonPath return null for missing leafs. Consider the following json

```javascript
[
   {
      "name" : "john",
      "gender" : "male"
   },
   {
      "name" : "ben"
   }
]
```

```C#
Configuration conf = Configuration.DefaultConfiguration;

//Works fine
string gender0 = JsonPath.Using(conf).Parse(json).Read<string>("$[0]['gender']");
//PathNotFoundException thrown
string gender1 = JsonPath.Using(conf).Parse(json).Read<string>("$[1]['gender']");

Configuration conf2 = conf.addOptions(ConfigurationOptionEnum.DefaultPathLeafToNull);

//Works fine
string gender0 = JsonPath.Using(conf2).Parse(json).Read<string>("$[0]['gender']");
//Works fine (null is returned)
string gender1 = JsonPath.Using(conf2).Parse(json).Read<string>("$[1]['gender']");
```
 
**ConfigurationOptionEnum.AlwaysReturnList**
<br/>
This option configures JsonPath to return a list even when the path is `definite`. 
 
```C#
Configuration conf = Configuration.DefaultConfiguration;

//Exception thrown
List<string> genders0 = JsonPath.Using(conf).Parse(json).Read<List<string>>("$[0]['gender']");

Configuration conf2 = conf.addOptions(ConfigurationOptionEnum.AlwaysReturnList);

//Works fine
List<string> genders0 = JsonPath.Using(conf2).Parse(json).Read<List<string>>("$[0]['gender']");
``` 
**ConfigurationOptionEnum.SuppressExceptions**
This option makes sure no exceptions are propagated from path evaluation. It follows these simple rules:

* If option `AlwaysReturnList` is present an empty list will be returned
* If option `AlwaysReturnList` is **NOT** present null returned 

**ConfigurationOptionEnum.RequireProperties**
This option configures JsonPath to require properties defined in path when an `indefinite` path is evaluated.

```C#
Configuration conf = Configuration.DefaultConfiguration;

//Works fine
List<string> genders = JsonPath.Using(conf).Parse(json).Read<List<string>>("$[*]['gender']");

Configuration conf2 = conf.addOptions(ConfigurationOptionEnum.RequireProperties);

//PathNotFoundException thrown
List<string> genders = JsonPath.Using(conf2).Parse(json).Read<List<string>>("$[*]['gender']");
```

### IJsonProvider interface

JsonPath is shipped to internally use two different Json serializers, implemented as interface IJsonProvider:

* [SystemTextJsonProvider](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.serialization?view=net-8.0) (default)
* [NewtonsoftJsonProvider](https://www.newtonsoft.com/json) 

Changing the configuration defaults as demonstrated should only be done when your application is being initialized. Changes during runtime is strongly discouraged, especially in multi threaded applications.
  
```C#
Configuration.SetDefaults(new DefaultsImpl(new NewtonsoftJsonProvider(), new NewtonsoftJsonMappingProvider(), new HashSet<ConfigurationOptionsEnum>());
```


### ICache interface

The interface ICache allows API consumers to configure path caching in a way that suits their needs. The cache must be configured before it is accesses for the first time or a JsonPathException is thrown. JsonPath ships with a single cache implementation that uses thread-safe [System.Runtime.Caching](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.caching?view=net-8.0),
but you can replace it with your own:

```C#
ICache myCacheProvider = new MyCacheImplementation();

CacheProvider.Cache = myCacheProvider;
```



