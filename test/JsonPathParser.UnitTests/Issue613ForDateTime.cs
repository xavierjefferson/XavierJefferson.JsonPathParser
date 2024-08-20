namespace XavierJefferson.JsonPathParser.UnitTests;

public class Issue613ForDateTime : Issue613Base<DateTime>
{
    public override DateTime LargeValue => new(2001, 3, 1, 1, 1, 1, 1);

    public override DateTime MiddleValue => new(2000, 2, 1, 1, 1, 1, 1);

    public override DateTime SmallValue => new(1999, 2, 1, 1, 1, 1, 1);
}