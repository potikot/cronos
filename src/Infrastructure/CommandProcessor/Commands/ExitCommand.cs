namespace Cronos.Infrastructure.CommandProcessor.Commands;

public class ExitCommand : BaseCommand
{
    public override string Name { get; } = "exit";
    public override string Description { get; } = "Exits the program";

    /// <param name="args"> null </param>
    public override Task<CommandResult> ProcessAsync(string[] args)
    {
        return ExitResult;
    }
}