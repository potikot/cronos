using Cronos.Bootstrap.DependencyInjection.Options;
using Cronos.Infrastructure.Logger;
using Cronos.Infrastructure.StringFormatter;
using Cronos.Infrastructure.StringFormatter.Model;

namespace Cronos.Infrastructure.Printer;

public class ConsolePrinterService : IPrinterService
{
    private readonly IStringFormatterService _formatter;

    public ConsoleColor TextColor { get; set; }
    public ConsoleColor BackgroundColor { get; set; }

    public ConsolePrinterService(
        IOptionObserver<ILoggerOptions> options,
        IStringFormatterService stringFormatterService
    ) {
        _formatter = stringFormatterService;

        TextColor = options.Value.DefaultColor;
        BackgroundColor = ConsoleColor.Black;
    }

    public void Print(string text, PrintOptions options = null)
    {
        ConsoleColor tmp = Console.ForegroundColor;
        ColoredStringList output = _formatter.ColorFormat(text, options?.TextColor ?? TextColor, options?.FormatterParams);
        foreach (ColoredString str in output.Content)
        {
            Console.ForegroundColor = str.TextColor ?? TextColor;
            Console.BackgroundColor = str.BackgroundColor ?? BackgroundColor;
            Console.Write(str.Text);
        }

        Console.ForegroundColor = tmp;
    }

    public void PrintLine(string text, PrintOptions options = null)
    {
        Print(text, options);
        Console.WriteLine();
    }

    public void PrintLine(int count = 1)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        
        for (int i = 0; i < count; i++)
            Console.WriteLine();
    }
}