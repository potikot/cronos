using Cronos.Infrastructure.CommandProcessor;
using Cronos.Infrastructure.CommandProcessor.Commands;

namespace Cronos.Features.Yougile.Commands;

public class LoginYouGileCommand(YougileService yougileService) : BaseCommand
{
    public override string Name { get; } = "login";
    public override string Description { get; } = "Login to YouGile";

    public override async Task<CommandResult> ProcessAsync(string[] args)
    {
        if (!TryParseArguments(args, out CommandArguments arguments))
            return CommandResult.Invalid;

        bool loggedIn = await yougileService.Login(arguments.Arguments[0]);
        return loggedIn ? CommandResult.Success : CommandResult.Invalid;
    }
}