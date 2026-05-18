using System;
using System.Collections.Generic;

public static class EventBus
{
    private static class Channel<T>
    {
        public static Action<T> listeners;
    }

    public static void Subscribe<T>(Action<T> listener)
    {
        Channel<T>.listeners += listener;
    }

    public static void Unsubscribe<T>(Action<T> listener)
    {
        Channel<T>.listeners -= listener;
    }

    public static void Publish<T>(T evt)
    {
        Channel<T>.listeners?.Invoke(evt);
    }
}
