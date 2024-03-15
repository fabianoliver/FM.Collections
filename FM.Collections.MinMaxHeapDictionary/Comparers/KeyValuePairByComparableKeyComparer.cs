using System;
using System.Collections.Generic;

namespace FM.Collections.Comparers;

public readonly struct KeyValuePairByComparableKeyComparer<TKey, TValue> : IComparer<KeyValuePair<TKey, TValue>>
    where TKey : IComparable<TKey>
{
    public int Compare(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y) => x.Key.CompareTo(y.Key);
}