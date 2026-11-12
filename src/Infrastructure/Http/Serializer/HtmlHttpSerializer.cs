using HtmlAgilityPack;

namespace Cronos.Infrastructure.Http.Serializer;

public class HtmlHttpSerializer : IHttpSerializer
{
    public const string CONTENT_TYPE = "text/html";
       
    public bool CanSerialize(string contentType) => contentType.Equals(CONTENT_TYPE, StringComparison.OrdinalIgnoreCase);

    public Task<HttpContent> SerializeAsync(object value)
    {
        throw new NotImplementedException();
    }

    public async Task<object> DeserializeAsync(Type targetType, HttpContent content)
    {
        if (targetType != typeof(HtmlDocument))
            throw new ArgumentException("'targetType' must be a 'HtmlDocument'");
        
        string stringContent = await content.ReadAsStringAsync();
        HtmlDocument htmlDocument = new();
        htmlDocument.LoadHtml(stringContent);
        return htmlDocument;
    }
}