namespace XavierJefferson.JsonPathParser.UnitTests;

public class Issue_973
{
    [Fact]
    public void shouldNotCauseStackOverflow()
    {
        var _ = Criteria.Parse("@[\"\",/\\");
    }
}