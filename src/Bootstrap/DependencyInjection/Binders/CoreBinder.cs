using Cronos.Bootstrap.DependencyInjection.Options;
using Cronos.Features.Settings;
using Cronos.Infrastructure.Converter;
using Cronos.Infrastructure.Dashboard;
using Cronos.Infrastructure.EncryptionService;
using Cronos.Infrastructure.FileSystem;
using Cronos.Infrastructure.Localization;
using Cronos.Infrastructure.Logger;
using Cronos.Infrastructure.Printer;
using Cronos.Infrastructure.Save;
using Cronos.Infrastructure.StringFormatter;

namespace Cronos.Bootstrap.DependencyInjection.Binders;

[BindOrder(-10000)]
public class CoreBinder : BaseBinder
{
    public override void Bind(DIContainer container)
    {
        BindConfiguration(container);
        BindServices(container);
    }

    private void BindConfiguration(DIContainer container)
    {
        IOptionObserver<IDashboardOptions> dashboardOptions =
            GetReloadableConfiguration<DashboardOptions>(GlobalBinding.Configuration, "dashboard");
        IOptionObserver<ILoggerOptions> loggerOptions =
            GetReloadableConfiguration<LoggerOptions>(GlobalBinding.Configuration, "logger");
        IOptionObserver<ILocalizationOptions> localizationOptions =
            GetReloadableConfiguration<LocalizationOptions>(GlobalBinding.Configuration, "localization");
        IOptionObserver<ISaveOptions> saveOptions =
            GetReloadableConfiguration<SaveOptions>(GlobalBinding.Configuration, "save");

        container
            .RegisterInstance(dashboardOptions)
            .RegisterInstance(loggerOptions)
            .RegisterInstance(localizationOptions)
            .RegisterInstance(saveOptions);
    }

    private void BindServices(DIContainer container)
    {
        container
            .Register<IStringFormatterService, StringFormatterService>()
            .Register<IPrinterService, ConsolePrinterService>()
            .Register<ILoggerService, ConsoleLoggerService>()
            .Register<IFileService, FileService>()
            .Register<IConverterService, ConverterService>()
            .Register<IEncryptionService, Base64EncryptionService>()
            .Register<ISaveService, EncryptedSaveService>()
            .Register<ISettingsService, SettingsService>();
    }
}