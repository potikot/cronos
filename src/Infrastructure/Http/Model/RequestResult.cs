namespace Cronos.Infrastructure.Http.Model;

public record RequestResult<T>(T Result, HttpError Error = null)
{
    public bool IsSuccess() => Error == null;
}