using Cronos.Features.Leetcode.Model;
using Cronos.Infrastructure.CommandProcessor;
using Cronos.Infrastructure.CommandProcessor.Commands;
using Cronos.Infrastructure.Localization;
using Cronos.Infrastructure.Printer;
using Cronos.Infrastructure.StringFormatter.Model;

namespace Cronos.Features.Leetcode.Commands;

public class PrintDailyLeetCodeChallenge(
    IPrinterService _printer,
    ILeetcodeService leetcodeService,
    ILocalizationService _localization) : BaseCommand
{
    private static readonly LocalizedString TitleLocale = new("leetcode_daily_challenge", "Today's LeetCode Challenge:");
    private static readonly LocalizedString ChallengeLocale = new("leetcode_challenge", "{title} ([$diffColor]{difficulty}[/]) | Url: {url}");

    private static readonly Dictionary<LeetcodeChallengeDifficulty, ConsoleColor> DifficultyColors = new()
    {
        { LeetcodeChallengeDifficulty.None, ConsoleColor.Magenta },
        { LeetcodeChallengeDifficulty.Easy, ConsoleColor.Green },
        { LeetcodeChallengeDifficulty.Medium, ConsoleColor.Yellow },
        { LeetcodeChallengeDifficulty.Hard, ConsoleColor.Red }
    };
    
    private static readonly FormatterParams ChallengeParams = new()
    {
        { "title", "" },
        { "difficulty", LeetcodeChallengeDifficulty.Easy },
        { "diffColor",  DifficultyColors[LeetcodeChallengeDifficulty.Easy] },
        { "url", "" }
    };
    
    public override string Name { get; } = "print";
    public override string Description { get; } = "Prints daily LeetCode challenge.";

    // todo: show if beaten and extract to command
    public override async Task<CommandResult> ProcessAsync(string[] args)
    {
        TryParseArguments(args, out CommandArguments arguments);
        LeetcodeChallengeData challenge = await leetcodeService.GetDailyChallengeAsync();
        string difficultyString = challenge.Difficulty.ToString();
        
        ChallengeParams["title"] = challenge.Title;
        ChallengeParams["difficulty"] = _localization.Get("leetcode_difficulty_" + difficultyString.ToLower(), difficultyString);
        ChallengeParams["diffColor"] = DifficultyColors[challenge.Difficulty];
        ChallengeParams["url"] = challenge.Url;

        if (arguments.HasOption("-t"))
            _printer.PrintLine(TitleLocale.ToString());

        _printer.PrintLine(ChallengeLocale.ToString(), new PrintOptions()
        {
            FormatterParams = ChallengeParams
        });
        _printer.PrintLine();
        
        return CommandResult.Success;
    }
}