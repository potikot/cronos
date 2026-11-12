using Cronos.Bootstrap.DependencyInjection;
using Cronos.Bootstrap.DependencyInjection.Options;
using Cronos.Consts;
using Cronos.Infrastructure.EncryptionService;
using Cronos.Infrastructure.FileSystem;
using Cronos.Infrastructure.Localization;
using Cronos.Infrastructure.Logger;
using Newtonsoft.Json;

namespace Cronos.Infrastructure.Save;

public class EncryptedSaveService : ISaveService
{
    private readonly IOptionObserver<ISaveOptions> _options;
    private readonly ILoggerService _logger;
    private readonly IFileService _fileService;
    private readonly IEncryptionService _encryptionService;
    
    private ILocalizationService _localization;
    
    private readonly JsonSerializerSettings _jsonSerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All
    };

    private Dictionary<string, object> _datas;

    private string FilePath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        GlobalConstants.ProductName,
        _options.Value.SaveFileName);

    public EncryptedSaveService(
        IOptionObserver<ISaveOptions> options,
        ILoggerService logger,
        IFileService fileService,
        IEncryptionService encryptionService,
        ILocalizationService localization)
    {
        _options = options;
        _logger = logger;
        _fileService = fileService;
        _encryptionService = encryptionService;
        _localization = localization;

        LoadData();
    }

    public EncryptedSaveService(
        DIContainer container,
        IOptionObserver<ISaveOptions> options,
        ILoggerService logger,
        IFileService fileService,
        IEncryptionService encryptionService)
        : this(options, logger, fileService, encryptionService, null)
    {
        container.NotifyWhenServiceRegistered<ILocalizationService>(service => _localization = service);
    }
    
    // public async Task OnInitializeAsync()
    // {
    //     _logger.LogWarning("initializing encrypted save service");
    //     // await LoadData();
    // }

    private void LoadData()
    {
        string json = _encryptionService.Decrypt(_fileService.Read(FilePath));
        if (string.IsNullOrEmpty(json))
        {
            _datas = [];
            return;
        }
        
        _datas = JsonConvert.DeserializeObject<Dictionary<string, object>>(json, _jsonSerializerSettings);
        if (_datas == null)
        {
            _logger.LogError(this, $"Could not parse encrypted save file: {FilePath}");
            _datas = [];
        }
    }
    
    public async Task OnExitAsync()
    {
        await SaveAsync();
    }
    
    public bool Set(string key, object data)
    {
        if (data == null)
            return Remove(key);
        
        if (_datas.TryGetValue(key, out object registration)
            && registration.GetType() != data.GetType())
        {
            _logger.LogError($"[SaveService] Data has another type. Key: {key}. Expected type: {data.GetType()}. Given type: {registration.GetType()}");
            return false;
        }
        
        _datas[key] = data;
        return true;
    }

    public bool Remove(string key)
    {
        return _datas.Remove(key);
    }
    
    public T Get<T>(string key, T defaultValue = default)
    {
        TryGet(key, out T data, defaultValue);
        return data;
    }

    public bool TryGet<T>(string key, out T data, T defaultValue = default)
    {
        if (_datas.TryGetValue(key, out object registration)
            && registration is T castedData)
        {
            data = castedData;
            return true;
        }

        data = defaultValue;
        return false;
    }
    
    public async Task SaveAsync()
    {
        _logger.LogInfo(_localization.Get("on_save", "Saving..."));
        string json = JsonConvert.SerializeObject(_datas, _jsonSerializerSettings);
        await _fileService.WriteAsync(FilePath, _encryptionService.Encrypt(json));
    }
}