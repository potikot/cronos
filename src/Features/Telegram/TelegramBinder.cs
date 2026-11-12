using Cronos.Bootstrap.DependencyInjection;
using Cronos.Bootstrap.DependencyInjection.Options;
using Cronos.Features.Telegram.Bot;

namespace Cronos.Features.Telegram;

public class TelegramBinder : BaseBinder
{
    public override void Bind(DIContainer container)
    {
        IOptionObserver<ITelegramOptions> options =
            GetReloadableConfiguration<TelegramOptions>(GlobalBinding.Configuration, "telegram");

        container
            .RegisterInstance(options)
            .Register<ITelegramBotService, TelegramBotService>();
    }
}