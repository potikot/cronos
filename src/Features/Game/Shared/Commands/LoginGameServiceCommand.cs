using Cronos.Infrastructure.CommandProcessor;
using Cronos.Infrastructure.CommandProcessor.Commands;
using Cronos.Infrastructure.Printer;

namespace Cronos.Features.Game.Shared.Commands;

public class LoginGameServiceCommand(IPrinterService _printer, IGameService _gameService) : BaseCommand
{
    private static readonly PrintOptions SuccessLoginPrintOptions = new PrintOptions()
    {
        TextColor = ConsoleColor.Green
    };
    
    public override string Name { get; } = "login";
    public override string Description { get; } = "Logins";

    /// <param name="args">
    /// <br></br> [0] - steam developer api key (<see cref="string"/>)
    /// <br></br> [1] - steam client id (<see cref="string"/>)
    /// </param>
    public override async Task<CommandResult> ProcessAsync(string[] args)
    {
        await _gameService.Login();
        
        // switch (args)
        // {
        //     case { Length: 2 }:
        //         steamService.Login(args[0], args[1]);
        //         break;
        //     default:
        //         steamService.Login();
        //         break;
        // }

        if (_gameService.IsLoggedIn)
            _printer.PrintLine("Logged in.\n", SuccessLoginPrintOptions);
        
        return CommandResult.Success;
    }
}