using System.Collections;
using System.Collections.Generic;

namespace FM.Collections;


public abstract class MinMaxHeapDictionaryBase<TArity, TComparer, TKey, TValue> : IMinMaxDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
    where TArity : struct, IConstInt
    where TComparer : IComparer<KeyValuePair<TKey, TValue>>
    where TKey : notnull
{
    private readonly MinMaxHeapDictionaryImpl<TArity, TComparer, TKey, TValue> _inner;

    protected MinMaxHeapDictionaryBase(TComparer comparer, IEqualityComparer<TKey>? keyComparer = null, int initialCapacity = -1)
    {
        _inner = new MinMaxHeapDictionaryImpl<TArity, TComparer, TKey, TValue>(new MinMaxDictionaryNodeComparer<TComparer, TKey, TValue>(comparer), keyComparer, initialCapacity);
    }

    protected MinMaxHeapDictionaryBase(TComparer comparer, IEnumerable<KeyValuePair<TKey, TValue>> values, IEqualityComparer<TKey>? keyComparer = null)
    {
        _inner = new MinMaxHeapDictionaryImpl<TArity, TComparer, TKey, TValue>(new MinMaxDictionaryNodeComparer<TComparer, TKey, TValue>(comparer), keyComparer, values);
    }

    protected MinMaxHeapDictionaryBase(MinMaxHeapDictionaryBase<TArity, TComparer, TKey, TValue> self)
    {
        _inner = new MinMaxHeapDictionaryImpl<TArity, TComparer, TKey, TValue>(self._inner);
    }
    internal IReadOnlyList<KeyValuePair<TKey, TValue>> RawValues => _inner.RawValues;

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _inner.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_inner).GetEnumerator();
    public void Add(KeyValuePair<TKey, TValue> item) => _inner.Add(item);
    public void Clear() => _inner.Clear();
    public bool Contains(KeyValuePair<TKey, TValue> item) => _inner.Contains(item);
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => _inner.CopyTo(array, arrayIndex);
    public bool Remove(KeyValuePair<TKey, TValue> item) => _inner.Remove(item);
    public int Count => _inner.Count;
    public bool IsReadOnly => _inner.IsReadOnly;
    public void Add(TKey key, TValue value) => _inner.Add(key, value);
    public bool ContainsKey(TKey key) => _inner.ContainsKey(key);
    public bool Remove(TKey key) => _inner.Remove(key);
    public bool TryGetValue(TKey key, out TValue value) => _inner.TryGetValue(key, out value);
    public ICollection<TKey> Keys => _inner.Keys;
    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;
    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

    public ICollection<TValue> Values => _inner.Values;
    public IComparer<KeyValuePair<TKey, TValue>> Comparer => _inner.Comparer.InnerComparer;
    public IEqualityComparer<TKey> KeyComparer => _inner.KeyComparer;
    public KeyValuePair<TKey, TValue> Max => _inner.Max;
    public KeyValuePair<TKey, TValue> Min => _inner.Min;
    public KeyValuePair<TKey, TValue> RemoveMax() => _inner.RemoveMax();
    public KeyValuePair<TKey, TValue> RemoveMin() => _inner.RemoveMin();

    public TValue this[TKey key]
    {
        get => _inner[key];
        set => _inner[key] = value;
    }
}