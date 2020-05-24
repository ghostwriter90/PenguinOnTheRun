using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtensions
{
    static List<Component> m_ComponentCache = new List<Component>();

    public static GameObject InstantinateCopy(this GameObject prefab)
    {
        return Object.Instantiate(prefab) as GameObject;
    }

    public static GameObject InstantinateCopyAndAlignToDefault(this GameObject prefab, Transform parent)
    {
        GameObject newGameObject = Object.Instantiate(prefab);
        newGameObject.transform.SetParentAndAlignToDefault(parent);
        return newGameObject;
    }

    /// <summary>
    /// Use this insted of GetComponent and test if it is null. It's about 10-20 times faster if result is null, else 3 times slover. 
    /// </summary>
    public static bool HasComponent<T>(this GameObject self) where T : Component
    {
        self.GetComponents(typeof(T), m_ComponentCache);
        bool result = m_ComponentCache.Count > 0;
        m_ComponentCache.Clear();
        return result;
    }

    /// <summary>
    /// Use this insted of GetComponent if you are not sure is there is a component. It's about 10-20 times faster if result is null, else 3 times slover. 
    /// </summary>
    public static bool TryGetComponent<T>(this GameObject self, out T component) where T : Component
    {
        self.GetComponents(typeof(T), m_ComponentCache);
        bool result = m_ComponentCache.Count > 0;
        component = result ? (T) m_ComponentCache[0] : null;
        m_ComponentCache.Clear();
        return result;
    }
    
}

