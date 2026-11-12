using System.Text;
using Cronos.Infrastructure.FileSystem;
using Cronos.Infrastructure.Localization;
using Cronos.Infrastructure.Logger;

namespace Cronos.Infrastructure.CommandProcessor.Commands;

public class WriteFileCommand(
    ILoggerService _logger,
    IFileService _fileService) : BaseCommand
{
    private static readonly LocalizedString SuccessMessageLocale = new("write-file_succes", "Wrote file.");
    private static readonly LocalizedString ErrorMessageLocale = new("write-file_succes", "Failed to write file.");
    
    public override string Name { get; } = "write";
    public override string Description { get; } = "Writes file with the given text";

    /// <param name="args">
    /// <br></br> [0] - path to file (<see cref="string"/>)
    /// <br></br> [1..length] - text (<see cref="string"/>)
    /// </param>
    public override async Task<CommandResult> ProcessAsync(string[] args)
    {
        if (args is { Length: < 2 })
            return CommandResult.Invalid;

        string path = args[0];
        StringBuilder content = new StringBuilder().AppendJoin(' ', args[1..]);

        if (await _fileService.WriteAsync(path, content.ToString()))
            _logger.LogInfo(SuccessMessageLocale);
        else
            _logger.LogError(ErrorMessageLocale);

        return CommandResult.Success;
    }
}