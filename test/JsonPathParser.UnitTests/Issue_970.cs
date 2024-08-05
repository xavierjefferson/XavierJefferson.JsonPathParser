namespace XavierJefferson.JsonPathParser.UnitTests;

public class Issue_970
{
    [Fact]
    public void shouldNotCauseStackOverflow()
    {
        var _ = Criteria.Where("[']',");
    }
}