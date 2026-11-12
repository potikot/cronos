using Cronos.Infrastructure.Http.Model;
using Cronos.Infrastructure.Http.Serializer;

namespace Cronos.Infrastructure.Http;

public interface IHttpService
{
    Task<RequestResult<string>> GetStringAsync(string url, params Header[] headers);
    Task<RequestResult<Stream>> GetStreamAsync(string url, params Header[] headers);
    Task<RequestResult<TResponse>> GetAsync<TResponse>(string url, IHttpSerializer serializer = null, params Header[] headers);
    Task<HttpResponseMessage> GetRawAsync(string url, params Header[] headers);
    
    Task<RequestResult<string>> PostStringAsync<TRequest>(string url, TRequest data, params Header[] headers);
    Task<RequestResult<Stream>> PostStreamAsync<TRequest>(string url, TRequest data, params Header[] headers);
    Task<RequestResult<TResponse>> PostAsync<TRequest, TResponse>(string url, TRequest data, IHttpSerializer serializer = null, params Header[] headers);
    Task<HttpResponseMessage> PostRawAsync<TRequest>(string url, TRequest data, params Header[] headers);
    
    Task<RequestResult<string>> PostStringAsync(string url, string data, params Header[] headers);
    Task<RequestResult<TResponse>> PostStringAsync<TResponse>(string url, string data, IHttpSerializer serializer = null, params Header[] headers);
    Task<RequestResult<string>> PostBytesAsync(string url, byte[] data, params Header[] headers);
    Task<RequestResult<TResponse>> PostBytesAsync<TResponse>(string url, byte[] data, IHttpSerializer serializer = null, params Header[] headers);
    Task<RequestResult<string>> PostStreamAsync(string url, Stream data, params Header[] headers);
    Task<RequestResult<TResponse>> PostStreamAsync<TResponse>(string url, Stream data, IHttpSerializer serializer = null, params Header[] headers);
}