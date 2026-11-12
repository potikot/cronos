using System.Net;

namespace Cronos.Infrastructure.Http.Model;

public class HttpError
{
    public HttpStatusCode Code { get; }
    public string Message { get; }

    public HttpError(HttpStatusCode code, string message)
    {
        Code = code;
        Message = message;
    }

    public HttpError(HttpResponseMessage response) : this(response.StatusCode, response.ReasonPhrase) { }

    public override string ToString()
    {
        return $"Status: {Code}. Message: {Message}";
    }
}