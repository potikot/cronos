using System.Diagnostics;

namespace Cronos.Infrastructure.Logger;

public class FallbackLoggerService : ILoggerService
{
    public void LogInfo(object message) => Debug.Print(message.ToString());
    public void LogInfo(object sender, object message) => LogInfo(sender == null ? message : $"[{sender.GetType().Name}] {message}");

    public void LogWarning(object message) => Debug.Print(message.ToString());
    public void LogWarning(object sender, object message) => LogWarning(sender == null ? message : $"[{sender.GetType().Name}] {message}");

    public void LogError(object message) => Debug.Fail(message.ToString());
    public void LogError(object sender, object message) => LogError(sender == null ? message : $"[{sender.GetType().Name}] {message}");
}