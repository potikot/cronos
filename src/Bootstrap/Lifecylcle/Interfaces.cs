namespace Cronos.Bootstrap.Lifecylcle;

public interface IOnInitializeAsync
{
    Task OnInitializeAsync();
}

public interface IOnStartAsync
{
    Task OnStartAsync();
}

public interface IOnExitAsync
{
    Task OnExitAsync();
}

public interface IOnUpdate
{
    void OnUpdate(float deltaTime);
}
