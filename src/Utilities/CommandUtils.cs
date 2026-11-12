using Cronos.Infrastructure.CommandProcessor;

namespace Cronos.Utilities;

public static class CommandUtils
{
    // todo: does not recognize command itself
    public static CommandArguments ParseCommand(this string input, char delimiter = ' ')
    {
        CommandArguments args = new();
        int argLength = 0;
        bool inQuotes = false;
        string option = null;
        
        for (int i = 0; i < input.Length; i++)
        {
            if (input[i] == '"')
            {
                inQuotes = !inQuotes;
                continue;
            }
            
            if (input[i] == delimiter && !inQuotes)
            {
                if (argLength > 0)
                {
                    ParseValue(input, args, i, argLength, ref option);
                    argLength = 0;
                    continue;
                }
            }
            
            argLength++;
        }

        if (argLength > 0)
        {
            ParseValue(input, args, input.Length, argLength, ref option);

            // int startOffset = input[^1] == '"' ? 1 : 0;
            // args.Arguments.Add(input[^(argLength + startOffset)..^startOffset]);
        }
        
        return args;
    }

    private static void ParseValue(string input, CommandArguments args, int curIdx, int argLength, ref string option)
    {
        int startOffset = input[curIdx - 1] == '"' ? 1 : 0;
        string value = input.Substring(curIdx - argLength - startOffset, argLength);
        
        if (value.StartsWith("-"))
        {
            option = value;
            args.Options.Add(option, []);
            return;
        }
        
        if (option == null)
            args.Arguments.Add(value);
        else
            args.Options[option].Add(value);
    }
}