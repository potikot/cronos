using Cronos.Infrastructure.Logger;

namespace Cronos.Bootstrap.DependencyInjection;

public static class GlobalContext
{
    public static readonly DIContainer Container;
    static GlobalContext() => Container = new DIContainer(new FallbackLoggerService());
}