namespace Cronos.Infrastructure.Localization;

public interface ILocalizationOptions
{
    string LocalizationSheetId { get; }
    string Language { get; }
    string FallbackLanguage { get; }
}

public class LocalizationOptions : ILocalizationOptions
{
    public string LocalizationSheetId { get; set; }
    public string Language { get; set; }
    public string FallbackLanguage { get; set; }
}