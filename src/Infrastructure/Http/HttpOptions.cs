namespace Cronos.Infrastructure.Http;

public interface IHttpOptions
{
    float ConnectionTimeout { get; }
}

public class HttpOptions : IHttpOptions
{
    public float ConnectionTimeout { get; set; }
}