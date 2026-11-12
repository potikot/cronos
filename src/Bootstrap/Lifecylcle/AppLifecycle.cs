namespace Cronos.Bootstrap.Lifecylcle;

public static class AppLifecycle
{
    private static readonly HashSet<IOnInitializeAsync> _initializeEventListeners = [];
    private static readonly HashSet<IOnStartAsync> _startEventListeners = [];
    private static readonly HashSet<IOnExitAsync> _exitEventListeners = [];
    private static readonly HashSet<IOnUpdate> _updateEventListeners = [];

    private static bool _isInitialized;
    private static bool _isStarted;
    private static bool _isExited;

    private static DateTime _lastUpdateTime;
    
    public static void Register<T>(T listener) where T : class
    {
        if (listener == null)
            return;

        if (listener is IOnInitializeAsync initializeEventListener)
            _initializeEventListeners.Add(initializeEventListener);
        if (listener is IOnStartAsync startEventListener)
            _startEventListeners.Add(startEventListener);
        if (listener is IOnExitAsync exitEventListener)
            _exitEventListeners.Add(exitEventListener);
        if (listener is IOnUpdate updateEventListener)
            _updateEventListeners.Add(updateEventListener);
    }
    
    public static async Task Initialize()
    {
        if (_isInitialized) return;
        _isInitialized = true;

        foreach (IOnInitializeAsync listener in _initializeEventListeners)
            await listener.OnInitializeAsync();
        
        _lastUpdateTime = DateTime.Now;
        
        Console.WriteLine($"[{nameof(AppLifecycle)}] Initialized.");
    }

    public static async Task Start()
    {
        if (_isStarted) return;
        _isStarted = true;
        
        foreach (IOnStartAsync listener in _startEventListeners)
            await listener.OnStartAsync();
        
        Console.WriteLine($"[{nameof(AppLifecycle)}] Started.");
    }

    public static void Update()
    {
        double deltaTime = DateTime.Now.Subtract(_lastUpdateTime).TotalSeconds;
        _lastUpdateTime = DateTime.Now;

        foreach (var listener in _updateEventListeners)
        {
            listener.OnUpdate((float)deltaTime);
        }
    }
    
    public static async Task Exit()
    {
        if (_isExited) return;
        _isExited = true;
        
        foreach (IOnExitAsync listener in _exitEventListeners)
            await listener.OnExitAsync();
        
        Console.WriteLine($"[{nameof(AppLifecycle)}] Exited.");
    }
}