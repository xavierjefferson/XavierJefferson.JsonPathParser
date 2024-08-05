using XavierJefferson.JsonPathParser.Enums;
using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser;

public class LimitingEvaluationListener 
{
    private readonly int _limit;

    public LimitingEvaluationListener(int limit)
    {
        _limit = limit;
    }


    public EvaluationContinuationEnum ResultFound(IFoundResult found)
    {
        if (found.Index == _limit - 1)
            return EvaluationContinuationEnum.Abort;
        return EvaluationContinuationEnum.Continue;
    }
}