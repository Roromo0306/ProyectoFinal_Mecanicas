using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventBus
{
    private static readonly Dictionary<Type, Delegate> events = new Dictionary<Type, Delegate>();

    public static void Subscribe<T>(Action<T> listener)
    {
        if (listener == null)
            return;

        Type type = typeof(T);

        if (events.TryGetValue(type, out Delegate current))
            events[type] = Delegate.Combine(current, listener);
        else
            events[type] = listener;
    }

    public static void Unsubscribe<T>(Action<T> listener)
    {
        if (listener == null)
            return;

        Type type = typeof(T);

        if (!events.TryGetValue(type, out Delegate current))
            return;

        Delegate newDelegate = Delegate.Remove(current, listener);

        if (newDelegate == null)
            events.Remove(type);
        else
            events[type] = newDelegate;
    }

    public static void Publish<T>(T evt)
    {
        Type type = typeof(T);

        if (!events.TryGetValue(type, out Delegate current))
            return;

        Delegate[] listeners = current.GetInvocationList();

        foreach (Delegate listener in listeners)
        {
            try
            {
                ((Action<T>)listener)?.Invoke(evt);
            }
            catch (Exception e)
            {
                Debug.LogError(
                    "EventBus -> Error publicando evento " + type.Name +
                    " en listener " + listener.Method.DeclaringType + "." + listener.Method.Name +
                    "\n" + e
                );
            }
        }
    }

    public static void ClearAll()
    {
        events.Clear();
    }
}
