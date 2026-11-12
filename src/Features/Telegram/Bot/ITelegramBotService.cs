using Cronos.Bootstrap.Lifecylcle;
using Cronos.Features.Telegram.Shared;

namespace Cronos.Features.Telegram.Bot;

public interface ITelegramBotService : IOnInitializeAsync
{
    bool IsInitialized { get; }

    Task<Dictionary<long, TelegramChat>> GetUpdatesAsync();
    Task<int> SendMessageAsync(long chatId, string text);
}