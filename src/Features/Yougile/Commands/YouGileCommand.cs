using Cronos.Infrastructure.CommandProcessor.Commands;
using Cronos.Infrastructure.Printer;

namespace Cronos.Features.Yougile.Commands;

public class YouGileCommand : CompositeCommandBase
{
    public override string Name { get; } = "yougile";
    public override string Description { get; } = "Works with YouGile web API.";

    public YouGileCommand(IPrinterService printer, YougileService yougileService)
    {
        DefaultCommand = new LoginYouGileCommand(yougileService);
        
        subCommands.Add(DefaultCommand);
        subCommands.Add(new AddYouGileTaskCommand(yougileService, printer));
    }
}