namespace Cronos.Infrastructure.Logger;

public interface ILoggerOptions
{
    ConsoleColor DefaultColor { get; }
    ConsoleColor LogColor { get; }
    ConsoleColor WarningColor { get; }
    ConsoleColor ErrorColor { get; }
}

public class LoggerOptions : ILoggerOptions
{
    public ConsoleColor DefaultColor { get; set; }
    public ConsoleColor LogColor { get; set; }
    public ConsoleColor WarningColor { get; set; }
    public ConsoleColor ErrorColor { get; set; }
}