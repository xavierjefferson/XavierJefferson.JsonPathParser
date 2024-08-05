using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Filtering.Evaluation;

public class EvaluatorFactory
{
    static readonly Dictionary<RelationalOperator, IEvaluator> Evaluators = new();

    static EvaluatorFactory()
    {
        Evaluators.Add(RelationalOperator.Exists, new ExistsEvaluator());
        Evaluators.Add(RelationalOperator.Ne, new NotEqualsEvaluator());
        Evaluators.Add(RelationalOperator.Tsne, new TypeSafeNotEqualsEvaluator());
        Evaluators.Add(RelationalOperator.Eq, new EqualsEvaluator());
        Evaluators.Add(RelationalOperator.Tseq, new TypeSafeEqualsEvaluator());
        Evaluators.Add(RelationalOperator.Lt, new LessThanEvaluator());
        Evaluators.Add(RelationalOperator.Lte, new LessThanEqualsEvaluator());
        Evaluators.Add(RelationalOperator.Gt, new GreaterThanEvaluator());
        Evaluators.Add(RelationalOperator.Gte, new GreaterThanEqualsEvaluator());
        Evaluators.Add(RelationalOperator.Regex, new RegexEvaluator());
        Evaluators.Add(RelationalOperator.Size, new SizeEvaluator());
        Evaluators.Add(RelationalOperator.Empty, new EmptyEvaluator());
        Evaluators.Add(RelationalOperator.In, new InEvaluator());
        Evaluators.Add(RelationalOperator.Nin, new NotInEvaluator());
        Evaluators.Add(RelationalOperator.All, new AllEvaluator());
        Evaluators.Add(RelationalOperator.Contains, new ContainsEvaluator());
        Evaluators.Add(RelationalOperator.Matches, new PredicateMatchEvaluator());
        Evaluators.Add(RelationalOperator.Type, new TypeEvaluator());
        Evaluators.Add(RelationalOperator.SubsetOf, new SubsetOfEvaluator());
        Evaluators.Add(RelationalOperator.AnyOf, new AnyOfEvaluator());
        Evaluators.Add(RelationalOperator.NoneOf, new NoneOfEvaluator());
    }

    public static IEvaluator CreateEvaluator(RelationalOperator @operator)
    {
        return Evaluators[@operator];
    }


}