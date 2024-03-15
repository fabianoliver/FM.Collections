using System.Collections.Generic;

namespace FM.Collections;

public interface IMinMaxDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IMinMaxHeap<KeyValuePair<TKey, TValue>>
{
    IEqualityComparer<TKey> KeyComparer { get; }
}