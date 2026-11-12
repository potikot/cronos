using Cronos.Bootstrap.Lifecylcle;

namespace Cronos.Infrastructure.Localization;

public interface ILocalizationService : IOnInitializeAsync
{
    string Language { get; }
    
    Task<bool> ChangeLanguage(string language);
    
    string Get(string key, string fallback);
    bool TryGet(string key, string fallback, out string localization);
    
    bool ContainsLanguage(string language);
}