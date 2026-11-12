using Cronos.Infrastructure.CommandProcessor;
using Cronos.Infrastructure.CommandProcessor.Commands;
using Cronos.Infrastructure.Printer;

namespace Cronos.Features.Game.Shared.Commands;

public class LogoutGameServiceCommand(IPrinterService _printer, IGameService _gameService) : BaseCommand
{
    private static readonly PrintOptions SuccessLogoutPrintOptions = new PrintOptions()
    {
        TextColor = ConsoleColor.Green
    };
    
    public override string Name { get; } = "logout";
    public override string Description { get; } = "Logouts";

    public override Task<CommandResult> ProcessAsync(string[] args)
    {
        _gameService.Logout();
        _printer.PrintLine("Logout.\n", SuccessLogoutPrintOptions);
        return SuccessResult;
    }
}