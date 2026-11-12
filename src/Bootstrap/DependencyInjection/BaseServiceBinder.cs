using Cronos.Infrastructure.CommandProcessor;
using Microsoft.Extensions.Configuration;

namespace Cronos.Bootstrap.DependencyInjection;

public class BaseServiceBinder : BaseBinder
{
    public sealed override void Bind(DIContainer container)
    {
        ICommandHandler commandHandler = container.Get<ICommandHandler>();
        Instantiator instantiator = container.Get<Instantiator>();

        BindOptions(GlobalBinding.Configuration, container);
        BindService(container);
        BindCommands(commandHandler, instantiator);
    }

    public virtual void BindOptions(IConfigurationRoot configurationRoot, DIContainer container) { }
    public virtual void BindService(DIContainer container) { }
    public virtual void BindCommands(ICommandHandler commandHandler, Instantiator instantiator) { }
}