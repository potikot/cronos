using Cronos.Bootstrap.DependencyInjection;

namespace Cronos.Infrastructure.Localization;

public class LocalizedString
{
    private static ILocalizationService Service;

    public string Key { get; }
    public string Fallback { get; }

    public ILocalizationService LocalizationService => Service;
    
    static LocalizedString()
    {
        DIContainer container = GlobalContext.Container;
        if (!container.TryGet(out Service))
            container.NotifyWhenServiceRegistered<ILocalizationService>(service => Service = service);
    }

    public LocalizedString(string key, string fallback)
    {
        Key = key;
        Fallback = fallback;
    }

    public override string ToString()
    {
        return Service == null ? Fallback : Service.Get(Key, Fallback);
    }
}