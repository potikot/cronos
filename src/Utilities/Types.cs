using System.Text.Json;
using HtmlAgilityPack;

namespace Cronos.Utilities;

public static class Types
{
    public static readonly Type Object = typeof(object);

    public static readonly Type Int = typeof(int);
    public static readonly Type Float = typeof(float);
    public static readonly Type Boolean = typeof(bool);
    public static readonly Type String = typeof(string);
    
    public static readonly Type ByteArray = typeof(byte[]);
    public static readonly Type StringArray = typeof(string[]);
    public static readonly Type ObjectArray = typeof(object[]);

    public static readonly Type Stream = typeof(Stream);
    
    public static readonly Type JsonDocument = typeof(JsonDocument);
    public static readonly Type HtmlDocument = typeof(HtmlDocument);

    public static readonly Type StringParsable = typeof(Infrastructure.Converter.IParsable<>);

    public static bool Match(this Type type, params Type[] match)
    {
        foreach (Type t in match)
            if (type == t)
                return true;

        return false;
    }
}