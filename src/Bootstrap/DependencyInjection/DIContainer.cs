using Cronos.Bootstrap.DependencyInjection.Model;
using Cronos.Bootstrap.Lifecylcle;
using Cronos.Infrastructure.Logger;

namespace Cronos.Bootstrap.DependencyInjection;

// todo: add ctor params to method args (think about)
public class DIContainer
{
    private readonly List<DIContainer> _children;
    private readonly Dictionary<Type, ServiceRegistration> _services = [];
    private readonly Dictionary<Type, List<ServiceListenerRegistration>> _serviceRegistrationListeners = [];
    private Instantiator _instantiator;
    private ILoggerService _logger;

    // todo: dependency conflicts with core elements like 'IPrinter' & 'ILoggerService'.
    // Because error handler depends on printer. And if error occurs program enters infinite recursion "tries to get error handler what does not exist in system and then also tries to get error handler to output error"

    public DIContainer(ILoggerService logger, IEnumerable<DIContainer> children = null)
    {
        _children = children == null ? [] : children.ToList();
        _instantiator = new Instantiator(this, logger);
        _logger = logger;

        NotifyWhenServiceRegistered<ILoggerService>(service => _logger = service);
        RegisterInstance(this);
        RegisterInstance(_instantiator);
    }
    
    public DIContainer Register<TInterface, TImplementation>(Lifetime lifetime = Lifetime.Singleton) where TImplementation : TInterface
    {
        return Register(typeof(TInterface), typeof(TImplementation), lifetime);
    }
    
    public DIContainer Register<TImplementation>(Lifetime lifetime = Lifetime.Singleton)
    {
        Type implType = typeof(TImplementation);
        return Register(implType, implType, lifetime);
    }

    public DIContainer RegisterInstance<TInterface>(TInterface instance, Lifetime lifetime = Lifetime.Singleton)
    {
        if (instance == null)
        {
            _logger.LogError(this, $"Service instance '{typeof(TInterface).Name}' is null.");
            return this;
        }

        return Register(typeof(TInterface), instance, lifetime);
    }

    private DIContainer Register(Type serviceType, object instanceOrImplType, Lifetime lifetime)
    {
        if (_services.ContainsKey(serviceType))
        {
            _logger.LogError(this, $"Service '{serviceType.Name}' already registered.");
            return this;
        }

        ServiceRegistration registration = instanceOrImplType switch
        {
            Type type => lifetime == Lifetime.Singleton ? new ServiceRegistration(_instantiator.Instantiate(type)) : new ServiceRegistration(type, lifetime),
            _ => new ServiceRegistration(instanceOrImplType)
        };
        
        _services.Add(serviceType, registration);
        AppLifecycle.Register(registration.Instance);
        
        if (_serviceRegistrationListeners.TryGetValue(serviceType, out List<ServiceListenerRegistration> listeners))
        {
            // todo: replace get service
            object service = Get(serviceType);
            for (int i = 0; i < listeners.Count; i++)
            {
                listeners[i].OnRegister.Invoke(service);
                if (listeners[i].Single)
                    listeners.RemoveAt(i--);
            }

            if (listeners.Count == 0)
                _serviceRegistrationListeners.Remove(serviceType);
        }
        
        return this;
    }
    
    public T Get<T>()
    {
        return (T)Get(typeof(T));
    }

    public object Get(Type type)
    {
        if (TryGet(type, out object service))
            return service;
        
        _logger.LogError(this, $"Service {type.Name} is not registered.");
        return null;
    }

    public bool TryGet<T>(out T service)
    {
        if (TryGet(typeof(T), out object objService))
        {
            service = (T)objService;
            return true;
        }
        
        service = default;
        return false;
    }

    public bool TryGet(Type type, out object service)
    {
        if (_services.TryGetValue(type, out ServiceRegistration registration))
        {
            service = registration.Lifetime switch
            {
                Lifetime.Singleton => registration.Instance,
                Lifetime.Transient => _instantiator.Instantiate(registration.Type),
            };
            
            return true;
        }

        foreach (DIContainer c in _children)
            if (c.TryGet(type, out service))
                return true;

        service = null;
        return false;
    }

    public void AddChild(DIContainer child)
    {
        if (child == null || _children.Contains(child))
            return;

        _children.Add(child);
    }

    public void RemoveChild(DIContainer child)
    {
        _children.Remove(child);
    }

    public void NotifyWhenServiceRegistered<TInterface>(Action<TInterface> callback, bool single = true)
    {
        if (callback == null) return;

        ServiceListenerRegistration registration = new(CallbackWrapper, single);
        Type type = typeof(TInterface);
        
        if (_serviceRegistrationListeners.TryGetValue(type, out List<ServiceListenerRegistration> listeners))
            listeners.Add(registration);
        else
            _serviceRegistrationListeners.Add(type, [registration]);
        
        void CallbackWrapper(object instance)
        {
            if (instance is TInterface ci)
                callback(ci);
        }
    }

    // todo: change readonly instance
    private class ServiceRegistration
    {
        public readonly Type Type;
        public readonly object Instance;
        public readonly Lifetime Lifetime;

        public ServiceRegistration(Type type, Lifetime lifetime)
        {
            Type = type;
            Lifetime = lifetime;
        }

        public ServiceRegistration(object instance) : this(instance.GetType(), Lifetime.Singleton)
        {
            Instance = instance;
        }
    }

    private class ServiceListenerRegistration
    {
        public readonly Action<object> OnRegister;
        public readonly bool Single;

        public ServiceListenerRegistration(Action<object> onRegister, bool single)
        {
            Single = single;
            OnRegister = onRegister;
        }
    }
}