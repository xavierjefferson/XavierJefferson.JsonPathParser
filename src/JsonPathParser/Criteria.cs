using System.Text.RegularExpressions;
using XavierJefferson.JsonPathParser.Exceptions;
using XavierJefferson.JsonPathParser.Extensions;
using XavierJefferson.JsonPathParser.Filtering;
using XavierJefferson.JsonPathParser.Filtering.ValueNodes;
using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser;
 
public class Criteria : IPredicate
{
    private readonly SerializingList<Criteria> _criteriaChain;
    private RelationalOperator? _criteriaType;
    private ValueNode? _left;
    private ValueNode? _right;

    private Criteria(SerializingList<Criteria> criteriaChain, ValueNode left)
    {
        _left = left;
        _criteriaChain = criteriaChain;
        _criteriaChain.Add(this);
    }

    private Criteria(ValueNode left) : this(new SerializingList<Criteria>(), left)
    {
    }


    public bool Apply(IPredicateContext ctx)
    {
        foreach (var expressionNode in ToRelationalExpressionNodes())
            if (!expressionNode.Apply(ctx))
                return false;
        return true;
    }


    public override string ToString()
    {
        var objs = ToRelationalExpressionNodes().Cast<object?>();
        return string.Join(" && ", objs);
    }

    private ICollection<RelationalExpressionNode> ToRelationalExpressionNodes()
    {
        var nodes = new SerializingList<RelationalExpressionNode>(_criteriaChain.Count());
        foreach (var criteria in _criteriaChain)
            nodes.Add(new RelationalExpressionNode(criteria._left, criteria._criteriaType, criteria._right));
        return nodes;
    }

    /// <summary>
    ///     Static factory method to create a Criteria using the provided key
    /// </summary>
    /// <param name="key">filed name</param>
    /// <returns> the new criteria</returns>
    [Obsolete]
    //This should be private.It exposes public classes
    public static Criteria Where(IPath key)
    {
        return new Criteria(ValueNode.CreatePathNode(key));
    }


    /// <summary>
    ///     Static factory method to create a Criteria using the provided key
    /// </summary>
    /// <param name="key">filed name</param>
    /// <returns> the new criteria</returns>
    public static Criteria Where(string key)
    {
        return new Criteria(ValueNode.ToValueNode(PrefixPath(key)));
    }

    /// <summary>
    ///     Static factory method to create a Criteria using the provided key
    /// </summary>
    /// <param name="key">ads new filed to criteria</param>
    /// <returns> the criteria builder</returns>
    public Criteria And(string key)
    {
        CheckComplete();
        return new Criteria(_criteriaChain, ValueNode.ToValueNode(PrefixPath(key)));
    }

    /// <summary>
    ///     Creates a criterion using equality
    /// </summary>
    /// <param name="o">
    ///     ///
    /// </param>
    ///     <returns> the criteria</returns>
    public Criteria Is(object? o)
    {
        _criteriaType = RelationalOperator.Eq;
        _right = ValueNode.ToValueNode(o);
        return this;
    }

    /// <summary>
    ///     Creates a criterion using equality
    /// </summary>
    /// <param name="o">
    ///     ///
    /// </param>
    ///     <returns> the criteria</returns>
    public Criteria Eq(object? o)
    {
        return Is(o);
    }

    /// <summary>
    ///     Creates a criterion using the <b>!=</b> operator
    /// </summary>
    /// <param name="o">
    ///     ///
    /// </param>
    ///     <returns> the criteria</returns>
    public Criteria Ne(object? o)
    {
        _criteriaType = RelationalOperator.Ne;
        _right = ValueNode.ToValueNode(o);
        return this;
    }

    /// <summary>
    ///     Creates a criterion using the <b>&lt;</b> operator
    /// </summary>
    /// <param name="o">
    ///     ///

