using Cronos.Features.Telegram.Bot;
using Cronos.Features.Telegram.Bot.Commands;
using Cronos.Infrastructure.CommandProcessor.Commands;
using Cronos.Infrastructure.Printer;

namespace Cronos.Features.Telegram.Shared;

public class TelegramCommand : CompositeCommandBase
{
    public override string Name { get; } = "tg";
    public override string Description { get; } = "Works with telegram web API.";

    public TelegramCommand(IPrinterService printer, ITelegramBotService telegramBotService)
    {
        subCommands.Add(new GetChatsCommand(printer, telegramBotService));
        subCommands.Add(new SendMessageCommand(printer, telegramBotService));
    }
}