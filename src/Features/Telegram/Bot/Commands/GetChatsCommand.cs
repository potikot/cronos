using Cronos.Infrastructure.CommandProcessor;
using Cronos.Infrastructure.CommandProcessor.Commands;
using Cronos.Infrastructure.Printer;

namespace Cronos.Features.Telegram.Bot.Commands;

public class GetChatsCommand(IPrinterService _printer, ITelegramBotService _telegramBotService) : BaseCommand
{
    public override string Name { get; } = "get-chats";
    public override string Description { get; } = "Returns list of available chats.";

    public override async Task<CommandResult> ProcessAsync(string[] args)
    {
        var response = await _telegramBotService.GetUpdatesAsync();
        if (response is null)
        {
            _printer.PrintLine("Failed to get updates.");
            return CommandResult.Success;
        }

        var chats = response.Values;
        if (chats.Count == 0)
        {
            _printer.PrintLine("No chats found.");
            return CommandResult.Success;
        }

        int maxTitleLenght = chats.Max(x => x.Title.Length);
        _printer.PrintLine("title".PadRight(maxTitleLenght) + " | id");
        foreach (var chat in chats)
        {
            _printer.PrintLine($"{chat.Title.PadRight(maxTitleLenght)} | {chat.Id}");
        }
        _printer.PrintLine();

        return CommandResult.Success;
    }
}