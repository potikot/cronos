using Cronos.Infrastructure.Printer;

namespace Cronos.Infrastructure.CommandProcessor.Commands;

public class HelpCommand(
    IPrinterService _printer,
    ICommandHandler _commandHandler) : BaseCommand
{
    private const int IndentSize = 2;
    private const int StartIndentLevel = 0;

    public override string Name { get; } = "help";
    public override string Description { get; } = "Shows this command list";

    public override Task<CommandResult> ProcessAsync(string[] args)
    {
        (IReadOnlyList<BaseCommand> commands, DisplayMode displayMode) = ParseArguments(args);
        if (commands == null) return InvalidResult;

        List<CommandEntry> commandEntries = [];
        CollectCommandEntries(commands, displayMode, StartIndentLevel, commandEntries);
        int maxLeftLength = commandEntries.Max(e => e.Name.Length);

        foreach (CommandEntry entry in commandEntries)
            _printer.PrintLine($"{entry.Name.PadRight(maxLeftLength)} | {entry.Description}");

        _printer.PrintLine();
        return SuccessResult;
    }

    private (IReadOnlyList<BaseCommand> commands, DisplayMode displayMode) ParseArguments(string[] args)
    {
        if (!TryParseArguments(args, out CommandArguments arguments))
            return (_commandHandler.Commands.ToList(), DisplayMode.TopLevel);

        bool recursive = arguments.HasOption("-a");
        if (arguments.Arguments.Count == 0 && recursive)
            return (_commandHandler.Commands.ToList(), DisplayMode.All);

        BaseCommand targetCommand = _commandHandler.GetCommand(arguments.Arguments);
        if (targetCommand == null)
            return (null, DisplayMode.TopLevel);

        return ([targetCommand], recursive ? DisplayMode.All : DisplayMode.ImmediateChildren);
    }

    private void CollectCommandEntries(
        IReadOnlyList<BaseCommand> commands,
        DisplayMode displayMode,
        int indentationLevel,
        List<CommandEntry> result)
    {
        string indent = new(' ', indentationLevel * IndentSize);
        foreach (BaseCommand command in commands)
        {
            string leftPart = $"{indent}{command.Name}";
            result.Add(new CommandEntry(leftPart, command.Description));
            
            if (command is CompositeCommandBase composite)
            {
                bool showChildren = displayMode switch
                {
                    DisplayMode.All => true,
                    DisplayMode.ImmediateChildren => indentationLevel == 0,
                    _ => false
                };

                if (showChildren)
                {
                    DisplayMode childDisplayMode = displayMode switch
                    {
                        DisplayMode.All => DisplayMode.All,
                        _ => DisplayMode.TopLevel
                    };

                    CollectCommandEntries(
                        composite.SubCommands,
                        childDisplayMode,
                        indentationLevel + 1,
                        result);
                }
            }
        }
    }

    private enum DisplayMode
    {
        /// <summary> Show commands only of the current level </summary>
        TopLevel = 0,
        /// <summary> Show commands of the current level and their immediate children </summary>
        ImmediateChildren = 1,
        /// <summary> Show all commands recursively </summary>
        All = 2
    }

    private record CommandEntry(string Name, string Description);
}