using Cronos.Bootstrap.DependencyInjection;
using Cronos.Bootstrap.DependencyInjection.Options;
using Cronos.Infrastructure.CommandProcessor;
using Microsoft.Extensions.Configuration;

namespace Cronos.Features.Game.Steam;

public class SteamBinder : BaseServiceBinder
{
    public override void BindOptions(IConfigurationRoot configurationRoot, DIContainer container)
    {
        IOptionObserver<SteamOptions> options =
            GetReloadableConfiguration<SteamOptions>(GlobalBinding.Configuration, "steam");

        container.RegisterInstance(options);
    }

    public override void BindService(DIContainer container)
    {
        container.Register<ISteamService, SteamService>();
    }

    public override void BindCommands(ICommandHandler commandHandler, Instantiator instantiator)
    {
        commandHandler.TryAddCommand(instantiator.Instantiate<SteamCommand>());
    }
}