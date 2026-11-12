using Cronos.Features.Game.Shared.Commands;
using Cronos.Infrastructure.BrowserLauncher;
using Cronos.Infrastructure.CommandProcessor.Commands;
using Cronos.Infrastructure.Printer;

namespace Cronos.Features.Game.EpicGames;

public class EpicGamesCommand : CompositeCommandBase
{
    public override string Name { get; } = "epic";
    public override string Description { get; } = "Interacts with EpicGamesStore";

    public EpicGamesCommand(
        IPrinterService printer,
        IEpicGamesService gameService,
        IBrowserLauncher browserLauncher)
    {
        DefaultCommand = new PrintFreeGamesCommand(printer, gameService);
        
        subCommands.Add(new LoginGameServiceCommand(printer, gameService));
        subCommands.Add(new LogoutGameServiceCommand(printer, gameService));
        subCommands.Add(new OpenFreeGameCommand(printer, gameService, browserLauncher));
        subCommands.Add(DefaultCommand);
        subCommands.Add(new UpdateFreeGamesCommand(printer, gameService));
    }
}