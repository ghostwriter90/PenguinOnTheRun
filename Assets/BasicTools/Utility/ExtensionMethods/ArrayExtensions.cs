using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ArrayExtensions
{
    public static void Fill<T>(this T[] originalArray, T value)
    {
        for (int i = 0; i < originalArray.Length; i++)
        {
            originalArray[i] = value;
        }
    }

    public static bool Contains<T>(this T[] originalArray, T value)
    {
        return Array.IndexOf(originalArray, value) > -1;
    }


    public delegate T OneTypeFunction<T>(T item);

    public static T[] MakeCopy<T>(this T[] source, OneTypeFunction<T> action = null)
    {
        int l = source.Length;
        T[] copy = new T[l];
        source.CopyTo<T>(ref copy, 0, 0, l, action);
        return copy;
    }

    public static void CopyTo<T>(this T[] source, ref T[] dest,  int sourceStart, int destStart, int count, OneTypeFunction<T> action = null)
    {
        if (dest == null || source == null)
        {
            Debug.LogError("Null ref error");
            return;
        }
        int j = 0;
        if (action == null)
        {
            for (int i = destStart; i < destStart + count; i++)
            {
                dest[i] = source[j];
                j++;
            }
        }
        else
        {
            for (int i = destStart; i < destStart + count; i++)
            {
                dest[i] = action(source[j]);
                j++;
            }
        }
    }

    public static T GetLast<T>(this T[] self)
    {
        return self[self.Length - 1];
    }

    public static T[] Append<T>(this T[] self, T[] other)
    {
        int sl = self.Length;
        int ol = other.Length;
        T[] all = new T[ol+sl];

        int j = 0;
        for (; j < sl; j++)
        {
            all[j] = self[j];
        }

        for (; j < sl+ol; j++)
        {
            all[j] = other[j - sl];
        }

        return all;
    }
}
