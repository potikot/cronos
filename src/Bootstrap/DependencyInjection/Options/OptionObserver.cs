namespace Cronos.Bootstrap.DependencyInjection.Options;

public class OptionObserver<T> : IOptionObserver<T>
{
    private T _value;
    private Action<T> _onChange;

    public event Action<T> OnChange
    {
        add => _onChange += value;
        remove => _onChange -= value;
    }
    
    public T Value
    {
        get => _value;
        set
        {
            if (_value.Equals(value))
                return;
            
            _onChange?.Invoke(value);
            _value = value;
        }
    }
    
    public OptionObserver(T value, Action<T> onChange = null)
    {
        _value = value;
        _onChange = onChange;
    }
}