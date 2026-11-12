namespace Cronos.Infrastructure.Converter;

public interface IConverterService
{
    /// <summary>
    /// T should be: int, float, bool, enum or IParsable
    /// </summary>
    /// <returns> object of type T </returns>
    bool TryConvert<T>(string value, out T result);

    /// <summary>
    /// T should be: int, float, bool, enum or IParsable
    /// </summary>
    /// <returns> object of type - targetType </returns>
    bool TryConvert(Type targetType, string value, out object result);

    /// <summary>
    /// T should be: int, float, bool, enum or IParsable
    /// </summary>
    /// <returns> object of type T </returns>
    T Convert<T>(string value);

    /// <summary>
    /// targetType should be: int, float, bool, enum or IParsable
    /// </summary>
    /// <returns> object of type - targetType </returns>
    object Convert(Type targetType, string value);
}