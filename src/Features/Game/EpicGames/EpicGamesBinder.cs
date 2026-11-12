using Cronos.Bootstrap.DependencyInjection;
using Cronos.Bootstrap.DependencyInjection.Options;
using Cronos.Infrastructure.CommandProcessor;
using Microsoft.Extensions.Configuration;

namespace Cronos.Features.Game.EpicGames;

public class EpicGamesBinder : BaseServiceBinder
{
    public override void BindOptions(IConfigurationRoot configurationRoot, DIContainer container)
    {
        IOptionObserver<EpicGamesOptions> options =
            GetReloadableConfiguration<EpicGamesOptions>(GlobalBinding.Configuration, "epicGames");

        container.RegisterInstance(options);
    }

    public override void BindService(DIContainer container)
    {
        container.Register<IEpicGamesService, EpicGamesService>();
    }

    public override void BindCommands(ICommandHandler commandHandler, Instantiator instantiator)
    {
        commandHandler.TryAddCommand(instantiator.Instantiate<EpicGamesCommand>());
    }
}