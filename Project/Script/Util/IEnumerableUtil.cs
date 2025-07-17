using System;
using System.Collections.Generic;

namespace Franken;

public static class IEnumerableUtil
{
    public static void Traverse<T>(this T[] array, Action<int, T> action)
    {
        for (int i = 0; i < array.Length; i++) action?.Invoke(i, array[i]);
    }

    public static void ForEach<T>(this IEnumerable<T> target, Action<T> action)
    {
        foreach (var item in target) action?.Invoke(item);
    }
}