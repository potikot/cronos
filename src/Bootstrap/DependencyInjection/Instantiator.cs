using System.Reflection;
using Cronos.Infrastructure.Logger;

namespace Cronos.Bootstrap.DependencyInjection;

public class Instantiator
{
    private readonly DIContainer _container;
    private ILoggerService _logger;

    public Instantiator(DIContainer container, ILoggerService logger)
    {
        _container = container;
        _logger = logger;

        _container.NotifyWhenServiceRegistered<ILoggerService>(service => _logger = service);
    }

    public T Instantiate<T>()
    {
        return (T)CreateInstance(typeof(T), null);
    }

    public T Instantiate<T>(params object[] extraArgs)
    {
        return (T)CreateInstance(typeof(T), extraArgs);
    }

    public object Instantiate(Type type)
    {
        return CreateInstance(type, null);
    }

    public object Instantiate(Type type, params object[] extraArgs)
    {
        return CreateInstance(type, extraArgs);
    }

    private object CreateInstance(Type type, object[] extraArgs)
    {
        ConstructorInfo[] ctors = type.GetConstructors();
        if (ctors.Length == 0)
        {
            _logger.LogError(this, $"Type '{type.Name}' has no public constructors.");
            return null;
        }

        IOrderedEnumerable<ConstructorInfo> descendingCtors = ctors.OrderByDescending(c => c.GetParameters().Length);
        foreach (ConstructorInfo ctor in descendingCtors)
        {
            ParameterInfo[] parameters = ctor.GetParameters();
            object[] args = new object[parameters.Length];
            bool continueFlag = false;

            for (int i = 0; i < parameters.Length; i++)
            {
                Type paramType = parameters[i].ParameterType;
                if (!ResolveDependency(paramType, out object parameter)
                    && !_container.TryGet(paramType, out parameter))
                {
                    // _logger.LogError(this, $"Dependency '{paramType.Name}' for '{type.Name}' not found.");
                    continueFlag = true;
                    break;
                }

                args[i] = parameter;
            }

            if (continueFlag)
                continue;

            try
            {
                return Activator.CreateInstance(type, args);
            }
            catch (Exception e)
            {
                _logger.LogError(this, $"Failed to create '{type.Name}'. Reason: {e.InnerException}");
                return null;
            }
        }

        _logger.LogError(this, $"Cannot find fitting constructor for type: '{type.Name}'"); // todo: add clue to error string
        return null;

        bool ResolveDependency(Type paramType, out object parameter)
        {
            parameter = null;
            if (extraArgs == null)
                return false;

            foreach (var arg in extraArgs)
            {
                if (arg != null && arg.GetType().IsAssignableTo(paramType))
                {
                    parameter = arg;
                    return true;
                }
            }

            return false;
        }
    }
}