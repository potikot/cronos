using Cronos.Bootstrap.DependencyInjection;
using Cronos.Features.Leetcode.Commands;
using Cronos.Infrastructure.CommandProcessor;

namespace Cronos.Features.Leetcode;

public class LeetcodeBinder : BaseServiceBinder
{
    public override void BindService(DIContainer container)
    {
        container.Register<ILeetcodeService, LeetcodeService>();
    }

    public override void BindCommands(ICommandHandler commandHandler, Instantiator instantiator)
    {
        commandHandler.TryAddCommand(instantiator.Instantiate<LeetcodeCommand>());
    }
}