    /// </param>
    ///     <returns> the criteria</returns>
    public Criteria Lt(object? o)
    {
        _criteriaType = RelationalOperator.Lt;
        _right = ValueNode.ToValueNode(o);
        return this;
    }

    /// <summary>
    ///     Creates a criterion using the <b>&lt;=</b> operator
    /// </summary>
    /// <param name="o">
  
    /// </param>
    ///     <returns> the criteria</returns>
    public Criteria Lte(object? o)
    {
        _criteriaType = RelationalOperator.Lte;
        _right = ValueNode.ToValueNode(o);
        return this;
    }

    /// <summary>
    ///     Creates a criterion using the <b>&gt;</b> operator
    /// </summary>
    /// <param name="o"> 
    /// </param>    
    /// ///     <returns> the criteria</returns>
    public Criteria Gt(object? o)
    {
        _criteriaType = RelationalOperator.Gt;
        _right = ValueNode.ToValueNode(o);
        return this;
    }

    /// <summary>
    ///     Creates a criterion using the <b>&gt;=</b> operator
    /// </summary>
    /// <param name="o">
    ///     ///
    ///     <returns> the criteria</returns>
    /// </param>
    public Criteria Gte(object? o)
    {
        _criteriaType = RelationalOperator.Gte;
        _right = ValueNode.ToValueNode(o);
        return this;
    }

    /// <summary>
    ///     Creates a criterion using a Regex
    /// </summary>
    /// <param name="pattern">
    ///     ///
    ///     <returns> the criteria</returns>
    /// </param>
    public Criteria Regex(Regex pattern)
    {
        if (pattern == null) throw new ArgumentNullException(nameof(pattern));
        _criteriaType = RelationalOperator.Regex;
        _right = ValueNode.ToValueNode(pattern);
        return this;
    }

    /// <summary>
    ///     The <code>in</code> operator is analogous to the SQL IN modifier, allowing you
    ///     to specify an array of possible matches.
    /// </summary>
    /// <param name="o">the values to match against</param>
    /// <returns> the criteria</returns>
    public Criteria In(params object?[] o)
    {
        return In(o.ToSerializingList());
    }

    /// <summary>
    ///     The <code>in</code> operator is analogous to the SQL IN modifier, allowing you
    ///     to specify an array of possible matches.
    /// </summary>
    /// <param name="collection">the collection containing the values to match against</param>
    /// <returns> the criteria</returns>
    public Criteria In(ICollection<object?> collection)
    {
        if (collection == null) throw new ArgumentNullException(nameof(collection));
        _criteriaType = RelationalOperator.In;
        _right = new ValueListNode(collection);
        return this;
    }

    /// <summary>
    ///     The <code>contains</code> operator asserts that the provided object is contained
    ///     in the result. The object that should contain the input can be either an object or a string.
    /// </summary>
    /// <param name="o">that should exists in given collection or</param>
    /// <returns> the criteria</returns>
    public Criteria Contains(object? o)
    {
        _criteriaType = RelationalOperator.Contains;
        _right = ValueNode.ToValueNode(o);
        return this;
    }

    /// <summary>
    ///     The <code>nin</code> operator is similar to $in except that it selects objects for
    ///     which the specified field does not have any value in the specified array.
    /// </summary>
    /// <param name="o">the values to match against</param>
    /// <returns> the criteria</returns>
    public Criteria Nin(params object?[] o)
    {
        return Nin((ICollection<object?>)o.ToSerializingList());
    }

    /// <summary>
    ///     The <code>nin</code> operator is similar to $in except that it selects objects for
    ///     which the specified field does not have any value in the specified array.
    /// </summary>
    /// <param name="collection">the values to match against</param>
    /// <returns> the criteria</returns>
    public Criteria Nin(ICollection<object?> collection)
    {
        if (collection == null) throw new ArgumentNullException(nameof(collection));
        _criteriaType = RelationalOperator.Nin;
        _right = new ValueListNode(collection);
        return this;
    }

