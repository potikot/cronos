namespace Cronos.Infrastructure.CommandProcessor.Commands;

public abstract class CompositeCommandBase : BaseCommand
{
    
    protected readonly List<BaseCommand> subCommands = [];
    protected BaseCommand DefaultCommand;
    
    public IReadOnlyList<BaseCommand> SubCommands => subCommands;

    public override Task<CommandResult> ProcessAsync(string[] args)
    {
        if (args is not { Length: not 0 })
            return DefaultCommand == null
                ? InvalidResult
                : DefaultCommand.ProcessAsync(args);

        BaseCommand cmd = subCommands.FirstOrDefault(cmd => cmd.Name == args[0]);
        int startIndex = 1;
        
        if (cmd == null)
        {
            startIndex = 0;
            cmd = DefaultCommand;
        }
        
        return cmd == null
            ? InvalidResult
            : cmd.ProcessAsync(args[startIndex..]);
    }

    public virtual bool TryGetSubCommand(string name, out BaseCommand command)
    {
        command = subCommands.FirstOrDefault(cmd => cmd.Name.Equals(name, NameComparisonType));
        return command != null;
    }
}