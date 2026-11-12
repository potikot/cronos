using Cronos.Features.Yougile.Model;
using Cronos.Infrastructure.CommandProcessor;
using Cronos.Infrastructure.CommandProcessor.Commands;
using Cronos.Infrastructure.Printer;

namespace Cronos.Features.Yougile.Commands;

public class AddYouGileTaskCommand(
    YougileService yougileService,
    IPrinterService _printer) : BaseCommand
{
    public override string Name { get; } = "add";
    public override string Description { get; } = "Adds task to YouGile board.";

    public override async Task<CommandResult> ProcessAsync(string[] args)
    {
        if (!TryParseArguments(args, out CommandArguments arguments))
            return CommandResult.Invalid;

        string title = arguments.Arguments[0];
        YougileTask task = await yougileService.AddTaskAsync(title, null);

        if (task == null)
        {
            _printer.PrintLine($"Failed to add task '{title}' to YouGile board.");
            return CommandResult.Invalid;
        }
        
        _printer.PrintLine($"Task '{task.Title}' added to YouGile board.");
        return CommandResult.Success;
    }
}