using System.Text;
using Newtonsoft.Json;

namespace Cronos.Infrastructure.Http.Serializer;

public class JsonHttpSerializer(JsonSerializerSettings _settings) : IHttpSerializer
{
    public const string CONTENT_TYPE = "application/json";
    
    public bool CanSerialize(string contentType) => contentType.Equals(CONTENT_TYPE, StringComparison.OrdinalIgnoreCase);

    public Task<HttpContent> SerializeAsync(object value)
    {
        string json = JsonConvert.SerializeObject(value, _settings);
        StringContent content = new StringContent(json, Encoding.UTF8, CONTENT_TYPE);
        return Task.FromResult<HttpContent>(content);
    }

    public async Task<object> DeserializeAsync(Type targetType, HttpContent content)
    {
        string json = await content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject(json, targetType, _settings);
    }
}