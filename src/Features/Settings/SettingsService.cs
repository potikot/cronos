using Cronos.Bootstrap.DependencyInjection.Options;
using Cronos.Infrastructure.CommandProcessor.Commands;
using Cronos.Infrastructure.Dashboard;
using Cronos.Infrastructure.Localization;
using Cronos.Infrastructure.Save;

namespace Cronos.Features.Settings;

public class SettingsService : ISettingsService
{
    private readonly ISaveService _saveService;
    private readonly IOptionObserver<IDashboardOptions> _dashboardOptions;
    private readonly IOptionObserver<ILocalizationOptions> _localizationOptions;

    private bool? _printSteamGamesOnStartup;
    private bool? _printEpicGamesOnStartup;
    private bool? _printLeetCodeChallengeOnStartup;

    public SettingsService(
        IOptionObserver<IDashboardOptions> dashboardOptions,
        IOptionObserver<ILocalizationOptions> localizationOptions,
        ISaveService saveService)
    {
        _dashboardOptions = dashboardOptions;
        _localizationOptions = localizationOptions;
        _saveService = saveService;
    }

    [Setting("dashboard", "Print dashboard on startup")]
    public bool ShowDashboardOnStartup { get; set; }
    // {
    //     set => _dashboardOptions.Value.PrintOnStart = value;
    //     get => _dashboardOptions.Value.PrintOnStart;
    // }
    
    [Setting("steam", "Print free steam games in dashboard")]
    public bool PrintSteamGamesOnStartup
    {
        set
        {
            _printSteamGamesOnStartup = value;
            _saveService.Set(SaveKeys.PrintSteamGamesOnStartup, value);
        }
        get => _printSteamGamesOnStartup
            ??= _saveService.Get(SaveKeys.PrintSteamGamesOnStartup, true);
    }
    
    [Setting("epic", "Print free epic games in dashboard")]
    public bool PrintEpicGamesOnStartup
    {
        set
        {
            _printEpicGamesOnStartup = value;
            _saveService.Set(SaveKeys.PrintEpicGamesOnStartup, value);
        }
        get => _printEpicGamesOnStartup
            ??= _saveService.Get(SaveKeys.PrintEpicGamesOnStartup, true);
    }
    
    [Setting("leetcode", "Print leetcode challenge in dashboard")]
    public bool PrintLeetCodeChallengeOnStartup
    {
        set
        {
            _printLeetCodeChallengeOnStartup = value;
            _saveService.Set(SaveKeys.PrintLeetCodeChallengeOnStartup, value);
        }
        get => _printLeetCodeChallengeOnStartup
            ??= _saveService.Get(SaveKeys.PrintLeetCodeChallengeOnStartup, true);
    }

    public string Language { get; set; } = "en";
    // {
    //     set => _localizationOptions.Value.Language = value;
    //     get => _localizationOptions.Value.Language;
    // }

    [Setting("reset", "Reset settings")]
    public void Reset()
    {
        ShowDashboardOnStartup = _dashboardOptions.Value.PrintOnStart;
        PrintSteamGamesOnStartup = true;
        PrintEpicGamesOnStartup = true;
        PrintLeetCodeChallengeOnStartup = true;

        Language = _localizationOptions.Value.Language;
    }

    public static class SaveKeys
    {
        public const string ShowDashboardOnStartup = "PrintSteamGamesOnStartup";
        public const string PrintSteamGamesOnStartup = "PrintSteamGamesOnStartup";
        public const string PrintEpicGamesOnStartup = "PrintEpicGamesOnStartup";
        public const string PrintLeetCodeChallengeOnStartup = "PrintLeetCodeChallengeOnStartup";
        
        public const string Language = "lang";
    }
}