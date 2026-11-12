using System.Text.Json;
using Cronos.Features.Game.EpicGames.Model;
using Cronos.Features.Game.Shared;
using Cronos.Infrastructure.Http;
using Cronos.Infrastructure.Http.Model;
using Cronos.Infrastructure.Input;
using Cronos.Infrastructure.Logger;
using Cronos.Consts;
using Cronos.Utilities;
// using Epic.OnlineServices;
// using Epic.OnlineServices.Auth;
// using Epic.OnlineServices.Ecom;
// using Epic.OnlineServices.Platform;

namespace Cronos.Features.Game.EpicGames;

public class EpicGamesService(
    ILoggerService _logger,
    IInputService _inputService,
    IHttpService _httpService) : IEpicGamesService
{
    // private PlatformInterface _platform;
    // private EpicAccountId _epicAccountId;
    private bool _eosInitialized;

    public List<BaseGameData> FreeGames { get; private set; } = [];
    public bool IsLoggedIn { get; private set; }

    public async Task<bool> Login()
    {
        return false;
        
        if (IsLoggedIn)
            return true;

        // Load or request EOS credentials
        string productId = ""; //_options.Value.ProductId;
        string sandboxId = ""; //_options.Value.SandboxId;
        string deploymentId = ""; //_options.Value.DeploymentId;
        string clientId = ""; //_options.Value.ClientId;
        string clientSecret = ""; //_options.Value.ClientSecret;

        if (string.IsNullOrWhiteSpace(productId))
            productId = _inputService.GetLine("Enter Epic ProductId: ");
        if (string.IsNullOrWhiteSpace(sandboxId))
            sandboxId = _inputService.GetLine("Enter Epic SandboxId: ");
        if (string.IsNullOrWhiteSpace(deploymentId))
            deploymentId = _inputService.GetLine("Enter Epic DeploymentId: ");
        if (string.IsNullOrWhiteSpace(clientId))
            clientId = _inputService.GetLine("Enter Epic ClientId: ");
        if (string.IsNullOrWhiteSpace(clientSecret))
            clientSecret = _inputService.GetLine("Enter Epic ClientSecret: ");

        if (string.IsNullOrWhiteSpace(productId) || string.IsNullOrWhiteSpace(sandboxId)
            || string.IsNullOrWhiteSpace(deploymentId) || string.IsNullOrWhiteSpace(clientId)
            || string.IsNullOrWhiteSpace(clientSecret))
        {
            _logger.LogError("Invalid Epic Games SDK credentials.");
            return false;
        }

        // Initialize EOS Platform
        // try
        // {
        //     if (!_eosInitialized)
        //     {
        //         InitializeOptions initOptions = new()
        //         {
        //             ProductName = GlobalConstants.ProductName,
        //             ProductVersion = GlobalConstants.ProductVersion
        //         };
        //         Result initResult = PlatformInterface.Initialize(ref initOptions);
        //         if (initResult != Result.Success && initResult != Result.AlreadyConfigured)
        //         {
        //             _logger.LogError($"EOS Initialize failed: {initResult}");
        //             return false;
        //         }
        //         _eosInitialized = true;
        //     }
        //
        //     WindowsOptions platformOptions = new()
        //     {
        //         ProductId = productId,
        //         SandboxId = sandboxId,
        //         DeploymentId = deploymentId,
        //         ClientCredentials = new ClientCredentials
        //         {
        //             ClientId = clientId,
        //             ClientSecret = clientSecret
        //         },
        //         CacheDirectory = AppContext.BaseDirectory,
        //         IsServer = false,
        //         TickBudgetInMilliseconds = 0
        //     };
        //
        //     _platform = PlatformInterface.Create(ref platformOptions);
        //     if (_platform == null)
        //     {
        //         _logger.LogError("EOS Platform creation returned null. Check credentials and native DLL availability.");
        //         return false;
        //     }
        // }
        // catch (Exception ex)
        // {
        //     _logger.LogError($"EOS Platform init failed: {ex.Message}");
        //     return false;
        // }

        // Save credentials if user wants
        if (_inputService.AskYesNo("Save credentials?"))
        {
            // _options.Value.ProductId = productId;
            // _options.Value.SandboxId = sandboxId;
            // _options.Value.DeploymentId = deploymentId;
            // _options.Value.ClientId = clientId;
            // _options.Value.ClientSecret = clientSecret;
        }

        // Authenticate via Account Portal (opens system browser)
        // AuthInterface auth = _platform.GetAuthInterface();
        // LoginOptions loginOptions = new()
        // {
        //     Credentials = new Credentials
        //     {
        //         Type = LoginCredentialType.AccountPortal
        //     },
        //     ScopeFlags = AuthScopeFlags.BasicProfile | AuthScopeFlags.FriendsList | AuthScopeFlags.Presence
        // };

        // bool loginCompleted = false;
        // bool loginSucceeded = false;

        // auth.Login(ref loginOptions, null, (ref LoginCallbackInfo info) =>
        // {
        //     if (info.ResultCode == Result.Success || info.ResultCode == Result.AuthAccountFeatureRestricted)
        //     {
        //         _epicAccountId = info.LocalUserId;
        //         IsLoggedIn = true;
        //         loginSucceeded = true;
        //     }
        //     else
        //     {
        //         _logger.LogError($"EOS Login failed: {info.ResultCode}");
        //     }
        //     loginCompleted = true;
        // });

        // Pump platform until callback fires
        // DateTime start = DateTime.UtcNow;
        // while (!loginCompleted && (DateTime.UtcNow - start).TotalSeconds < 300)
        // {
        //     _platform?.Tick();
        //     await Task.Delay(50);
        // }
        //
        // if (!loginCompleted)
        // {
        //     _logger.LogError("EOS Login timed out.");
        //     return false;
        // }
        //
        // return loginSucceeded;
        return false;
    }

    public void Logout()
    {
        return;
        // try
        // {
        //     if (_platform != null && _epicAccountId != null)
        //     {
        //         AuthInterface auth = _platform.GetAuthInterface();
        //         LogoutOptions options = new() { LocalUserId = _epicAccountId };
        //         auth.Logout(ref options, null, (ref LogoutCallbackInfo _) => { });
        //     }
        // }
        // catch (Exception ex)
        // {
        //     _logger.LogError($"EOS Logout error: {ex.Message}");
        // }
        // finally
        // {
        //     IsLoggedIn = false;
        //     _epicAccountId = null;
        //     _platform?.Release();
        //     _platform = null;
        //     if (_eosInitialized)
        //     {
        //         PlatformInterface.Shutdown();
        //         _eosInitialized = false;
        //     }
        // }
    }

    public async Task<List<BaseGameData>> GetFreeGamesAsync()
    {
        string url = EpicGamesApiEndpoints.FreeGamesPromotions.WithQueryParameters(
            ("locale", "ru"),
            ("country", "RU"),
            ("allowCountries", "RU"));
        
        List<BaseGameData> games = [];
        RequestResult<JsonDocument> response = await _httpService.GetAsync<JsonDocument>(url);
        if (!response.IsSuccess() || response.Result == null)
            return games;
        
        if (!response.Result.RootElement.TryGetProperty("data", out JsonElement dataElement)
            || !dataElement.TryGetProperty("Catalog", out JsonElement catalogElement)
            || !catalogElement.TryGetProperty("searchStore", out JsonElement searchStoreElement)
            || !searchStoreElement.TryGetProperty("elements", out JsonElement elementsElement))
            return games;

        foreach (JsonElement element in elementsElement.EnumerateArray())
        {
            if (!element.TryGetProperty("promotions", out JsonElement promotionsElement))
                continue;
            if (promotionsElement.ValueKind != JsonValueKind.Object
                || !promotionsElement.TryGetProperty("promotionalOffers", out JsonElement promotionalOffersElement)
                || promotionalOffersElement.GetArrayLength() == 0)
                continue;
            if (!element.TryGetProperty("title", out JsonElement titleElement))
                continue;

            string title = titleElement.GetString();
            string productSlug = element.GetPropertyOrNull("productSlug")?.GetString();
            if (string.IsNullOrEmpty(productSlug)
                && element.TryGetProperty("catalogNs", out JsonElement catalogNsElement)
                && catalogNsElement.TryGetProperty("mappings", out JsonElement mappingsElement)
                && mappingsElement.GetArrayLength() > 0)
            {
                productSlug = mappingsElement[0].GetProperty("pageSlug").GetString();
            }

            if (string.IsNullOrEmpty(productSlug))
                continue;

            EpicGameData game = new(title, EpicGamesApiEndpoints.Game + productSlug);
            game.IsOwned = await IsGameOwnedAsync(game);
            games.Add(game);
        }

        FreeGames = games;
        return games;
    }

    public async Task<bool> IsGameOwnedAsync(BaseGameData game)
    {
        if (game is not EpicGameData epicGame)
        {
            _logger.LogWarning(this, $"Invalid game data provided: {game?.GetType().Name}");
            return false;
        }
        
        return false;
        // if (!IsLoggedIn || _platform == null || _epicAccountId == null)
        // {
        //     return false;
        // }
        //
        // // Without catalog item identifiers, we can only query all entitlements and try a best-effort match.
        // // This is a heuristic and may not be accurate for all titles.
        // EcomInterface ecom = _platform.GetEcomInterface();
        // QueryEntitlementsOptions qeo = new()
        // {
        //     LocalUserId = _epicAccountId,
        //     EntitlementNames = Array.Empty<Utf8String>(),
        //     IncludeRedeemed = true
        // };
        //
        // bool done = false;
        // bool owned = false;
        // ecom.QueryEntitlements(ref qeo, null, (ref QueryEntitlementsCallbackInfo info) =>
        // {
        //     if (info.ResultCode != Result.Success)
        //     {
        //         done = true;
        //         return;
        //     }
        //
        //     CopyEntitlementByIndexOptions ceOpts = new() { LocalUserId = _epicAccountId };
        //     // After a successful query, entitlements are cached. Use GetEntitlementsCount to iterate.
        //     GetEntitlementsCountOptions cntOpts = new() { LocalUserId = _epicAccountId };
        //     uint count = ecom.GetEntitlementsCount(ref cntOpts);
        //     for (uint i = 0; i < count; i++)
        //     {
        //         ceOpts.EntitlementIndex = i;
        //         Result rc = ecom.CopyEntitlementByIndex(ref ceOpts, out Entitlement? entitlement);
        //         if (rc == Result.Success)
        //         {
        //             string entitlementName = entitlement?.EntitlementName;
        //             if (!string.IsNullOrWhiteSpace(entitlementName) && !string.IsNullOrWhiteSpace(game.Title))
        //             {
        //                 if (string.Equals(entitlementName, game.Title, StringComparison.OrdinalIgnoreCase)
        //                     || entitlementName.Contains(game.Title, StringComparison.OrdinalIgnoreCase)
        //                     || game.Title.Contains(entitlementName, StringComparison.OrdinalIgnoreCase))
        //                 {
        //                     owned = true;
        //                     break;
        //                 }
        //             }
        //         }
        //     }
        //     done = true;
        // });
        //
        // // Pump until callback completes
        // DateTime start = DateTime.UtcNow;
        // while (!done && (DateTime.UtcNow - start).TotalSeconds < 60)
        // {
        //     _platform?.Tick();
        //     await Task.Delay(25);
        // }

        // return owned;
        return false;
    }
}