namespace Cronos.Infrastructure.Http.Serializer;

public class FormDataHttpSerializer : IHttpSerializer
{
    public const string CONTENT_TYPE = "application/x-www-form-urlencoded";
    
    public bool CanSerialize(string contentType) => contentType.Equals(CONTENT_TYPE, StringComparison.OrdinalIgnoreCase);

    public Task<HttpContent> SerializeAsync(object value)
    {
        if (value is FormUrlEncodedContent formContent)
            return Task.FromResult<HttpContent>(formContent);
        
        throw new ArgumentException("FormDataHttpSerializer only supports FormUrlEncodedContent", nameof(value));
    }

    public async Task<object> DeserializeAsync(Type targetType, HttpContent content)
    {
        string response = await content.ReadAsStringAsync();
        return response;
    }
}
