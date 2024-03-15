using System;
using System.Collections.Generic;

namespace FM.Collections.Comparers;

public readonly struct ComparableComparer<T> : IComparer<T>
    where T : IComparable<T>
{
    public int Compare(T? x, T? y)
    {
        if (x is null && y is null) return 0;
        if (x is null) return -1;
        if (y is null) return 1;
        return x.CompareTo(y);
    }
}