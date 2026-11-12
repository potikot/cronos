namespace Cronos.Infrastructure.Input;

public interface IInputService
{
    string GetLine(string text = null, ConsoleColor color = ConsoleColor.Gray);
    // int GetChar(string text = null, ConsoleColor color = ConsoleColor.Gray); todo: breaks next Console.ReadLine()
    ConsoleKeyInfo GetKey(string text = null, ConsoleColor color = ConsoleColor.Gray);
    bool AskYesNo(string text, ConsoleColor color = ConsoleColor.Gray);
}