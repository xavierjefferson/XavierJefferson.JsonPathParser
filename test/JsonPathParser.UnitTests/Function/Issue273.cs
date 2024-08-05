using XavierJefferson.JsonPathParser.UnitTests.Extensions;

namespace XavierJefferson.JsonPathParser.UnitTests.Function;

public class Issue273
{
    [Fact]
    public void TestGetPropertyFromArray()
    {
        var json = "[\n" +
                   "   [\n" +
                   "      {\n" +
                   "         \"category\" : \"reference\",\n" +
                   "         \"author\" : \"Nigel Rees\",\n" +
                   "         \"title\" : \"Sayings of the Century\",\n" +
                   "         \"price\" : 8.95\n" +
                   "      },\n" +
                   "      {\n" +
                   "         \"category\" : \"fiction\",\n" +
                   "         \"author\" : \"Evelyn Waugh\",\n" +
                   "         \"title\" : \"Sword of Honour\",\n" +
                   "         \"price\" : 12.99\n" +
                   "      },\n" +
                   "      {\n" +
                   "         \"category\" : \"fiction\",\n" +
                   "         \"author\" : \"Herman Melville\",\n" +
                   "         \"title\" : \"Moby Dick\",\n" +
                   "         \"isbn\" : \"0-553-21311-3\",\n" +
                   "         \"price\" : 8.99\n" +
                   "      },\n" +
                   "      {\n" +
                   "         \"category\" : \"fiction\",\n" +
                   "         \"author\" : \"J. R. R. Tolkien\",\n" +
                   "         \"title\" : \"The Lord of the Rings\",\n" +
                   "         \"isbn\" : \"0-395-19395-8\",\n" +
                   "         \"price\" : 22.99\n" +
                   "      }\n" +
                   "   ],\n" +
                   "   {\n" +
                   "      \"color\" : \"red\",\n" +
                   "      \"price\" : 19.95\n" +
                   "   }\n" +
                   "]\n";

        var arr = JsonPath.Read(json, "$..[2].author").AsList();
        Assert.Equal("Herman Melville", arr[0]);
    }

    [Fact]
    public void TestGetPropertyFromObject()
    {
        var json = "{\n" +
                   "    \"store\": {\n" +
                   "        \"book\": [\n" +
                   "            {\n" +
                   "                \"category\": \"reference\",\n" +
                   "                \"author\": \"Nigel Rees\",\n" +
                   "                \"title\": \"Sayings of the Century\",\n" +
                   "                \"price\": 8.95\n" +
                   "            },\n" +
                   "            {\n" +
                   "                \"category\": \"fiction\",\n" +
                   "                \"author\": \"Evelyn Waugh\",\n" +
                   "                \"title\": \"Sword of Honour\",\n" +
                   "                \"price\": 12.99\n" +
                   "            },\n" +
                   "            {\n" +
                   "                \"category\": \"fiction\",\n" +
                   "                \"author\": \"Herman Melville\",\n" +
                   "                \"title\": \"Moby Dick\",\n" +
                   "                \"isbn\": \"0-553-21311-3\",\n" +
                   "                \"price\": 8.99\n" +
                   "            },\n" +
                   "            {\n" +
                   "                \"category\": \"fiction\",\n" +
                   "                \"author\": \"J. R. R. Tolkien\",\n" +
                   "                \"title\": \"The Lord of the Rings\",\n" +
                   "                \"isbn\": \"0-395-19395-8\",\n" +
                   "                \"price\": 22.99\n" +
                   "            }\n" +
                   "        ],\n" +
                   "        \"bicycle\": {\n" +
                   "            \"color\": \"red\",\n" +
                   "            \"price\": 19.95\n" +
                   "        }\n" +
                   "    },\n" +
                   "    \"expensive\": 10\n" +
                   "}\n" +
                   "                ";
        var arr = JsonPath.Read(json, "$..[2].author").AsList();
        Assert.Equal("Herman Melville", arr[0]);
    }
}