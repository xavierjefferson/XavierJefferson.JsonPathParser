using XavierJefferson.JsonPathParser.Interfaces;

namespace XavierJefferson.JsonPathParser.Logging;

public class LoggerFactory
{
    public static Func<Type, ILog> Logger { get; set; } = (type) => new EmptyLogger();
}