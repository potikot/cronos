using Cronos.Features.GoogleSheetsService;
using Cronos.Infrastructure.BrowserLauncher;
using Cronos.Infrastructure.Input;
using Cronos.Infrastructure.Localization;

namespace Cronos.Bootstrap.DependencyInjection.Binders;

[BindOrder(-100)]
public class ExternalAPIServicesBinder : BaseBinder
{
    public override void Bind(DIContainer container)
    {
        container
            .Register<IGoogleSheetService, GoogleSheetService>()
            .Register<ILocalizationService, LocalizationService>()
            .Register<IInputService, ConsoleInputService>()
            .Register<IBrowserLauncher, UniversalBrowserLauncher>();
    }
}