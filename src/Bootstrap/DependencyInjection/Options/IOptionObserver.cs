namespace Cronos.Bootstrap.DependencyInjection.Options;

public interface IOptionObserver<out T>
{
    event Action<T> OnChange;

    T Value { get; }
}