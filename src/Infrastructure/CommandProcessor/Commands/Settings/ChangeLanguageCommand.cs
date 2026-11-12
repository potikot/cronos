using Cronos.Infrastructure.Localization;
using Cronos.Infrastructure.Logger;

namespace Cronos.Infrastructure.CommandProcessor.Commands;

public class ChangeLanguageCommand(
    ILoggerService _logger,
    ILocalizationService _localization) : BaseCommand
{
    public override string Name { get; } = "lang";
    public override string Description { get; } = "Changes the language";

    /// <param name="args">
    /// <br></br> [0] - language code (ru, en etc.)
    /// </param>
    public override async Task<CommandResult> ProcessAsync(string[] args)
    {
        if (args is not { Length: not 0 })
            return CommandResult.Invalid;

        string newLanguage = args[0];
        if (await _localization.ChangeLanguage(newLanguage))
        {
            _logger.LogInfo(this, $"Language changed to '{newLanguage}'.\n");
            return CommandResult.Success;
        }
        
        return CommandResult.Invalid;
    }
}