using Cronos.Infrastructure.Printer;

namespace Cronos.Utilities;

public static class PrinterExtensions
{
    private const string DefaultListElementFormat = "{0}. {1}";
    
    public static void PrintList<T>(this IPrinterService printer, IReadOnlyList<T> list, PrintOptions options)
    {
        printer.PrintList(list, (i, e) => string.Format(DefaultListElementFormat, i + 1, e), options);
    }

    public static void PrintList<T>(this IPrinterService printer, IReadOnlyList<T> list, Func<int, T, string> selector, PrintOptions options)
    {
        ArgumentNullException.ThrowIfNull(list);
        ArgumentNullException.ThrowIfNull(selector);
        
        for (int i = 0; i < list.Count; i++)
            printer.PrintLine(selector(i, list[i]), options);
    }
}