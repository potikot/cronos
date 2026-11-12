using Cronos.Bootstrap.DependencyInjection.Options;
using Cronos.Infrastructure.Http;
using Cronos.Infrastructure.Http.Serializer;
using Cronos.Infrastructure.Logger;

namespace Cronos.Bootstrap.DependencyInjection.Binders;

[BindOrder(-1000)]
public class HttpServiceBinder : BaseBinder
{
    public override void Bind(DIContainer container)
    {
        IOptionObserver<IHttpOptions> httpOptions =
            GetReloadableConfiguration<HttpOptions>(GlobalBinding.Configuration, "http");
        
        IHttpService httpService = new HttpService(
            CreateHttpClient(httpOptions.Value),
            container.Get<ILoggerService>(),
            CreateSerializers(),
            "application/json"
        );

        container.RegisterInstance(httpService);
    }
    
    private HttpClient CreateHttpClient(IHttpOptions options)
    {
        return new HttpClient()
        {
            Timeout = TimeSpan.FromSeconds(options.ConnectionTimeout)
        };
    }

    private List<IHttpSerializer> CreateSerializers()
    {
        return
        [
            new JsonHttpSerializer(null),
            new FormDataHttpSerializer(),
            new HtmlHttpSerializer()
        ];
    }
}