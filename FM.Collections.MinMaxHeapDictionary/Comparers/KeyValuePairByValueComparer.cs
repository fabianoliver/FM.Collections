using System.Collections.Generic;

namespace FM.Collections.Comparers;

public readonly struct KeyValuePairByValueComparer<TValueComparer, TKey, TValue> : IComparer<KeyValuePair<TKey, TValue>>
    where TValueComparer : IComparer<TValue>
{
    public readonly TValueComparer ValueComparer;

    public KeyValuePairByValueComparer(TValueComparer valueComparer)
    {
        ValueComparer = valueComparer;
    }

    public int Compare(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y) => ValueComparer.Compare(x.Value, y.Value);
}