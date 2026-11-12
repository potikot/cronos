namespace Cronos.Utilities;

public static class ColorUtils
{
    private static readonly Dictionary<string, ConsoleColor> Dictionary = [];

    static ColorUtils()
    {
        ConsoleColor[] values = Enum.GetValues<ConsoleColor>();

        foreach (ConsoleColor color in values)
            Dictionary.Add(color.ToString().ToLower(), color);
    }

    public static ConsoleColor GetOrDefault(string name, ConsoleColor defaultValue = ConsoleColor.Gray)
    {
        return Dictionary.GetValueOrDefault(name.ToLower(), defaultValue);
    }
}