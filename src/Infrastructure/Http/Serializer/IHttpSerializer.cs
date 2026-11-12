namespace Cronos.Infrastructure.Http.Serializer;

public interface IHttpSerializer
{
    bool CanSerialize(string contentType);
    Task<HttpContent> SerializeAsync(object value);
    Task<object> DeserializeAsync(Type targetType, HttpContent content);
}