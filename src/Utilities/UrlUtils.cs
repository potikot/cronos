using System.Net;
using System.Text;

namespace Cronos.Utilities;

public static class UrlUtils
{
    public static string WithQueryParameter(this string url, string key, object value)
    {
        if (string.IsNullOrWhiteSpace(url)
            ||string.IsNullOrWhiteSpace(key)
            || value == null)
            return url;
        
        string encodedKey = WebUtility.UrlEncode(key);
        string encodedValue = WebUtility.UrlEncode(value.ToString());

        char separator = url.Contains("?") ? '&' : '?';
        return $"{url}{separator}{encodedKey}={encodedValue}";
    }
    
    public static string WithQueryParameters(this string url, params (string key, object value)[] parameters)
    {
        if (string.IsNullOrWhiteSpace(url)
            || parameters is not { Length: not 0 })
            return url;
        
        StringBuilder sb = new();
        bool hasPrevious = false;
        
        for (int i = 0; i < parameters.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(parameters[i].key)
                || parameters[i].value == null)
                continue;

            if (hasPrevious) sb.Append('&');
            sb.AppendFormat("{0}={1}",
                WebUtility.UrlEncode(parameters[i].key),
                WebUtility.UrlEncode(parameters[i].value.ToString()));
            hasPrevious = true;
        }

        char separator = url.Contains("?") ? '&' : '?';
        return sb.Length > 0 ? $"{url}{separator}{sb}" : url;
    }
}