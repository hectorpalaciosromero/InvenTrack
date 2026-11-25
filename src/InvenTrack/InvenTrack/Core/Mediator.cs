namespace InvenTrack.Core;

public static class Mediator
{
    private static readonly Dictionary<string, List<Action<object?>>> _subscribers = [];

    public static void Subscribe(string channel, Action<object?> callback)
    {
        if (!_subscribers.ContainsKey(channel))
            _subscribers[channel] = [];

        _subscribers[channel].Add(callback);
    }

    public static void Unsubscribe(string channel, Action<object?> callback)
    {
        if (_subscribers.TryGetValue(channel, out var callbacks))
            callbacks.Remove(callback);
    }

    public static void Publish(string channel, object? data = null)
    {
        if (!_subscribers.TryGetValue(channel, out var callbacks))
            return;

        foreach (var callback in callbacks.ToList())
            callback(data);
    }
}
