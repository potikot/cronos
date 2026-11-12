using Cronos.Features.Settings;
using Cronos.Infrastructure.CommandProcessor;
using Cronos.Infrastructure.CommandProcessor.Commands;
using Cronos.Infrastructure.Converter;
using Cronos.Infrastructure.FileSystem;
using Cronos.Infrastructure.Localization;
using Cronos.Infrastructure.Logger;
using Cronos.Infrastructure.Printer;

namespace Cronos.Bootstrap.DependencyInjection.Binders;

[BindOrder(-10)]
public class CommandHandlerBinder : BaseBinder
{
    public override void Bind(DIContainer container)
    {
        ILoggerService logger = container.Get<ILoggerService>();
        IPrinterService printer = container.Get<IPrinterService>();
        IFileService fileService = container.Get<IFileService>();
        ISettingsService settings = container.Get<ISettingsService>();
        ILocalizationService localization = container.Get<ILocalizationService>();
        IConverterService converter = container.Get<IConverterService>();

        ICommandHandler commandHandler = new CommandHandler(logger);

        commandHandler.TryAddCommand(new HelpCommand(printer, commandHandler));
        commandHandler.TryAddCommand(new ExitCommand());
        commandHandler.TryAddCommand(new SettingsCommand(logger, localization, settings, converter));

        commandHandler.TryAddCommand(new WriteFileCommand(logger, fileService));

        commandHandler.TryAddCommand(new PrintCommand(printer));

        container.RegisterInstance(commandHandler);
    }
}