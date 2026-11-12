using Cronos.Features.Game.Shared.Commands;
using Cronos.Infrastructure.BrowserLauncher;
using Cronos.Infrastructure.CommandProcessor.Commands;
using Cronos.Infrastructure.Printer;

namespace Cronos.Features.Game.Steam;

public class SteamCommand : CompositeCommandBase
{
    public override string Name { get; } = "steam";
    public override string Description { get; } = "Works with Steam web API.";

    public SteamCommand(
        IPrinterService printer,
        ISteamService gameService,
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