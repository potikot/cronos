using Cronos.Bootstrap.DependencyInjection;
using Cronos.Bootstrap.DependencyInjection.Options;
using Cronos.Bootstrap.Lifecylcle;
using Cronos.Consts;
using Cronos.Features.Settings;
using Cronos.Infrastructure.CommandProcessor;
using Cronos.Infrastructure.Dashboard;
using Cronos.Infrastructure.Input;
using Cronos.Infrastructure.Localization;
using Cronos.Infrastructure.Printer;
using Cronos.Infrastructure.StringFormatter.Model;

namespace Cronos;
/*
 Ошибка из-за потери соединения со стимом
 
 Unhandled exception. System.Net.Http.HttpRequestException: Error while copying content to a stream.
 ---> System.Net.Http.HttpIOException: The response ended prematurely. (ResponseEnded)
   at System.Net.Http.HttpConnection.FillAsync(Boolean async)
   at System.Net.Http.HttpConnection.CopyToContentLengthAsync(Stream destination, Boolean async, UInt64 length, Int32 bufferSize, CancellationToken cancellationToken)
   at System.Net.Http.HttpConnection.ContentLengthReadStream.CompleteCopyToAsync(Task copyTask, CancellationToken cancellationToken)
   at System.Net.Http.HttpConnectionResponseContent.<SerializeToStreamAsync>g__Impl|6_0(Stream stream, CancellationToken cancellationToken)
   at System.Net.Http.HttpContent.LoadIntoBufferAsyncCore(Task serializeToStreamTask, MemoryStream tempBuffer)
   --- End of inner exception stack trace ---
   at System.Net.Http.HttpContent.LoadIntoBufferAsyncCore(Task serializeToStreamTask, MemoryStream tempBuffer)
   at System.Net.Http.HttpClient.<SendAsync>g__Core|83_0(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationTokenSource cts, Boolean disposeCts, CancellationTokenSource pendingRequestsCts, CancellationToken originalCancellationToken)
   at Cronos.Services.Http.HttpService.GetStringAsync(String url) in C:\Programs\Code Projects\Cronos\Src\Services\Http\HttpService.cs:line 39
   at Cronos.Services.Game.Steam.SteamService.IsGameOwnedAsync(GameData game) in C:\Programs\Code Projects\Cronos\Src\Services\Game\Steam\SteamService.cs:line 144
   at Cronos.Services.Game.Steam.SteamService.GetFreeGamesAsync() in C:\Programs\Code Projects\Cronos\Src\Services\Game\Steam\SteamService.cs:line 119
   at Cronos.Services.CommandProcessor.Commands.PrintFreeGamesCommand.ProcessAsync(String[] args) in C:\Programs\Code Projects\Cronos\Src\Services\CommandProcessor\Commands\GameServces\PrintFreeGamesCommand.cs:line 18
   at Cronos.Program.Main(String[] args) in C:\Programs\Code Projects\Cronos\Src\Program.cs:line 59
   at Cronos.Program.<Main>(String[] args)
 */
// todo: think about (just command in cmd) - <your_app> <cmd> <args>
// todo: copy all links or auto send msgs - just pretty copy - mb customizable

internal static class Program
{
    private static readonly LocalizedString ExitMessageLocale = new("cmd_exit", "Exiting...");
    private static readonly LocalizedString EmptyMessageLocale = new("cmd_empty", "Empty command. Type 'help' for a list of available commands.");
    private static readonly LocalizedString InvalidMessageLocale = new("cmd_invalid", "Invalid command ({userInput}). Type 'help' for a list of available commands.");

    private static ISettingsService Settings;
    private static IPrinterService Printer;
    private static IInputService InputService;
    private static ICommandHandler CommandHandler;
    
    private static IOptionObserver<IDashboardOptions> DashboardOptions;

    private static DIContainer C => GlobalContext.Container;
    
    private static async Task Initialize()
    {
        Console.Title = GlobalConstants.Title;

        // init app
        GlobalBinding.Initialize();
        await AppLifecycle.Initialize();
        await AppLifecycle.Start();

        // get services
        Settings = C.Get<ISettingsService>();
        Printer = C.Get<IPrinterService>();
        InputService = C.Get<IInputService>();
        CommandHandler = C.Get<ICommandHandler>();
        
        // get options
        DashboardOptions = C.Get<IOptionObserver<IDashboardOptions>>();
    }
    
    public static async Task Main(string[] args)
    {
        await Initialize();
        // Printer.TextColor = ConsoleColor.Cyan;

        PeriodicTimer updateTimer = new PeriodicTimer(TimeSpan.FromSeconds(1));
        _ = RunPeriodicUpdateAsync(updateTimer);

        if (DashboardOptions.Value.PrintOnStart)
            await ShowDashboard();

        while (true)
        {
            string userInput = InputService.GetLine("$ ", ConsoleColor.Blue);
            CommandResult result = await CommandHandler.ProcessCommandAsync(userInput);
            
            switch (result)
            {
                case CommandResult.Exit:
                    Printer.PrintLine(ExitMessageLocale.ToString());
                    updateTimer.Dispose();
                    await AppLifecycle.Exit();
                    return;
                case CommandResult.Invalid: // todo: give more info?
                    Printer.PrintLine(InvalidMessageLocale.ToString(), new PrintOptions
                    {
                        TextColor = ConsoleColor.Red,
                        FormatterParams = new FormatterParams { { "userInput", userInput } }
                    });
                    break;
                case CommandResult.Empty:
                    Printer.PrintLine(EmptyMessageLocale.ToString());
                    break;
            }
        }
    }

    private static async Task ShowDashboard()
    {
        foreach (string command in DashboardOptions.Value.DashboardCommands)
            await CommandHandler.ProcessCommandAsync(command);
    }
    
    private static async Task RunPeriodicUpdateAsync(PeriodicTimer timer)
    {
        try
        {
            while (await timer.WaitForNextTickAsync())
            {
                AppLifecycle.Update();
            }
        }
        catch (OperationCanceledException)
        {
            // timer stopped
        }
    }
}