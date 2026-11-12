using System.Net;
using System.Text.Json;
using Cronos.Infrastructure.Http.Model;
using Cronos.Infrastructure.Http.Serializer;
using Cronos.Infrastructure.Logger;
using Cronos.Utilities;
using HtmlAgilityPack;

namespace Cronos.Infrastructure.Http;

public class HttpService(
    HttpClient _httpClient,
    ILoggerService _logger,
    List<IHttpSerializer> _serializers,
    string _defaultContentType) : IHttpService, IDisposable
{
    private bool _disposed;

    public async Task<RequestResult<string>> GetStringAsync(string url, params Header[] headers)
    {
        return await GetAsync<string>(url, null, headers);
    }

    public async Task<RequestResult<Stream>> GetStreamAsync(string url, params Header[] headers)
    {
        return await GetAsync<Stream>(url, null, headers);
    }
    
    public async Task<RequestResult<TResponse>> GetAsync<TResponse>(string url, IHttpSerializer serializer = null, params Header[] headers)
    {
        HttpRequestMessage request = BuildRequest(url, HttpMethod.Get, null, headers);
        return await SendAsync<TResponse>(request, serializer);
    }

    public async Task<HttpResponseMessage> GetRawAsync(string url, params Header[] headers)
    {
        HttpRequestMessage request = BuildRequest(url, HttpMethod.Get, null, headers);
        return await SendRawAsync(request);
    }
    
    public async Task<RequestResult<string>> PostStringAsync<TRequest>(string url, TRequest data, params Header[] headers)
    {
        return await PostAsync<TRequest, string>(url, data, null, headers);
    }

    public async Task<RequestResult<Stream>> PostStreamAsync<TRequest>(string url, TRequest data, params Header[] headers)
    {
        return await PostAsync<TRequest, Stream>(url, data, null, headers);
    }
    
    public async Task<RequestResult<TResponse>> PostAsync<TRequest, TResponse>(string url, TRequest data, IHttpSerializer serializer = null, params Header[] headers)
    {
        HttpResponseMessage response = await PostRawAsync(url, data, headers);
        return await HandleResponseAsync<TResponse>(response, serializer);
    }

    public async Task<HttpResponseMessage> PostRawAsync<TRequest>(string url, TRequest data, params Header[] headers)
    {
        HttpContent content = await CreateContentAsync(data, headers);
        HttpRequestMessage request = BuildRequest(url, HttpMethod.Post, content, headers);
        
        return await SendRawAsync(request);
    }

    public async Task<RequestResult<string>> PostStringAsync(string url, string data, params Header[] headers)
    {
        return await PostAsync<string, string>(url, data, null, headers);
    }

    public async Task<RequestResult<TResponse>> PostStringAsync<TResponse>(string url, string data, IHttpSerializer serializer = null, params Header[] headers)
    {
        return await PostAsync<string, TResponse>(url, data, serializer, headers);
    }

    public async Task<RequestResult<string>> PostBytesAsync(string url, byte[] data, params Header[] headers)
    {
        return await PostAsync<byte[], string>(url, data, null, headers);
    }

    public async Task<RequestResult<TResponse>> PostBytesAsync<TResponse>(string url, byte[] data, IHttpSerializer serializer = null, params Header[] headers)
    {
        return await PostAsync<byte[], TResponse>(url, data, serializer, headers);
    }

    public async Task<RequestResult<string>> PostStreamAsync(string url, Stream data, params Header[] headers)
    {
        return await PostAsync<Stream, string>(url, data, null, headers);
    }

    public async Task<RequestResult<TResponse>> PostStreamAsync<TResponse>(string url, Stream data, IHttpSerializer serializer = null, params Header[] headers)
    {
        return await PostAsync<Stream, TResponse>(url, data, serializer, headers);
    }
    
    private async Task<HttpContent> CreateContentAsync<TRequest>(TRequest data, Header[] headers)
    {
        if (data == null)
            return null;

        string contentType = GetContentType(headers);
        HttpContent content = data switch
        {
            string stringData => new StringContent(stringData, System.Text.Encoding.UTF8, contentType),
            byte[] byteData => new ByteArrayContent(byteData),
            Stream streamData => new StreamContent(streamData),
            _ => await (_serializers
                            .FirstOrDefault(x => x.CanSerialize(contentType))
                            ?.SerializeAsync(data)
                        ?? Task.FromResult<HttpContent>(null))
        };

        if (content == null)
            _logger.LogError(this, $"No serializer available for content type: {contentType}");
        
        // _logger.LogInfo(this, $"Content: {content?.ReadAsStringAsync().Result}");
        return content;
    }

    private string GetContentType(Header[] headers)
    {
        return headers?.FirstOrDefault(h => h != null && h.Name.Equals("Content-Type", StringComparison.OrdinalIgnoreCase))?.Value
               ?? _defaultContentType;
    }
    
    private async Task<RequestResult<TResponse>> SendAsync<TResponse>(HttpRequestMessage request, IHttpSerializer serializer = null)
    {
        try
        {
            // _logger.LogInfo(this, $"Sending request: {request.RequestUri}");
            HttpResponseMessage response = await _httpClient.SendAsync(request);
            return await HandleResponseAsync<TResponse>(response, serializer);
        }
        catch (HttpRequestException e)
        {
            _logger.LogError(this, e);
            return new RequestResult<TResponse>(default, new HttpError(e.StatusCode ?? HttpStatusCode.ServiceUnavailable, e.Message));
        }
        catch (Exception e)
        {
            _logger.LogError(this, e);
            return new RequestResult<TResponse>(default, new HttpError(HttpStatusCode.InternalServerError, e.Message));
        }
    }

    private async Task<HttpResponseMessage> SendRawAsync(HttpRequestMessage request)
    {
        try
        {
            return await _httpClient.SendAsync(request);
        }
        catch (Exception e)
        {
            _logger.LogError(this, e);
            return null;
        }
    }

    private HttpRequestMessage BuildRequest(string url, HttpMethod method, HttpContent content, params Header[] headers)
    {
        HttpRequestMessage request = new(method, url)
        {
            Content = content
        };

        if (headers != null)
            foreach (var header in headers)
                if (header != null && !header.Name.Equals("Content-Type", StringComparison.OrdinalIgnoreCase))
                    request.Headers.TryAddWithoutValidation(header.Name, header.Value);

        return request;
    }
    
    private async Task<RequestResult<T>> HandleResponseAsync<T>(HttpResponseMessage response, IHttpSerializer serializer = null)
    {
        if (response == null)
            return new RequestResult<T>(default, new HttpError(HttpStatusCode.UnsupportedMediaType, "Response is null"));

        if (!response.IsSuccessStatusCode)
        {
            string url = response.RequestMessage?.RequestUri?.ToString() ?? "UNKNOWN_URL";
            string method = response.RequestMessage?.Method.Method ?? "UNKNOWN_METHOD";
            string errorContent = await response.Content.ReadAsStringAsync();
        
            _logger.LogError($"{method}({url}) failed with status code '{response.StatusCode}'. Message: {response.ReasonPhrase}. Content: {errorContent}");
            return new RequestResult<T>(default, new HttpError(response.StatusCode, errorContent));
        }

        string contentType = response.Content.Headers.ContentType?.MediaType ?? "";
        object responseData;

        Type targetType = typeof(T);
        if (targetType == Types.String)
        {
            responseData = await response.Content.ReadAsStringAsync();
        }
        else if (targetType == Types.Stream)
        {
            responseData = await response.Content.ReadAsStreamAsync();
        }
        else if (targetType == Types.ByteArray)
        {
            responseData = await response.Content.ReadAsByteArrayAsync();
        }
        else if (targetType == Types.JsonDocument)
        {
            string json = await response.Content.ReadAsStringAsync();
            responseData = JsonDocument.Parse(json);
        }
        else if (targetType == Types.HtmlDocument)
        {
            string json = await response.Content.ReadAsStringAsync();
            HtmlDocument document = new();
            document.LoadHtml(json);
            responseData = document;
        }
        else
        {
            serializer ??= _serializers.FirstOrDefault(x => x.CanSerialize(contentType));
            if (serializer == null)
            {
                _logger.LogError(this, $"No serializer available for content type: {contentType}");
                return new RequestResult<T>(default, new HttpError(HttpStatusCode.UnsupportedMediaType, $"No serializer available for content type: {contentType}"));
            }
            responseData = await serializer.DeserializeAsync(targetType, response.Content);
        }

        return new RequestResult<T>((T)responseData);
    }
    
    public void Dispose()
    {
        if (_disposed)
            return;

        _httpClient.Dispose();
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}