using Cronos.Features.Leetcode.Model;
using Cronos.Infrastructure.BrowserLauncher;
using Cronos.Infrastructure.CommandProcessor;
using Cronos.Infrastructure.CommandProcessor.Commands;
using Cronos.Infrastructure.Printer;

namespace Cronos.Features.Leetcode.Commands;

public class OpenDailyLeetCodeChallenge(
    IPrinterService _printer,
    ILeetcodeService leetcodeService,
    IBrowserLauncher _browser) : BaseCommand
{
    private static readonly PrintOptions SuccessOpenPrintOptions = new PrintOptions()
    {
        TextColor = ConsoleColor.Green
    };
    
    public override string Name { get; } = "open";
    public override string Description { get; } = "Opens daily LeetCode challenge";

    /// <param name="args"> null </param>
    public override async Task<CommandResult> ProcessAsync(string[] args)
    {
        LeetcodeChallengeData challenge = await leetcodeService.GetDailyChallengeAsync();
        _browser.Open(challenge.Url);
        
        _printer.PrintLine("Opened daily leet code challenge...", SuccessOpenPrintOptions);
        _printer.PrintLine();
        
        return CommandResult.Success;
    }
}