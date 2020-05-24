using System;
using System.Collections.Generic;
using UnityEngine;

public static class ComponentExtensions
{
    static List<Component> m_ComponentCache = new List<Component>();

    public static T Copy<T>(this T original, GameObject destination) where T : Component
    {
        Type type = original.GetType();
        var dst = destination.GetComponent(type) as T;
        if (!dst) dst = destination.AddComponent(type) as T;
        var fields = type.GetFields();
        foreach (var field in fields)
        {
            if (field.IsStatic) continue;
            field.SetValue(dst, field.GetValue(original));
        }
        var props = type.GetProperties();
        foreach (var prop in props)
        {
            if (!prop.CanWrite || !prop.CanWrite || prop.Name == "name") continue;
            prop.SetValue(dst, prop.GetValue(original, null), null);
        }
        return dst as T;
    }

    /// <summary>
    /// Use This insted of GetComponent and test if it is null. It's about 10-20 times faster if result is null, else 3 times slover. 
    /// </summary>
    public static bool HasComponent<T>(this Component self) where T : Component
    {
        self.GetComponents(typeof(T), m_ComponentCache);
        bool result = m_ComponentCache.Count > 0;
        m_ComponentCache.Clear();
        return result;
    }

    /// <summary>
    /// Use this insted of GetComponent if you are not sure is there is a component. It's about 10-20 times faster if result is null, else 3 times slover. 
    /// </summary>
    public static bool TryGetComponent<T>(this Component self, out T component) where T : Component
    {
        self.GetComponents(typeof(T), m_ComponentCache);
        bool result = m_ComponentCache.Count > 0;
        component = result ? (T)m_ComponentCache[0] : null;
        m_ComponentCache.Clear();
        return result;
    }
}
