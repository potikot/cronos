namespace Cronos.Infrastructure.Logger;

public interface ILoggerService
{
    void LogInfo(object message);
    void LogInfo(object sender, object message);
    
    void LogWarning(object message);
    void LogWarning(object sender, object message);
    
    void LogError(object message);
    void LogError(object sender, object message);
}