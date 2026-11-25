using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace InvenTrack.Core;

public abstract class ViewModelBase : INotifyPropertyChanged, IDisposable
{
    private readonly List<(string Channel, Action<object?> Callback)> _subscriptions = [];
    private bool _disposed;

    public event PropertyChangedEventHandler? PropertyChanged;

    public void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    protected void Subscribe(string channel, Action<object?> callback)
    {
        _subscriptions.Add((channel, callback));
        Mediator.Subscribe(channel, callback);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        foreach (var (channel, callback) in _subscriptions)
            Mediator.Unsubscribe(channel, callback);

        _subscriptions.Clear();
        OnDispose();
        GC.SuppressFinalize(this);
    }

    protected virtual void OnDispose() { }
}
