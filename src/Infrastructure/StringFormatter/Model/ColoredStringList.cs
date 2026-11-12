using Cronos.Utilities;

namespace Cronos.Infrastructure.StringFormatter.Model;

public class ColoredStringList(List<ColoredString> content)
{
    public readonly List<ColoredString> Content = content ?? [];
    
    public ColoredString Append(string text, ConsoleColor? textColor)
    {
        return Append(text, textColor, null);
    }

    public ColoredString Append(string text, ConsoleColor? textColor, ConsoleColor? backgroundColor)
    {
        ColoredString coloredString = new(text, textColor, backgroundColor);
        Append(coloredString);

        return coloredString;
    }

    public void Append(ColoredString coloredString)
    {
        Content.Add(coloredString);
    }
    
    public bool RemoveAt(int index)
    {
        if (!Content.InRange(index))
            return false;
        
        Content.RemoveAt(index);
        return true;
    }

    public bool Remove(ColoredString text)
    {
        return Content.Remove(text);
    }

    public bool Remove(string text, StringComparison comparisonType = StringComparison.Ordinal)
    {
        int index = Content.FindIndex(x => x.Text.Equals(text, comparisonType));
        return RemoveAt(index);
    }
}

public class ColoredString(
    string text,
    ConsoleColor? textColor = null,
    ConsoleColor? backgroundColor = null)
{
    public string Text = text;
    public ConsoleColor? TextColor = textColor;
    public ConsoleColor? BackgroundColor = backgroundColor;
}