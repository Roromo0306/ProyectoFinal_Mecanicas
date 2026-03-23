using System;
using System.Collections.Generic;
using UnityEngine;

public static class ServiceLocator
{
    private static Dictionary<Type, object> services = new();

    public static void Register<T>(T service)
    {
        services[typeof(T)] = service;
    }

    public static T Get<T>()
    {
        if (!services.ContainsKey(typeof(T)))
        {
            Debug.LogError($"Service {typeof(T)} not registered yet!");
        }
        return (T)services[typeof(T)];
    }
}
