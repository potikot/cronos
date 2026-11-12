using Cronos.Bootstrap.DependencyInjection.Options;
using Cronos.Infrastructure.Printer;

namespace Cronos.Infrastructure.Logger;

public class ConsoleLoggerService : ILoggerService
{
    private readonly IPrinterService _printer;
    
    private readonly PrintOptions _logPrintOptions;
    private readonly PrintOptions _warningPrintOptions;
    private readonly PrintOptions _errorPrintOptions;

    public ConsoleLoggerService(IOptionObserver<ILoggerOptions> options, IPrinterService printer)
    {
        _printer = printer;
        _logPrintOptions = new PrintOptions() { TextColor = options.Value.LogColor };
        _warningPrintOptions = new PrintOptions() { TextColor = options.Value.WarningColor };
        _errorPrintOptions = new PrintOptions() { TextColor = options.Value.ErrorColor };
    }

    public void LogInfo(object message) => _printer.PrintLine(message.ToString(), _logPrintOptions);
    public void LogInfo(object sender, object message) => LogInfo(sender == null ? message : $"[{sender.GetType().Name}] {message}");
    
    public void LogWarning(object message) => _printer.PrintLine(message.ToString(), _warningPrintOptions);
    public void LogWarning(object sender, object message) => LogWarning(sender == null ? message : $"[{sender.GetType().Name}] {message}");

    public void LogError(object message) => _printer.PrintLine(message.ToString(), _errorPrintOptions);
    public void LogError(object sender, object message) => LogError(sender == null ? message : $"[{sender.GetType().Name}] {message}");
}