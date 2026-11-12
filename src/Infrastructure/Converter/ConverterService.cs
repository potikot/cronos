using System.Reflection;
using Cronos.Utilities;

namespace Cronos.Infrastructure.Converter;

public class ConverterService : IConverterService
{
    /// <summary>
    /// T should be: int, float, bool, enum or IParsable
    /// </summary>
    /// <returns> object of type T </returns>
    public bool TryConvert<T>(string value, out T result)
    {
        result = (T)Convert(typeof(T), value);
        return result != null;
    }
    
    /// <summary>
    /// T should be: int, float, bool, enum or IParsable
    /// </summary>
    /// <returns> object of type - targetType </returns>
    public bool TryConvert(Type targetType, string value, out object result)
    {
        result = Convert(targetType, value);
        return result != null;
    }
    
    /// <summary>
    /// T should be: int, float, bool, enum or IParsable
    /// </summary>
    /// <returns> object of type T </returns>
    public T Convert<T>(string value)
    {
        return (T)Convert(typeof(T), value);
    }

    /// <summary>
    /// targetType should be: int, float, bool, enum or IParsable
    /// </summary>
    /// <returns> object of type - targetType </returns>
    public object Convert(Type targetType, string value)
    {
        if (targetType == Types.String || targetType == Types.Object)
            return value;

        if (targetType.IsEnum)
        {
            if (int.TryParse(value, out int resultInt))
            {
                if (resultInt > 0 && Enum.GetNames(targetType).Length > resultInt)
                    return Enum.ToObject(targetType, resultInt);
            }
            else if (Enum.TryParse(targetType, value, true, out object resultEnum))
                return resultEnum;

            return null;
        }

        Type convertableInterface = targetType.GetInterface(Types.StringParsable.Name);
        if (convertableInterface != null)
        {
            // IParsable result = (IParsable)Activator.CreateInstance(targetType);
            // result!.TryParse(value);

            MethodInfo method = targetType.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static);
            object result = method!.Invoke(null, [value]);
            
            return result;
        }

        if (targetType == Types.Int)
        {
            if (int.TryParse(value, out int result))
                return result;

            return null;
        }

        if (targetType == Types.Float)
        {
            if (float.TryParse(value, out float result))
                return result;

            return null;
        }

        if (targetType == Types.Boolean)
        {
            return value switch
            {
                "1" => true,
                "0" => false,
                _ => null
            };
        }

        return null;
    }
}