namespace Cronos.Infrastructure.CommandProcessor.Commands;

public abstract class BaseCommand
{
    protected const StringComparison NameComparisonType = StringComparison.OrdinalIgnoreCase;

    protected static readonly Task<CommandResult> ExitResult = Task.FromResult(CommandResult.Exit);
    protected static readonly Task<CommandResult> EmptyResult = Task.FromResult(CommandResult.Empty);
    protected static readonly Task<CommandResult> SuccessResult = Task.FromResult(CommandResult.Success);
    protected static readonly Task<CommandResult> InvalidResult = Task.FromResult(CommandResult.Invalid);

    public abstract string Name { get; }
    public abstract string Description { get; }

    public abstract Task<CommandResult> ProcessAsync(string[] args);

    protected static bool TryParseArguments(string[] args, out CommandArguments result)
    {
        if (args is not { Length: not 0 })
        {
            result = null;
            return false;
        }

        result = new CommandArguments();
        string currentOption = null;
        foreach (string arg in args)
        {
            if (arg.StartsWith('-'))
            {
                currentOption = arg;
                result.Options.TryAdd(currentOption, []);
                continue;
            }
            
            if (currentOption == null)
                result.Arguments.Add(arg);
            else
                result.Options[currentOption].Add(arg);
        }
        
        return true;
    }
}