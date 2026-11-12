using Cronos.Bootstrap.DependencyInjection;
using Cronos.Bootstrap.DependencyInjection.Options;
using Cronos.Features.Yougile.Commands;
using Cronos.Infrastructure.CommandProcessor;
using Microsoft.Extensions.Configuration;

namespace Cronos.Features.Yougile;

public class YougileBinder : BaseServiceBinder
{
    public override void BindOptions(IConfigurationRoot configurationRoot, DIContainer container)
    {
        IOptionObserver<IYougileOptions> options =
            GetReloadableConfiguration<YougileOptions>(configurationRoot, "yougile");

        container.RegisterInstance(options);
    }

    public override void BindService(DIContainer container)
    {
        container.Register<YougileService>();
    }

    public override void BindCommands(ICommandHandler commandHandler, Instantiator instantiator)
    {
        commandHandler.TryAddCommand(instantiator.Instantiate<YouGileCommand>());
    }
}