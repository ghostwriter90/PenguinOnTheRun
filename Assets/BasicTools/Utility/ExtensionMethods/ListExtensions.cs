using System.Collections.Generic;

public static class ListExtensions
{
    /// <summary>
    /// Shuffle the items of the List
    /// </summary>

    public static IList<T> Shuffle<T>(this IList<T> list)
    {
        if (list.IsNullOrEmpty())
            return list;

        for (int i = 0; i < list.Count - 1; i++)
        {
            // random index [i,n)
            int index = UnityEngine.Random.Range(i, list.Count);
            list.Swap(i, index);
        }
        return list;
    }

    /// <summary>
    /// Megadja, hogy az adott lista null vagy üres.
    /// </summary>
    /// <typeparam name="T">A listában levő elemek típusa.</typeparam>
    /// <param name="list">Maga a lista.</param>
    /// <returns>True, ha ez a lista üres vagy null.</returns>
    public static bool IsNullOrEmpty<T>(this IList<T> list)
    {
        return (list == null || list.Count == 0);
    }

    /// <summary>
    ///  Megcserél két elemet a listában, ha ez lehetséges.
    /// </summary>
    /// <typeparam name="T">A listában levő elemek típusa.</typeparam>
    /// <param name="list">Maga a lista.</param>
    /// <param name="i">Az első elem indexe.</param>
    /// <param name="j">A második elem indexe.</param>
    /// <returns>Visszatér a listával.</returns>
    public static IList<T> Swap<T>(this IList<T> list, int i, int j)
    {
        // Invalid list
        if (list.IsNullOrEmpty())
            return list;

        // Invalid indexes
        if (i == j || list.Count <= i || list.Count <= j || i < 0 || j < 0)
            return list;

        var swap = list[i];
        list[i] = list[j];
        list[j] = swap;
        return list;
    }

    /// <summary>
    /// Visszaad egy random elemet a listából, ha lehetséges, ha nem akkor a default elemmel tér vissza.
    /// </summary>
    public static T GetRandomElement<T>(this IList<T> list)
    {
        if (list.IsNullOrEmpty())
            return default(T);

        return list[UnityEngine.Random.Range(0, list.Count)];
    }

    public static T GetRandomElement<T>(this IList<T> list, T _default)
    {
        if (list.IsNullOrEmpty())
            return _default;

        return list[UnityEngine.Random.Range(0, list.Count)];
    }

    /// <summary>
    /// Kivesz egy random elemet a listából, ha lehetséges, ha nem akkor a default elemmel tér vissza.
    /// </summary>
    public static T PopRandomElement<T>(this IList<T> list)
    {
        if (list.IsNullOrEmpty())
            return default(T);

        var obj = list[UnityEngine.Random.Range(0, list.Count)];
        list.Remove(obj);
        return obj;
    }
}

