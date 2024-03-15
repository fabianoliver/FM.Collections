using System;
using System.Collections.Generic;

namespace FM.Collections.Comparers;

public readonly struct KeyValuePairByComparableValueComparer<TKey, TValue> : IComparer<KeyValuePair<TKey, TValue>>
    where TValue : IComparable<TValue>
{
    public int Compare(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y) => x.Value.CompareTo(y.Value);
}