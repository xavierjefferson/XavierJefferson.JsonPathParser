namespace XavierJefferson.JsonPathParser.Extensions;

public static class ObjectExtensions
{
    public static bool TryConvertDouble(this object? x, out double result)
    {

        result = 0;
        if (x == null) { return false; }
        if (x is double myDouble)
        {
            result = myDouble;
            return true;
        }
        if (x is long)
        {
            result = Convert.ToDouble(x);
            return true;
        }
        return false;
    }
}