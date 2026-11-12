using System.Text.Json;
using Cronos.Consts;
using Cronos.Features.Game.Shared;
using Cronos.Features.Game.Steam.Model;
using Cronos.Features.Game.Steam.Model.ApiResponses;
using Cronos.Infrastructure.Http;
using Cronos.Infrastructure.Http.Model;
using Cronos.Infrastructure.Input;
using Cronos.Infrastructure.Logger;
using Cronos.Utilities;
using HtmlAgilityPack;
using Microsoft.Extensions.Caching.Memory;

namespace Cronos.Features.Game.Steam;

public class SteamService(
    ILoggerService _logger,
    IInputService _inputService,
    IHttpService _httpService) : ISteamService
{
    private const string OwnedGamesCacheKey = "OwnedGames";
    private const string PlayerInfoCacheKey = "PlayerInfo";
    private const string FreeGamesCacheKey = "FreeGames";

    private const int MaxLoginAttempts = 3;

    private static readonly MemoryCache Cache = new(new MemoryCacheOptions());

    private static readonly TimeSpan OwnedGamesExpiration = TimeSpan.FromMinutes(1);
    private static readonly TimeSpan PlayerInfoExpiration = TimeSpan.FromHours(1);
    private static readonly TimeSpan FreeGamesExpiration = TimeSpan.FromHours(6);

    private readonly SteamCredentials _credentials = new();

    public List<BaseGameData> FreeGames { get; private set; } = [];
    public bool IsLoggedIn { get; private set; }

    public Task OnStartAsync()
    {
        RestoreCredentialsCache();
        return Task.CompletedTask;
    }

    public async Task<bool> Login()
    {
        return await LoginInternal(MaxLoginAttempts);
    }

    public async Task<bool> Login(string apiKey, string clientId)
    {
        if (string.IsNullOrWhiteSpace(apiKey)
            || string.IsNullOrWhiteSpace(clientId)
            || !await CheckCredentials(apiKey, clientId))
        {
            _logger.LogError("Invalid credentials provided");
            return false;
        }

        _credentials.ApiKey = apiKey;
        _credentials.ClientId = clientId;
        IsLoggedIn = true;

        if (_inputService.AskYesNo("Save credentials?"))
        {
            // _options.Value.ApiKey = _credentials.ApiKey;
            // _options.Value.ClientId = _credentials.ClientId;
        }

        await RefreshOwnedGamesAsync();
        return true;
    }

    public void Logout()
    {
        _credentials.ApiKey = null;
        _credentials.ClientId = null;
        // _options.Value.ApiKey = null;
        // _options.Value.ClientId = null;
        Cache.Clear();
        IsLoggedIn = false;
    }
    
    public async Task<SteamUserInfo> GetPlayerInfoAsync()
    {
        if (!IsLoggedIn)
        {
            _logger.LogError("Not logged in");
            return null;
        }

        if (Cache.TryGetValue(PlayerInfoCacheKey, out SteamUserInfo cachedInfo))
            return cachedInfo;
        
        string url = SteamApiEndpoints.GetPlayerSummaries.WithQueryParameters(
            ("key", _credentials.ApiKey),
            ("steamids", _credentials.ClientId));

        RequestResult<SAR_GetPlayerSummary> response = await _httpService.GetAsync<SAR_GetPlayerSummary>(url);
        if (!response.IsSuccess())
        {
            _logger.LogError($"Failed to get player info: {response.Error?.Message ?? "Unknown error"}");
            return null;
        }

        SteamUserInfo userInfo = response.Result?.Response?.Players?.FirstOrDefault();
        if (userInfo != null)
            Cache.Set(PlayerInfoCacheKey, userInfo, PlayerInfoExpiration);
        
        return userInfo;
    }
    
    public async Task<List<BaseGameData>> GetFreeGamesAsync()
    {
        if (Cache.TryGetValue(FreeGamesCacheKey, out List<BaseGameData> games))
            return games;
        
        games = [];
        int page = 1;
        const int maxPages = 50;

        while (page <= maxPages)
        {
            string url = SteamApiEndpoints.Search.WithQueryParameters(
                ("maxprice", "free"),
                ("specials", 1),
                ("page", page));
            
            RequestResult<HtmlDocument> response = await _httpService.GetAsync<HtmlDocument>(url);
            if (!response.IsSuccess())
            {
                _logger.LogWarning($"Failed to fetch page {page}: {response.Error?.Message ?? "Unknown error"}");
                break;
            }

            HtmlNodeCollection gameNodes = response.Result?.DocumentNode.SelectNodes("//a[contains(@class,'search_result_row')]");
            if (gameNodes is not { Count: not 0 })
                break;

            foreach (HtmlNode node in gameNodes)
            {
                string appIdStr = node.GetAttributeValue("data-ds-appid", "");
                if (!int.TryParse(appIdStr, out int appId))
                {
                    _logger.LogWarning($"Invalid app ID: {appIdStr}");
                    continue;
                }

                string title = node.SelectSingleNode(".//span[@class='title']")?.InnerText?.Trim() ?? "Unknown";
                string gameUrl = node.GetAttributeValue("href", "");
                string buyGameUrl = SteamApiEndpoints.AddFreeLicense + appId;

                SteamGameData game = new(appId, title, gameUrl, buyGameUrl);
                games.Add(game);
            }

            page++;
        }

        // SteamGameData cs = new(
        //     730,
        //     "Counter Strike 2",
        //     "https://store.steampowered.com/app/730/CounterStrike_2/");
        // games.Add(cs);
        
        FreeGames = games;
        
        if (games.Count != 0)
            Cache.Set(FreeGamesCacheKey, games, FreeGamesExpiration);
        
        return games;
    }

    public async Task<bool> IsGameOwnedAsync(BaseGameData game)
    {
        if (game is not SteamGameData steamGame)
        {
            _logger.LogWarning(this, $"Invalid game data provided: {game?.GetType().Name}");
            return false;
        }
        if (!IsLoggedIn)
        {
            // _logger.LogError("Not logged in");
            return false;
        }
        
        if (steamGame.IsOwned)
            return true;

        if (Cache.TryGetValue(OwnedGamesCacheKey, out HashSet<int> ownedGamesCache))
            return ownedGamesCache.Contains(steamGame.AppId);
        else
        {
            await RefreshOwnedGamesAsync();
            if (Cache.TryGetValue(OwnedGamesCacheKey, out ownedGamesCache))
                return ownedGamesCache.Contains(steamGame.AppId);
        }
        
        return false;
    }

        
    private async Task<bool> LoginInternal(int maxAttempts)
    {
        if (maxAttempts <= 0)
        {
            _logger.LogError("Maximum login attempts exceeded");
            return false;
        }

        string apiKey = _credentials.ApiKey; // ?? _options.Value?.ApiKey;
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            apiKey = _inputService.GetLine("Enter steam developer api key: ");
            if (string.IsNullOrWhiteSpace(apiKey))
                return false;
        }

        string clientId = _credentials.ClientId; // ?? _options.Value?.ClientId;
        if (string.IsNullOrWhiteSpace(clientId))
        {
            clientId = _inputService.GetLine("Enter steam client id: ");
            if (string.IsNullOrWhiteSpace(clientId))
                return false;
        }

        if (await CheckCredentials(apiKey, clientId))
        {
            _credentials.ApiKey = apiKey;
            _credentials.ClientId = clientId;

            if (_inputService.AskYesNo("Save credentials?"))
            {
                // _options.Value.ApiKey = _credentials.ApiKey;
                // _options.Value.ClientId =  _credentials.ClientId;
            }
        
            IsLoggedIn = true;
            await RefreshOwnedGamesAsync();
            return true;
        }
        else
        {
            _logger.LogWarning("Invalid credentials provided");
            // if ((!string.IsNullOrWhiteSpace(_options.Value?.ApiKey) || !string.IsNullOrWhiteSpace(_options.Value?.ClientId))
            //     && _inputService.AskYesNo("Clear credentials?"))
            // {
            //     _options.Value.ApiKey = null;
            //     _options.Value.ClientId = null;
            // }
            if (_inputService.AskYesNo("Retry login?"))
                return await LoginInternal(maxAttempts - 1);
        }

        return false;
    }
    
    private async Task RefreshOwnedGamesAsync()
    {
        string url = SteamApiEndpoints.GetOwnedGames.WithQueryParameters(
            ("key", _credentials.ApiKey),
            ("steamid", _credentials.ClientId),
            ("include_played_free_games", 1));

        RequestResult<JsonDocument> response = await _httpService.GetAsync<JsonDocument>(url);
        if (!response.IsSuccess())
        {
            _logger.LogError($"Failed to get owned games: {response.Error.Message ?? "Unknown error"}");
            return;
        }

        JsonElement? gamesElement = response.Result.RootElement
            .GetPropertyOrNull("response")
            ?.GetPropertyOrNull("games");

        if (gamesElement == null)
        {
            _logger.LogError("Failed to get 'response/games' json element");
            return;
        }
        
        HashSet<int> ownedGames = Cache.Set(OwnedGamesCacheKey, new HashSet<int>(), OwnedGamesExpiration);
        foreach (JsonElement game in gamesElement.Value.EnumerateArray())
        {
            if (game.TryGetProperty("appid", out JsonElement appIdElement)
                && appIdElement.TryGetInt32(out int appId))
            {
                ownedGames.Add(appId);
            }
        }
        
        _logger.LogInfo($"Owned games cache updated: {ownedGames.Count} games");
    }
    
    private void RestoreCredentialsCache()
    {
        // if (string.IsNullOrWhiteSpace(_options.Value?.ApiKey)
        //     || string.IsNullOrWhiteSpace(_options.Value?.ClientId))
        //     return;
        //
        // IsLoggedIn = true;
        // _credentials.ApiKey = _options.Value.ApiKey;
        // _credentials.ClientId = _options.Value.ClientId;
    }
    
    private async Task<bool> CheckCredentials(string apiKey, string clientId)
    {
        try
        {
            string url = SteamApiEndpoints.GetPlayerSummaries.WithQueryParameters(
                ("key", apiKey),
                ("steamids", clientId));

            RequestResult<SAR_GetPlayerSummary> response = await _httpService.GetAsync<SAR_GetPlayerSummary>(url);
            if (!response.IsSuccess())
            {
                _logger.LogError($"Credentials validation failed: {response.Error?.Message ?? "Unknown error"}");
                return false;
            }

            return response.Result?.Response?.Players?.Any() == true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception in CheckCredentials: {ex.Message}");
            return false;
        }
    }
}