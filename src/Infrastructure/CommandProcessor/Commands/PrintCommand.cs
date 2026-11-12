using System.Text;
using Cronos.Infrastructure.Printer;

namespace Cronos.Infrastructure.CommandProcessor.Commands;

public class PrintCommand : BaseCommand
{
    private readonly IPrinterService _printer;

    public override string Name { get; } = "print";
    public override string Description { get; } = "Prints text to console.";

    public PrintCommand(IPrinterService printer)
    {
        _printer = printer;
    }

    public override Task<CommandResult> ProcessAsync(string[] args)
    {
        if (!TryParseArguments(args, out CommandArguments arguments))
            return EmptyResult;

        var output = ConcatStringArray(arguments.Arguments);
        var colors = arguments.GetOptionArguments("-c");
        if (colors.Count > 0)
            output = $"[{colors[0]}]{output}";

        _printer.PrintLine(output);
        return SuccessResult;
    }

    private string ConcatStringArray(List<string> arguments)
    {
        if (arguments is not { Count: > 0 })
            return "";

        StringBuilder sb = new(arguments[0]);
        for (int i = 1; i < arguments.Count; i++)
            sb.AppendFormat(" {0}", arguments[i]);

        return sb.ToString();
    }
}