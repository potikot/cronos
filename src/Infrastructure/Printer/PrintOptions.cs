using Cronos.Infrastructure.StringFormatter.Model;

namespace Cronos.Infrastructure.Printer;

public record PrintOptions
{
    public ConsoleColor? TextColor { get; init; }
    public ConsoleColor? BackgroundColor { get; init; }
    public FormatterParams FormatterParams { get; init; }
}