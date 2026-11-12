using Cronos.Infrastructure.Printer;

namespace Cronos.Infrastructure.Input;

public class ConsoleInputService(IPrinterService _printer) : IInputService
{
    private static readonly List<string> _positiveAnswers = ["y", "yes", "on"];
    
    public string GetLine(string text, ConsoleColor color = ConsoleColor.Gray)
    {
        _printer.Print(text, new PrintOptions() { TextColor = color });
        return Console.ReadLine();
    }

    public int GetChar(string text, ConsoleColor color = ConsoleColor.Gray)
    {
        _printer.Print(text, new PrintOptions() { TextColor = color });
        return Console.Read();
    }

    public ConsoleKeyInfo GetKey(string text, ConsoleColor color = ConsoleColor.Gray)
    {
        _printer.Print(text, new PrintOptions() { TextColor = color });
        return Console.ReadKey();
    }

    public bool AskYesNo(string text, ConsoleColor color = ConsoleColor.Gray)
    {
        string question = string.IsNullOrWhiteSpace(text) ? "(Y/N) " : $"{text} (Y/N) ";
        string answer = GetLine(question, color);
        return _positiveAnswers.Any(x => x.Equals(answer, StringComparison.OrdinalIgnoreCase));
    }
}