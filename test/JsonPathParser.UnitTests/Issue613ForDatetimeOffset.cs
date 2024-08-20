namespace XavierJefferson.JsonPathParser.UnitTests;

//test for issue: https://github.com/json-path/JsonPath/issues/613
public class Issue613ForDatetimeOffset : Issue613Base<DateTimeOffset>
{
    public override DateTimeOffset MiddleValue => new(2000, 2, 1, 1, 1, 1, 1, DateTimeOffset.Now.Offset);

    public override DateTimeOffset SmallValue => new(1999, 2, 1, 1, 1, 1, 1, DateTimeOffset.Now.Offset);

    public override DateTimeOffset LargeValue => new(2001, 3, 1, 1, 1, 1, 1, TimeSpan.FromHours(14));
}