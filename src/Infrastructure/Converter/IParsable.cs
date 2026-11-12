namespace Cronos.Infrastructure.Converter;

public interface IParsable<T> where T : IParsable<T>
{
    static abstract T Parse(string value);
    static abstract bool TryParse(string value, out T result);
}