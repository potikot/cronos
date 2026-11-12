using System.Text;
using Cronos.Infrastructure.StringFormatter.Model;
using Cronos.Utilities;

namespace Cronos.Infrastructure.StringFormatter;

public class StringFormatterService : IStringFormatterService
{
    private static readonly Delimiter _commandDelimiter = new('{', '}');
    private static readonly Delimiter _colorDelimiter = new('[', ']');

    public string Format(string format, FormatterParams parameters)
    {
        int lastParamEnd = 0;
        int paramStart = 0;
        bool flag = false;
        StringBuilder result = new(format.Length);
        
        for (int i = 0; i < format.Length; i++)
        {
            if (format[i] == '{')
            {
                paramStart = i + 1;
                flag = true;
            }
            
            if (flag && format[i] == '}')
            {
                if (lastParamEnd != paramStart - 1)
                    result.Append(format.Substring(lastParamEnd, paramStart - lastParamEnd - 1));

                string param = format.Substring(paramStart, i - paramStart);
                if (parameters != null && parameters.TryGetValue(param, out object value))
                    result.Append(value);
                else
                    result.Append($"<param not found: {param}>");

                lastParamEnd = i + 1;
                flag = false;
            }
        }
        
        if (lastParamEnd < format.Length)
            result.Append(format[lastParamEnd..]);

        return result.ToString();
    }
    
    public ColoredStringList ColorFormat(string format, ConsoleColor defaultColor, FormatterParams parameters)
    {
        int lastParamEnd = 0;
        int paramStart = 0;
        ConsoleColor color = defaultColor;
        ParamType paramType = ParamType.None;
        ColoredStringList result = new([]);
        
        for (int i = 0; i < format.Length; i++)
        {
            switch (paramType)
            {
                case ParamType.None:
                    if (format[i] == _commandDelimiter.Open)
                    {
                        paramStart = i + 1;
                        paramType = ParamType.Command;
                    }
                    if (format[i] == _colorDelimiter.Open)
                    {
                        paramStart = i + 1;
                        paramType = ParamType.Color;
                    }
                    break;
                case ParamType.Command:
                    if (format[i] != _commandDelimiter.Close)
                        continue;
                    if (lastParamEnd != paramStart - 1)
                        result.Append(format.Substring(lastParamEnd, paramStart - lastParamEnd - 1), color);

                    string param1 = format.Substring(paramStart, i - paramStart);
                    if (parameters != null && parameters.TryGetValue(param1, out object value1))
                        result.Append(value1.ToString(), color);
                    else
                        result.Append($"<param not found: {param1}>", color);

                    lastParamEnd = i + 1;
                    paramType = ParamType.None;
                    break;
                case ParamType.Color:
                    if (format[i] != _colorDelimiter.Close)
                        continue;
                    if (lastParamEnd != paramStart - 1)
                        result.Append(format.Substring(lastParamEnd, paramStart - lastParamEnd - 1), color);

                    string param2 = format.Substring(paramStart, i - paramStart);
                    color = ColorUtils.GetOrDefault(parameters != null && param2.StartsWith("$") ? parameters[param2[1..]].ToString() : param2, defaultColor);

                    lastParamEnd = i + 1;
                    paramType = ParamType.None;
                    break;
            }
        }
        
        if (lastParamEnd < format.Length)
            result.Append(format[lastParamEnd..], color);

        return result;
    }
    
    private record Delimiter(char Open, char Close);

    private enum ParamType
    {
        None = 0,
        Command = 1,
        Color = 2
    }
}