using Cronos.Infrastructure.CommandProcessor;
using Cronos.Infrastructure.CommandProcessor.Commands;
using Cronos.Infrastructure.Printer;

namespace Cronos.Features.Telegram.Bot.Commands;

public class SendMessageCommand(IPrinterService _printer, ITelegramBotService _telegramBotService) : BaseCommand
{
    public override string Name { get; } = "send";
    public override string Description { get; } = "Returns list of available chats.";

    /// <param name="args">
    /// <br></br>[0] - telegram chat id (<see cref="long"/>)
    /// <br></br>[0] - message text (<see cref="string"/>)
    /// </param>
    public override async Task<CommandResult> ProcessAsync(string[] args)
    {
        if (args.Length <= 1)
        {
            _printer.PrintLine("You need to enter chat id and message after");
            return CommandResult.Invalid;
        }

        if (!long.TryParse(args[0], out long chatId))
        {
            _printer.PrintLine("Failed to parse chat id");
            return CommandResult.Invalid;
        }

        string message = string.Join(" ", args.Skip(1));
        int messageId = await _telegramBotService.SendMessageAsync(chatId, message);

        if (messageId == 0)
            _printer.PrintLine($"Unexpected error");
        else
            _printer.PrintLine($"Succesfully sent msg({messageId})");

        return CommandResult.Success;
    }
}