namespace Cronos.Features.Telegram;

public interface ITelegramOptions
{
    public string BotToken { get; }
}

public class TelegramOptions : ITelegramOptions
{
    public string BotToken { get; set; }
}