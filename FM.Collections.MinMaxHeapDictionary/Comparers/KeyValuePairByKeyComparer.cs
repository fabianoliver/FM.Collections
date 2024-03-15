using System.Collections.Generic;

namespace FM.Collections.Comparers;

public readonly struct KeyValuePairByKeyComparer<TKeyComparer, TKey, TValue> : IComparer<KeyValuePair<TKey, TValue>>
    where TKeyComparer : IComparer<TKey>
{
    public readonly TKeyComparer KeyComparer;

    public KeyValuePairByKeyComparer(TKeyComparer keyComparer)
    {
        KeyComparer = keyComparer;
    }

    public int Compare(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y) => KeyComparer.Compare(x.Key, y.Key);
}