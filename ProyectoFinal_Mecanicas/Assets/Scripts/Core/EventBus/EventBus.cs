using System;
using System.Collections.Generic;

public static class EventBus
{
    private static Dictionary<Type, Action<object>> events = new();

    public static void Subscribe<T>(Action<object> listener)
    {
        var type = typeof(T);
        if (!events.ContainsKey(type))
            events[type] = delegate { };

        events[type] += listener;
    }

    public static void Unsubscribe<T>(Action<object> listener)
    {
        var type = typeof(T);
        if (events.ContainsKey(type))
            events[type] -= listener;
    }

    public static void Publish<T>(T evt)
    {
        var type = typeof(T);
        if (events.ContainsKey(type))
            events[type]?.Invoke(evt);
    }
}
