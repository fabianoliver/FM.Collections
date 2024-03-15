using System.Collections.Generic;

namespace FM.Collections;

internal readonly struct MinMaxDictionaryNode<TKey,TValue>
{
    public readonly KeyValuePair<TKey, TValue> Kvp;
    public readonly Index Index;

    public MinMaxDictionaryNode(KeyValuePair<TKey, TValue> kvp, Index index)
    {
        Kvp = kvp;
        Index = index;
    }
}