using Cronos.Bootstrap.DependencyInjection.Options;
using Cronos.Bootstrap.Lifecylcle;
using Cronos.Consts;
using Cronos.Features.Yougile.Model;
using Cronos.Infrastructure.Http;
using Cronos.Infrastructure.Http.Model;
using Cronos.Infrastructure.Logger;
using Cronos.Infrastructure.Save;

namespace Cronos.Features.Yougile;

public class YougileService : IOnInitializeAsync
{
    private const string ApiKey = "yougile_api-key";

    private readonly IOptionObserver<IYougileOptions> _options;
    private readonly ILoggerService _logger;
    private readonly IHttpService _httpService;
    private readonly ISaveService _saveService;

    private Header[] _headers;

    private string _apiKey;

    private readonly YouGileColumn _notSortedColumn = new("")
    {
        Name = "Not Sorted"
    };

    public YougileService(
        IOptionObserver<IYougileOptions> options,
        ILoggerService logger,
        IHttpService httpService,
        ISaveService saveService)
    {
        // options.RegisterOnChange(opt => Console.WriteLine("updated configuration for yougile"));
        _options = options;
        _logger = logger;
        _saveService = saveService;
        _httpService = httpService;

        _apiKey = saveService.Get<string>(ApiKey);

        _headers =
        [
            new Header("Authorization", $"Bearer {_apiKey}"),
            new Header("Content-Type", "application/json")
        ];
    }

    public async Task OnInitializeAsync()
    {
        
    }

    public async Task<bool> Login(string apiKey)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
            return false;

        Header[] h =
        [
            new("Authorization", $"Bearer {apiKey}"),
            new("Content-Type", "application/json")
        ];

        RequestResult<string> response = await _httpService.GetStringAsync(YouGileApiEndpoints.Resources.Projects, h);
        _logger.LogWarning(this, response.Result);

        if (response.IsSuccess())
        {
            _apiKey = apiKey;
            _headers = h;
            return true;
        }

        return false;
    }

    public async Task<YougileTask> AddTaskAsync(string title, string description)
    {
        YougileTask task = new()
        {
            Title = title,
            Description = description,
            ColumnId = _notSortedColumn.Id
        };

        RequestResult<string> response = await _httpService.PostStringAsync(YouGileApiEndpoints.Resources.Task, task, _headers);

        if (!response.IsSuccess())
            _logger.LogError($"Task '{title}' was not added to column '{_notSortedColumn.Name}'.");

        return task;
    }
}