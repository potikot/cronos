using Cronos.Infrastructure.BrowserLauncher;
using Cronos.Infrastructure.CommandProcessor;
using Cronos.Infrastructure.CommandProcessor.Commands;
using Cronos.Infrastructure.Printer;
using Cronos.Utilities;

namespace Cronos.Features.Game.Shared.Commands;

public class OpenFreeGameCommand(
    IPrinterService _printer,
    IGameService _gameService,
    IBrowserLauncher _browser) : BaseCommand
{
    private static readonly PrintOptions SuccessOpenPrintOptions = new PrintOptions()
    {
        TextColor = ConsoleColor.Green
    };
    
    public override string Name { get; } = "open";
    public override string Description { get; } = "Opens free game from list";

    /// <param name="args">
    /// <br></br>[0] - game number in console (<see cref="int"/>)
    /// </param>
    public override async Task<CommandResult> ProcessAsync(string[] args)
    {
        List<BaseGameData> games = await _gameService.GetFreeGamesAsync();

        if (!TryParseArguments(args, out CommandArguments arguments))
        {
            foreach (BaseGameData game in games)
                OpenGamePage(game);
            
            return CommandResult.Success;
        }
        else if (int.TryParse(arguments.Arguments[0], out int appIdx))
        {
            if (!games.InRange(--appIdx))
                return CommandResult.Invalid;

            OpenGamePage(games[appIdx]);
            return CommandResult.Success;
        }

        return CommandResult.Invalid;
    }

    private void OpenGamePage(BaseGameData game)
    {
        string url = string.IsNullOrEmpty(game.BuyUrl) ? game.Url : game.BuyUrl;
        _browser.Open(url);
        _printer.PrintLine($"Opened '{game.Title}' game page.", SuccessOpenPrintOptions);
    }
}