namespace Cronos.Utilities;

public static class StringUtils
{
    public static TOutput Pipe<TInput, TOutput>(this TInput input, Func<TInput, TOutput> transform)
        where TInput : class
        where TOutput : class
    {
        if (input == null || transform == null)
            return null;
        
        return transform(input);
    }

    public static string CapitalizeFirstLetter(this string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return input;
        return char.ToUpper(input[0]) + input[1..];
    }

    public static string Color(this string text, ConsoleColor color, bool end = true)
    {
        string colorText = color.ToString().ToLower();
        return $"[{colorText}]{text}{(end ? "[/]" : "")}";
    }
}