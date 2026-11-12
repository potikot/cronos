using Cronos.Bootstrap.Lifecylcle;

namespace Cronos.Infrastructure.Save;

public interface ISaveService : IOnExitAsync
{
    Task SaveAsync();
    
    bool Set(string key, object data);

    bool Remove(string key);
    
    T Get<T>(string key, T defaultValue = default);
    bool TryGet<T>(string key, out T data, T defaultValue = default);
}