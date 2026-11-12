namespace Cronos.Infrastructure.Printer;

public interface IPrinterService
{
    ConsoleColor TextColor { get; set; }
    ConsoleColor BackgroundColor { get; set; }
    
    void Print(string text, PrintOptions options = null);
    void PrintLine(string text, PrintOptions options = null);
    void PrintLine(int count = 1);
}