    /// <summary>
    ///     The <code>subsetof</code> operator selects objects for which the specified field is
    ///     an array whose elements comprise a subset of the set comprised by the elements of
    ///     the specified array.
    /// </summary>
    /// <param name="o">the values to match against</param>
    /// <returns> the criteria</returns>
    public Criteria SubsetOf(params object?[]? o)
    {
        return SubsetOf(o.ToSerializingList());
    }

    /// <summary>
    ///     The <code>subsetof</code> operator selects objects for which the specified field is
    ///     an array whose elements comprise a subset of the set comprised by the elements of
    ///     the specified array.
    /// </summary>
    /// <param name="c">the values to match against</param>
    /// <returns> the criteria</returns>
    public Criteria SubsetOf(ICollection<object?>? c)
    {
        if (c == null) throw new ArgumentNullException(nameof(c));
        _criteriaType = RelationalOperator.SubsetOf;
        _right = new ValueListNode(c ?? new JpObjectList());
        return this;
    }

    /// <summary>
    ///     The <code>anyof</code> operator selects objects for which the specified field is
    ///     an array that contain at least an element in the specified array.
    /// </summary>
    /// <param name="o">the values to match against</param>
    /// <returns> the criteria</returns>
    public Criteria AnyOf(params object?[] o)
    {
        return AnyOf(o.ToSerializingList());
    }

    /// <summary>
    ///     The <code>anyof</code> operator selects objects for which the specified field is
    ///     an array that contain at least an element in the specified array.
    /// </summary>
    /// <param name="c">the values to match against</param>
    /// <returns> the criteria</returns>
    public Criteria AnyOf<T>(IEnumerable<T> c)
    {
        if (c == null) throw new ArgumentNullException(nameof(c));
        _criteriaType = RelationalOperator.AnyOf;
        _right = new ValueListNode(c.Cast<object?>().ToSerializingList());
        return this;
    }

    /// <summary>
    ///     The <code>noneof</code> operator selects objects for which the specified field is
    ///     an array that does not contain any of the elements of the specified array.
    /// </summary>
    /// <param name="o">the values to match against</param>
    /// <returns> the criteria</returns>
    public Criteria NoneOf(params object?[] o)
    {
        return NoneOf(o.ToSerializingList());
    }

    /// <summary>
    ///     The <code>noneof</code> operator selects objects for which the specified field is
    ///     an array that does not contain any of the elements of the specified array.
    /// </summary>
    /// <param name="c">the values to match against</param>
    /// <returns> the criteria</returns>
    public Criteria NoneOf(ICollection<object?> c)
    {
        if (c == null) throw new ArgumentNullException(nameof(c));
        _criteriaType = RelationalOperator.NoneOf;
        _right = new ValueListNode(c ?? new JpObjectList());
        return this;
    }

    ///<summary>
    /// The <code>all</code> operator is similar to $in, but instead of matching any value
    /// in the specified array all values in the array must be matched.
    ///</summary>
    ///<param name="o"></param>
    ///<returns> the criteria</returns>
    public Criteria All(params object?[] o)
    {
        return All(o.ToSerializingList());
    }

    /// <summary>
    ///     The <code>all</code> operator is similar to $in, but instead of matching any value
    ///     in the specified array all values in the array must be matched.
    /// </summary>
    /// <param name="collection">
    ///     ///
    ///     <returns> the criteria</returns>
    /// </param>
    public Criteria All(ICollection<object?> collection)
    {
        if (collection == null) throw new ArgumentNullException(nameof(collection));
        _criteriaType = RelationalOperator.All;
        _right = new ValueListNode(collection);
        return this;
    }

    /// <summary>
    ///     The <code>size</code> operator matches:
    ///     <p />
    ///     <ol>
    ///         <li>array with the specified number of elements.</li>
    ///         <li>string with given length.</li>
    ///     </ol>
    /// </summary>
    /// <param name="size">
    ///     ///
    ///     <returns> the criteria</returns>
    /// </param>
    public Criteria Size(int size)
    {
        _criteriaType = RelationalOperator.Size;
        _right = ValueNode.ToValueNode(size);
        return this;
    }

