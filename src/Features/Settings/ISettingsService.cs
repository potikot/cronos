namespace Cronos.Features.Settings;

public interface ISettingsService
{
    bool ShowDashboardOnStartup { set; get; }
    bool PrintSteamGamesOnStartup { set; get; }
    bool PrintEpicGamesOnStartup { set; get; }
    bool PrintLeetCodeChallengeOnStartup { set; get; }
    
    string Language { set; get; }
    
    void Reset();
}