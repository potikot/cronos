using System.Text.Json;
using Cronos.Bootstrap.DependencyInjection.Options;
using Cronos.Features.GoogleSheetsService;
using Cronos.Features.GoogleSheetsService.Model;
using Cronos.Features.Settings;
using Cronos.Infrastructure.FileSystem;
using Cronos.Infrastructure.Logger;

namespace Cronos.Infrastructure.Localization;

public class LocalizationService(
    IOptionObserver<ILocalizationOptions> _options,
    ISettingsService _settings,
    ILoggerService _logger,
    IFileService _fileService,
    IGoogleSheetService _googleSheetsService)
    : ILocalizationService
{
    private const string KEY_HEADER = "key";

    private HashSet<string> _supportedLanguages;
    private Dictionary<string, Dictionary<string, string>> _dict;

    public string Language
    {
        get => _settings.Language;
        private set => _settings.Language = value;
    }

    public async Task OnInitializeAsync()
    {
        await ReloadDataAsync();
    }

    private async Task ReloadDataAsync()
    {
        TableData table = await _googleSheetsService.GetSheetAsync(_options.Value.LocalizationSheetId);
        if (table == null)
        {
            await LocalReloadDataAsync();
            return;
        }
        
        _supportedLanguages = new HashSet<string>(table.Headers.Skip(1));

        List<string> keysList = table.GetColumn(KEY_HEADER);
        List<string> localesList = table.GetColumn(Language);

        Dictionary<string, string> locales = new();
        _dict = new Dictionary<string, Dictionary<string, string>> { { Language, locales } };

        for (int i = 0; i < keysList.Count; i++)
            locales.Add(keysList[i], localesList[i]);
    }

    private async Task LocalReloadDataAsync()
    {
        string localizationJson = await _fileService.ReadAsync("localization.dat");
        if (string.IsNullOrEmpty(localizationJson))
        {
            _logger.LogError(this, "Localization file missing: localization.dat.");
            _dict = [];
            return;
        }
        
        _dict = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(localizationJson);
        if (_dict == null)
        {
            _logger.LogError(this, $"Failed to deserialize localization file: localization.dat.");
            _dict = [];
            return;
        }
    
        if (!_dict.ContainsKey(Language))
        {
            string lang = _dict.Keys.FirstOrDefault() ?? "en";
            _logger.LogError(this, $"Fallback language '{Language}' not found. Changed to '{lang}'.");
            Language = lang;
        }
    }

    public async Task<bool> ChangeLanguage(string language)
    {
        if (_supportedLanguages.Contains(language))
        {
            Language = language;
            await ReloadDataAsync();
            return true;
        }
        
        _logger.LogError(this, $"Language not supported: {language}.");
        return false;
    }

    public string Get(string key, string fallback)
    {
        TryGet(key, fallback, out string localization);
        return localization;
    }
    
    public bool TryGet(string key, string fallback, out string localization)
    {
        // todo: costyl
        string loadedLanguage = _dict.Keys.First();
        if (Language != loadedLanguage)
        {
            if (!ChangeLanguage(Language).Result)
                Language = loadedLanguage;
        }
        
        if (_dict.TryGetValue(Language, out Dictionary<string, string> lang)
            && lang.TryGetValue(key, out string value))
        {
            localization = value;
            return true;
        }

        localization = fallback;
        return false;
    }

    public bool ContainsLanguage(string language)
    {
        return _dict.ContainsKey(language);
    }
}