    /// <summary>
    ///     The $type operator matches values based on their Java JSON type.
    /// </summary>
    /// Supported types are:
    /// *  typeof(List)
    /// typeof(Dictionary)
    /// typeof(string)
    /// typeof(double)
    /// typeof(Boolean)
    /// * Other types evaluates to false
    /// </summary>
    /// <param name="type">
    ///     ///
    ///     <returns> the criteria</returns>
    /// </param>
    public Criteria Type(Type type)
    {
        _criteriaType = RelationalOperator.Type;
        _right = ValueNode.CreateClassNode(type);
        return this;
    }

    /// <summary>
    ///     Check for existence (or lack thereof) of a field.
    /// </summary>
    /// <param name="shouldExist">
    ///     ///
    ///     <returns> the criteria</returns>
    /// </param>
    public Criteria Exists(bool shouldExist)
    {
        _criteriaType = RelationalOperator.Exists;
        _right = ValueNode.ToValueNode(shouldExist);
        _left = _left.AsPathNode().AsExistsCheck(shouldExist);
        return this;
    }

    /// <summary>
    ///     The <code>notEmpty</code> operator checks that an array or string is not empty.
    /// </summary>
    /// <returns> the criteria</returns>
    [Obsolete]
    public Criteria NotEmpty()
    {
        return Empty(false);
    }

    /// <summary>
    ///     The <code>notEmpty</code> operator checks that an array or string is empty.
    /// </summary>
    /// <param name="empty">should be empty</param>
    /// <returns> the criteria</returns>
    public Criteria Empty(bool empty)
    {
        _criteriaType = RelationalOperator.Empty;
        _right = empty ? ValueNodeConstants.True : ValueNodeConstants.False;
        return this;
    }

    /// <summary>
    ///     The <code>matches</code> operator checks that an object matches the given predicate.
    /// </summary>
    /// <param name="p">
    ///     ///
    /// </param>
    ///     <returns> the criteria</returns>
    public Criteria Matches(IPredicate p)
    {
        _criteriaType = RelationalOperator.Matches;
        _right = new PredicateNode(p);
        return this;
    }

    /// <summary>
    ///     Parse the provided criteria
    /// </summary>
    /// <param name="criteria"></param>
    /// <returns> a criteria</returns>
    [Obsolete]
    public static Criteria Parse(string criteria)
    {
        if (criteria == null) throw new InvalidPathException("Criteria can not be null");
        var split = criteria.Trim().Split(" ");
        if (split.Length == 3)
            return Create(split[0], split[1], split[2]);
        if (split.Length == 1)
            return Create(split[0], nameof(RelationalOperator.Exists), "true");
        throw new InvalidPathException("Could not Parse criteria");
    }

    /// <summary>
    ///     Creates a new criteria</summary>
    ///     <param name="left">path to evaluate in criteria</param>
    ///     <param name="operator">operator</param>
    ///     <param name="right">expected value</param>
    ///     <returns> a new Criteria</returns>
    [Obsolete]
    public static Criteria Create(string left, string @operator, string right)
    {
        var criteria = new Criteria(ValueNode.ToValueNode(left));
        criteria._criteriaType = RelationalOperator.FromName(@operator);
        criteria._right = ValueNode.ToValueNode(right);
        return criteria;
    }


    private static string PrefixPath(string key)
    {
        if (!key.StartsWith("$") && !key.StartsWith("@")) key = $"@.{key}";
        return key;
    }

    private void CheckComplete()
    {
        var complete = _left != null && _criteriaType != null && _right != null;
        if (!complete)
            throw new JsonPathException("Criteria build exception. Complete on criteria before defining next.");
    }
}