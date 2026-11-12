using System.Text.Json;

namespace Cronos.Utilities;

public static class JsonUtils
{
    public static JsonElement? GetPropertyOrNull(this JsonElement element, string propertyName)
    {
        return element.TryGetProperty(propertyName, out JsonElement value) ? value : null;
    }
}