namespace Cronos.Infrastructure.CommandProcessor;

public class CommandArguments
{
    public List<string> Arguments { get; }
    public Dictionary<string, List<string>> Options { get; }

    public CommandArguments(List<string> arguments, Dictionary<string, List<string>> options)
    {
        Arguments = arguments;
        Options = options;
    }

    public CommandArguments() : this([], []) { }

    public bool HasOption(string option)
    {
        return Options != null && Options.ContainsKey(option);
    }

    public List<string> GetOptionArguments(string option)
    {
        return Options != null ? Options.GetValueOrDefault(option, []) : [];
    }
}