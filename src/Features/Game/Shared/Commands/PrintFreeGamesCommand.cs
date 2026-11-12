using System.Text;
using Cronos.Infrastructure.CommandProcessor;
using Cronos.Infrastructure.CommandProcessor.Commands;
using Cronos.Infrastructure.Localization;
using Cronos.Infrastructure.Printer;
using Cronos.Infrastructure.StringFormatter.Model;
using TextCopy;

namespace Cronos.Features.Game.Shared.Commands;

public class PrintFreeGamesCommand : BaseCommand
{
    private static readonly LocalizedString ListElementLocale = new("list_element", "{index}. {title} | Url: {url}");
    private static readonly LocalizedString TitleLocale = new("game_list_title", "Found {count} free game{s} in {store}:");
    
    private readonly IPrinterService _printer;
    private readonly IGameService _gameService;
    
    private readonly string _serviceName;
    
    public override string Name { get; } = "games";
    public override string Description { get; } = "Prints free games";

    public PrintFreeGamesCommand(IPrinterService printer, IGameService gameService)
    {
        _printer = printer;
        _gameService = gameService;
        _serviceName = _gameService.GetType().Name.Replace("Service", "");
    }
    
    public override async Task<CommandResult> ProcessAsync(string[] args)
    {
        TryParseArguments(args, out CommandArguments arguments);
        List<BaseGameData> games = await _gameService.GetFreeGamesAsync();

        if (games.Count > 0)
        {
            if (arguments != null)
            {
                if (arguments.HasOption("-c"))
                {
                    StringBuilder sb = new();
                    foreach (BaseGameData game in games)
                        sb.AppendFormat("- {0}\n", game.Url);
                    await ClipboardService.SetTextAsync(sb.ToString());

                    _printer.PrintLine("Copied to clipboard.");
                    
                    List<string> oa = arguments.GetOptionArguments("-c");
                    if (oa.Count > 0
                        && bool.TryParse(oa[0], out bool value)
                        && value)
                        return CommandResult.Success;
                }
            
                if (arguments.HasOption("-t"))
                {
                    FormatterParams titleParameters = new()
                    {
                        { "count", games.Count.ToString() },
                        { "store", _serviceName }
                    };
                
                    if (TitleLocale.LocalizationService.Language == "en")
                        titleParameters.Add("s", games.Count > 1 ? "s" : "");
                    
                    _printer.PrintLine(TitleLocale.ToString(), new PrintOptions()
                    {
                        FormatterParams = titleParameters
                    });
                }
            }
            
            FormatterParams listParameters = new()
            {
                { "index", 0 },
                { "title", "" },
                { "url", "" },
                { "color", ConsoleColor.Gray }
            };
            
            for (int i = 0; i < games.Count; i++)
            {
                games[i].IsOwned = await _gameService.IsGameOwnedAsync(games[i]);

                listParameters["index"] = i + 1;
                listParameters["title"] = games[i].Title;
                listParameters["url"] = games[i].Url;
                listParameters["color"] = games[i].IsOwned ? ConsoleColor.Green : ConsoleColor.Red;

                _printer.PrintLine(ListElementLocale.ToString(), new PrintOptions()
                {
                    FormatterParams = listParameters
                });
            }
        }
        else
            _printer.PrintLine("\nFound 0 free games.");
        
        _printer.PrintLine();
        return CommandResult.Success;
    }
}