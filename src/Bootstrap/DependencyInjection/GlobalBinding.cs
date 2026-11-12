using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace Cronos.Bootstrap.DependencyInjection;

public static class GlobalBinding
{
    private static bool IsInitialized;

    public static IConfigurationRoot Configuration { get; private set; }

    public static void Initialize()
    {
        if (IsInitialized) return;
        
        BuildConfiguration();
        CollectAndBindAllBinders();

        IsInitialized = true;
    }

    private static void BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", false, true);

#if PRIVATE
        builder.AddJsonFile("appsettings.private.json", true, true);
#endif

        Configuration = builder.Build();
    }

    private static void CollectAndBindAllBinders()
    {
        DIContainer container = GlobalContext.Container;
        IEnumerable<(int order, BaseBinder binder)> binders = CollectAllBinders();
        foreach ((_, BaseBinder binder) in binders)
            binder.Bind(container);
    }

    // todo: search by all assemblies (runtime or cache while build app)
    private static IEnumerable<(int order, BaseBinder binder)> CollectAllBinders()
    {
        Type binderType = typeof(BaseBinder);
        
        List<(int order, BaseBinder binder)> binders = [];
        IEnumerable<Type> allTypes = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a => a.GetTypes());

        foreach (Type type in allTypes)
        {
            if (type.IsClass && !type.IsAbstract && binderType.IsAssignableFrom(type))
            {
                int order = type.GetCustomAttribute<BindOrderAttribute>()?.Order ?? 0;
                BaseBinder binder = (BaseBinder)Activator.CreateInstance(type);
                binders.Add((order, binder));
            }
        }

        return binders.OrderBy(x => x.order);
    }
}