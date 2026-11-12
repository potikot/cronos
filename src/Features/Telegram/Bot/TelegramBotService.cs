using System.Text.RegularExpressions;
using Cronos.Bootstrap.DependencyInjection.Options;
using Cronos.Consts;
using Cronos.Features.Telegram.Shared;
using Cronos.Infrastructure.Http;
using Cronos.Infrastructure.Input;
using Cronos.Infrastructure.Localization;
using Cronos.Infrastructure.Printer;
using Cronos.Infrastructure.Save;
using Cronos.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cronos.Features.Telegram.Bot;

public class TelegramBotService(
    IOptionObserver<ITelegramOptions> _options,
    IHttpService _httpService,
    IInputService _inputService,
    IPrinterService _printer,
    ISaveService _saveService) : ITelegramBotService
{
    private const string BotTokenKey = "telegram_bot_token";

    private static readonly LocalizedString InvalidTokenLocale =
        new("telegram_invalid_token", "Invalid token validation status");
    public bool IsInitialized { get; private set; }

    public IReadOnlyDictionary<long, TelegramChat> AvailableChats => _availableChats;
    
    private Dictionary<long, TelegramChat> _availableChats = new();

    public async Task OnInitializeAsync()
    {
        _options = new OptionObserver<TelegramOptions>(new TelegramOptions());
        var status = await ValidateTokenAsync(_options.Value.BotToken);
        if (status != BotTokenValidationStatus.Valid)
        {
            string token = _inputService.GetLine("Bot Token: ");
            if (await ValidateTokenAsync(token) != BotTokenValidationStatus.Valid)
            {
                _printer.PrintLine(_printer.TextColor.ToString());
                _printer.PrintLine(InvalidTokenLocale.ToString());
                return;
            }

            _saveService.Set(BotTokenKey, token);
            // _options.Value.BotToken = token;
        }

        IsInitialized = true;
    }

    public async Task<int> SendMessageAsync(long chatId, string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return 0;

        var url = BuildUrl(TelegramApiEndpoints.Bot.SendMessage)
            .WithQueryParameters(("chat_id", chatId), ("text", text), ("parse_mode", "HTML"));

        var response = await _httpService.GetStringAsync(url);
        if (!response.IsSuccess())
        {
            _printer.PrintLine("Not Successful Sending");
            return 0;
        }

        JObject json = JObject.Parse(response.Result);
        int message_id = json["result"]?["message_id"]?.Value<int>() ?? 0;
        return message_id;
    }

    public async Task<Dictionary<long, TelegramChat>> GetUpdatesAsync()
    {
        var url = BuildUrl(TelegramApiEndpoints.Bot.GetUpdates);
        var response = await _httpService.GetStringAsync(url);
        if (!response.IsSuccess())
            return null;

        JObject json = JObject.Parse(response.Result);
        if (json["ok"]?.Value<bool>() != true
            || json["result"] is not JArray updates)
        {
            return null;
        }

        foreach (var updateToken in updates)
        {
            JToken chatToken = updateToken["message"]?["chat"]
                ?? updateToken["edited_message"]?["chat"]
                ?? updateToken["channel_post"]?["chat"]
                ?? updateToken["edited_channel_post"]?["chat"]
                ?? updateToken["my_chat_member"]?["chat"];

            JToken idToken = chatToken?["id"];
            if (idToken is null)
                continue;

            long chatId = idToken.Value<long>();
            if (_availableChats.ContainsKey(chatId))
                continue;

            string title =
                chatToken["title"]?.Value<string>()
                ?? chatToken["username"]?.Value<string>()
                ?? chatToken["first_name"]?.Value<string>()
                ?? chatId.ToString();

            _availableChats[chatId] = new TelegramChat(chatId, title);
        }

        return _availableChats;
    }

    private async Task<BotTokenValidationStatus> ValidateTokenAsync(string token)
    {
        if (string.IsNullOrEmpty(token)
            || !Regex.IsMatch(token, @"\A[0-9]+:[A-Za-z0-9_-]+\z"))
            return BotTokenValidationStatus.Invalid;

        var response = await _httpService.GetStringAsync(BuildUrl(token, TelegramApiEndpoints.Bot.GetMe));
        if (!response.IsSuccess())
            return BotTokenValidationStatus.Unavailable;

        try
        {
            JObject json = JObject.Parse(response.Result);

            return (bool?)json["ok"] == true && (bool?)json["result"]?["is_bot"] == true
                ? BotTokenValidationStatus.Valid
                : BotTokenValidationStatus.Unavailable;
        }
        catch (JsonException)
        {
            return BotTokenValidationStatus.Unavailable;
        }
        catch (HttpRequestException)
        {
            return BotTokenValidationStatus.Unavailable;
        }
    }

    private string BuildUrl(string method)
    {
        return BuildUrl(_saveService.Get<string>(BotTokenKey), method);
    }

    private string BuildUrl(string token, string method)
    {
        return $"{TelegramApiEndpoints.Bot.Api}{token}/{method}";
    }
}