using Cronos.Infrastructure.StringFormatter.Model;

namespace Cronos.Infrastructure.StringFormatter;

public interface IStringFormatterService
{
    string Format(string format, FormatterParams parameters);
    ColoredStringList ColorFormat(string format, ConsoleColor defaultColor, FormatterParams parameters);
}