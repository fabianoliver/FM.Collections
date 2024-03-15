using System.Collections.Generic;

namespace FM.Collections;

internal readonly struct MinMaxDictionaryNodeComparer<TComparer, TKey, TValue> : IComparer<MinMaxDictionaryNode<TKey, TValue>>
    where TComparer : IComparer<KeyValuePair<TKey, TValue>>
{
    public readonly TComparer InnerComparer;

    public MinMaxDictionaryNodeComparer(TComparer innerComparer)
    {
        InnerComparer = innerComparer;
    }

    public int Compare(MinMaxDictionaryNode<TKey, TValue> x, MinMaxDictionaryNode<TKey, TValue> y)
    {
        return InnerComparer.Compare(x.Kvp, y.Kvp);
    }
}