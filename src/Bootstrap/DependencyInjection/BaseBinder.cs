using Cronos.Bootstrap.DependencyInjection.Options;
using Microsoft.Extensions.Configuration;

namespace Cronos.Bootstrap.DependencyInjection;

public abstract class BaseBinder
{
    // private static int Count;
    
    public abstract void Bind(DIContainer container);

    static protected IOptionObserver<T> GetReloadableConfiguration<T>(IConfigurationRoot root, string sectionName)
    {
        IConfigurationSection section = root.GetSection(sectionName);
        OptionObserver<T> options = new(section.Get<T>());
        // Console.WriteLine($"GetReloadableConfiguration - {Count}");

        IDisposable callbackDisposable = Register();
        return options;
        
        // todo: change callback called twice per change
        IDisposable Register()
        {
            return section
                .GetReloadToken()
                .RegisterChangeCallback(_ =>
                {
                    options.Value = section.Get<T>();
                    
                    // var clr = Console.ForegroundColor;
                    // Console.ForegroundColor = ConsoleColor.Green;
                    // Console.WriteLine($"{++Count}: updated options");
                    // Console.ForegroundColor = clr;
                    
                    Register();
                }, null);
        }
    }
}