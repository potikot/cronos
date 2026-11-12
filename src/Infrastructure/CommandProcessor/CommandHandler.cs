using Cronos.Infrastructure.CommandProcessor.Commands;
using Cronos.Infrastructure.Logger;

namespace Cronos.Infrastructure.CommandProcessor;

/* Команды - это строки с путем команды и параметрами
 * Путь команды - это строка включающая в себя рут-команду и её подкоманды (пример: steam login)
 * Параметры команды - это строка разделенная пробелами и кавычками, в которой содержатся данные для работы команды. Идет после пути команды
 *  (пример: <api_key> <client_id> или "44 12.4 44")
 *  Параметры могут включать в себя значения, которые начинаются с тире. Это так-же параметры, но они отвечают за включение/выключение части бизнес-логики команды
 *   (пример: -r или -rw)
 *   Эти значения определяются внутри команды и могут быть неизвестны снаружи
 * 
 * Команда парсится в объект типа CommandArguments, который хранит все эти данные в удобном для обработки формате
 * CommandArguments передается в команду и она его обработывает
 * Команда возвращает результат выполнения
 */

public class CommandHandler(ILoggerService _logger) : ICommandHandler
{
    private const char Separator = ' ';

    private readonly Dictionary<string, BaseCommand> _commands = [];

    public IReadOnlyCollection<BaseCommand> Commands => _commands.Values;

    public bool TryAddCommand(BaseCommand command)
    {
        if (command == null)
        {
            _logger.LogError(this, "Invalid command");
            return false;
        }

        if (!_commands.TryAdd(command.Name, command))
        {
            _logger.LogError(this, $"Command '{command.Name}' already exists");
            return false;
        }

        return true;
    }

    public bool RemoveCommand(string name) => _commands.Remove(name);
    public bool RemoveCommand(string name, out BaseCommand command) => _commands.Remove(name, out command);

    public async Task<CommandResult> ProcessCommandAsync(string input)
    {
        if (string.IsNullOrEmpty(input))
            return CommandResult.Empty;
        
        string[] args = ParseArgs(input);
        return _commands.TryGetValue(args[0], out BaseCommand cmd)
            ? await cmd.ProcessAsync(args[1..])
            : CommandResult.Invalid;
    }

    public BaseCommand GetCommand(string commandPath)
    {
        if (string.IsNullOrEmpty(commandPath))
            return null;

        return GetCommand(ParseArgs(commandPath));
    }

    public BaseCommand GetCommand(IReadOnlyList<string> commandPath)
    {
        if (commandPath is not { Count: not 0 })
            return null;

        if (!_commands.TryGetValue(commandPath[0], out BaseCommand command))
            return null;
        
        int idx = 1;
        while (idx < commandPath.Count)
        {
            if (command is not CompositeCommandBase compositeCommand) break;
            if (!compositeCommand.TryGetSubCommand(commandPath[idx++], out BaseCommand cmd)) break;
            command = cmd;
        }
        
        return command;
    }

    private string[] ParseArgs(string input)
    {
        List<string> args = [];
        int argLength = 0;
        bool inQuotes = false;
        
        for (int i = 0; i < input.Length; i++)
        {
            if (input[i] == '"')
            {
                inQuotes = !inQuotes;
                continue;
            }
            
            if (input[i] == Separator && !inQuotes)
            {
                if (argLength > 0)
                {
                    int startOffset = input[i - 1] == '"' ? 1 : 0;
                    // _logger.LogWarning($"'{input.Substring(i - argLength - startOffset, argLength)}'");
                    args.Add(input.Substring(i - argLength - startOffset, argLength));
                    argLength = 0;
                    continue;
                }
            }
            
            argLength++;
        }

        if (argLength > 0)
        {
            int startOffset = input[^1] == '"' ? 1 : 0;
            // _logger.LogWarning($"last ({input[^1] == '"'}): '{input[^(argLength + startOffset)..^startOffset]}'");
            args.Add(input[^(argLength + startOffset)..^startOffset]);
        }
        
        return args.ToArray();
    }
}