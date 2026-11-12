using Cronos.Infrastructure.CommandProcessor.Commands;

namespace Cronos.Infrastructure.CommandProcessor;

public interface ICommandHandler
{
    IReadOnlyCollection<BaseCommand> Commands { get; }
    
    bool TryAddCommand(BaseCommand command);
    bool RemoveCommand(string name);
    bool RemoveCommand(string name, out BaseCommand command);
    Task<CommandResult> ProcessCommandAsync(string input);
    BaseCommand GetCommand(string commandPath);
    BaseCommand GetCommand(IReadOnlyList<string> commandPath);
}