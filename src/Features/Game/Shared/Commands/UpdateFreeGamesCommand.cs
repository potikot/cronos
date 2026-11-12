using Cronos.Infrastructure.CommandProcessor;
using Cronos.Infrastructure.CommandProcessor.Commands;
using Cronos.Infrastructure.Printer;

namespace Cronos.Features.Game.Shared.Commands;

public class UpdateFreeGamesCommand(IPrinterService _printer, IGameService _gameService) : BaseCommand
{
    private static readonly PrintOptions SuccessUpdatePrintOptions = new PrintOptions()
    {
        TextColor = ConsoleColor.Green
    };
    
    public override string Name { get; } = "update";
    public override string Description { get; } = "Updates free games list";

    public override async Task<CommandResult> ProcessAsync(string[] args)
    {
        await _gameService.GetFreeGamesAsync();
        _printer.PrintLine("Games updated.\n", SuccessUpdatePrintOptions);
        return CommandResult.Success;
    }
}