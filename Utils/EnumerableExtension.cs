using System;
using System.Collections.Generic;

namespace Utils;

public static class EnumerableExtension
{
    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
        foreach (var item in enumerable)
        {
            action.Invoke(item);
        }
    }

    public static IEnumerable<T> Do<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
        foreach (var item in enumerable)
        {
            action.Invoke(item);
            yield return item;
        }
    }
}