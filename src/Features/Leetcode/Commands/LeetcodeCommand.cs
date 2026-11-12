using Cronos.Infrastructure.BrowserLauncher;
using Cronos.Infrastructure.CommandProcessor.Commands;
using Cronos.Infrastructure.Localization;
using Cronos.Infrastructure.Printer;

namespace Cronos.Features.Leetcode.Commands;

public class LeetcodeCommand : CompositeCommandBase
{
    public override string Name { get; } = "leetcode";
    public override string Description { get; } = "Works with LeetCode web API.";

    public LeetcodeCommand(
        IPrinterService printer,
        ILeetcodeService leetcodeService,
        IBrowserLauncher browser,
        ILocalizationService localization)
    {
        DefaultCommand = new PrintDailyLeetCodeChallenge(printer, leetcodeService, localization);
        
        subCommands.Add(DefaultCommand);
        subCommands.Add(new OpenDailyLeetCodeChallenge(printer, leetcodeService, browser));
    }
}