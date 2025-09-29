using System;
using System.Collections.Generic;

public static class EventBus
{
    private static readonly Dictionary<Type, List<Delegate>> _eventTable = new();

    public static void Subscribe<T>(Action<T> handler) where T : IEvent
    {
        var eventType = typeof(T);
        if (!_eventTable.TryGetValue(eventType, out var handlers))
        {
            handlers = new List<Delegate>();
            _eventTable.Add(eventType, handlers);
        }
        if (!handlers.Contains(handler))
            handlers.Add(handler);
    }

    public static void Unsubscribe<T>(Action<T> handler) where T : IEvent
    {
        var eventType = typeof(T);
        if (_eventTable.TryGetValue(eventType, out var handlers))
        {
            handlers.Remove(handler);
            if (handlers.Count == 0)
                _eventTable.Remove(eventType);
        }
    }

    public static void Raise<T>(T evt) where T : IEvent
    {
        var eventType = typeof(T);
        if (_eventTable.TryGetValue(eventType, out var handlers))
        {
            foreach (var handler in handlers)
            {
                (handler as Action<T>)?.Invoke(evt);
            }
        }
    }
}