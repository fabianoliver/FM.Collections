using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FM.Collections;

internal sealed class MinMaxHeapDictionaryImpl<TArity, TComparer, TKey, TValue> : MinMaxHeapBase<TArity, MinMaxDictionaryNodeComparer<TComparer, TKey, TValue>, MinMaxDictionaryNode<TKey,TValue>>, IMinMaxDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
    where TArity : struct, IConstInt
    where TComparer : IComparer<KeyValuePair<TKey, TValue>>
    where TKey : notnull
{
    // Typical operations require pushing elements up/down the heap; we'd like to avoid comparatively expensive dictionary lookups for every swap,
    // Hence the int index is wrapped into a ref type (Index). When typically then just need to look up the starting value in this dictionary,
    // And for every resulting swap, we can update the index via Node.Index, instead of having to go back to this dictionary via lookup.
    private readonly Dictionary<TKey, Index> _indices; 

    IComparer<KeyValuePair<TKey, TValue>> IMinMaxHeap<KeyValuePair<TKey, TValue>>.Comparer => MinMaxComparer;
    public TComparer MinMaxComparer => Comparer.InnerComparer;
   
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public IEqualityComparer<TKey> KeyComparer { get; }
    public new KeyValuePair<TKey, TValue> Max => base.Max.Kvp;
    public new KeyValuePair<TKey, TValue> Min => base.Min.Kvp;
    public new KeyValuePair<TKey, TValue> RemoveMax() => base.RemoveMax().Kvp;
    public new KeyValuePair<TKey, TValue> RemoveMin() => base.RemoveMin().Kvp;
    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;
    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

    private object? _syncRoot = null;
    private ValueCollection? _valueCollection = null;
    private KeyCollection? _keyCollection = null;
    private RawValueCollection? _rawValueCollection = null;


    public KeyCollection Keys => _keyCollection ??= new KeyCollection(this);
    public ValueCollection Values => _valueCollection ??= new ValueCollection(this);
    internal RawValueCollection RawValues =>  _rawValueCollection ??= new RawValueCollection(this);
    
    ICollection<TValue> IDictionary<TKey, TValue>.Values => Values;
    ICollection<TKey> IDictionary<TKey, TValue>.Keys => Keys;

    private object SyncRoot
    {
        get
        {
            if (_syncRoot == null)
                System.Threading.Interlocked.CompareExchange<object?>(ref _syncRoot, new object(), null);
            return _syncRoot;
        }
    }
    
    internal MinMaxHeapDictionaryImpl(MinMaxDictionaryNodeComparer<TComparer, TKey, TValue> comparer, IEqualityComparer<TKey>? keyComparer, int initialCapacity)
        : base(comparer, initialCapacity)
    {
        KeyComparer = keyComparer ?? EqualityComparer<TKey>.Default;
        _indices = new Dictionary<TKey, Index>(keyComparer);
    }
    
    internal MinMaxHeapDictionaryImpl(MinMaxDictionaryNodeComparer<TComparer, TKey, TValue> comparer, IEqualityComparer<TKey>? keyComparer, IEnumerable<KeyValuePair<TKey, TValue>> items)
        : base(comparer, CreateNodes(items))
    {
        KeyComparer = keyComparer ?? EqualityComparer<TKey>.Default;
        _indices = new Dictionary<TKey, Index>(Count, keyComparer);
        
        for (var i = 0; i < Count; i++)
            _indices[base[i].Kvp.Key] = base[i].Index;
    }
    
    private static IEnumerable<MinMaxDictionaryNode<TKey, TValue>> CreateNodes(IEnumerable<KeyValuePair<TKey, TValue>> source)
    {
        return source.Select((x, i) => new MinMaxDictionaryNode<TKey, TValue>(x, new Index(i))).ToList();
    }

    public void Add(TKey key, TValue value)
    {
        Add(new KeyValuePair<TKey, TValue>(key, value));
    }

    public override void Add(MinMaxDictionaryNode<TKey, TValue> item)
    {
        _indices.Add(item.Kvp.Key, item.Index);
        base.Add(item);
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        var index = new Index(Count);
        var node = new MinMaxDictionaryNode<TKey, TValue>(item, index);
        Add(node);
    }
    
    public new IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        foreach (var value in (MinMaxHeapBase<TArity, MinMaxDictionaryNodeComparer<TComparer, TKey, TValue>, MinMaxDictionaryNode<TKey, TValue>>)this)
            yield return value.Kvp;
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return TryGetValue(item.Key, out var existing) && Equals(existing, item.Value);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        for (var i = 0; i < Count; i++)
            array[arrayIndex++] = this[i].Kvp;
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        if (!Contains(item))
            return false;
        return Remove(item.Key);
    }

    public bool ContainsKey(TKey key)
    {
        return _indices.ContainsKey(key);
    }

    public bool Remove(TKey key)
    {
        if (!_indices.TryGetValue(key, out var index))
            return false;
        Remove(index.Value);
        return true;
    }

    protected override void SetValue(MinMaxDictionaryNode<TKey, TValue>[] values, int index, MinMaxDictionaryNode<TKey, TValue> newValue)
    {
        //if(newValue.Kvp.Key.ToString() == "77")
        //    Debugger.Break();
        base.SetValue(values, index, newValue);
        newValue.Index.Value = index;
    }
    
    protected override MinMaxDictionaryNode<TKey, TValue> this[int index]
    {
        get => base[index];
        set
        {
            if(_indices.TryGetValue(value.Kvp.Key, out var existingIndex))
                existingIndex.Value = index;
            else
                _indices[value.Kvp.Key] = new Index(Count);
            
            base[index] = value;
        }
    }
    
    public TValue this[TKey key]
    {
        get => TryGetValue(key, out var result)
            ? result
            : throw new KeyNotFoundException();
        set
        {
            var kvp = new KeyValuePair<TKey, TValue>(key, value);
            if (_indices.TryGetValue(key, out var index))
                this[index.Value] = new MinMaxDictionaryNode<TKey, TValue>(kvp, index);
            else
                Add(new KeyValuePair<TKey, TValue>(key, value));
        }
    }

    protected override MinMaxDictionaryNode<TKey, TValue> Remove(int index)
    {
        var kvp = base.Remove(index);
        _indices.Remove(kvp.Kvp.Key);
        return kvp;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        if (!_indices.TryGetValue(key, out var index))
        {
            value = default!;
            return false;
        }

        value = this[index.Value].Kvp.Value;
        return true;
    }
    
    public sealed class RawValueCollection : IReadOnlyList<KeyValuePair<TKey, TValue>>
    {
        private readonly MinMaxHeapDictionaryImpl<TArity, TComparer, TKey, TValue> _dictionary;
        public RawValueCollection(MinMaxHeapDictionaryImpl<TArity, TComparer, TKey, TValue> dictionary)
        {
            _dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dictionary.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int Count => _dictionary.Count;
        public KeyValuePair<TKey, TValue> this[int index] => _dictionary[index].Kvp;
    }
    

    public sealed class ValueCollection : ICollection<TValue>, ICollection, IReadOnlyList<TValue>
    {
        private readonly MinMaxHeapDictionaryImpl<TArity, TComparer, TKey, TValue> _dictionary;

        public ValueCollection(MinMaxHeapDictionaryImpl<TArity, TComparer, TKey, TValue> dictionary)
        {
            _dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
        }

        public void CopyTo(TValue[]? array, int index)
        {
            if (array is null)
                throw new ArgumentNullException(nameof(array));

            if (index < 0 || index > array.Length)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (array.Length - index < _dictionary.Count)
                throw new ArgumentException("Array too small");

            foreach (var kvp in _dictionary)
                array[index++] = kvp.Value;
        }

        public int Count => _dictionary.Count;

        bool ICollection<TValue>.IsReadOnly => true;

        void ICollection<TValue>.Add(TValue item) => throw new NotSupportedException();

        bool ICollection<TValue>.Remove(TValue item) => throw new NotSupportedException();

        void ICollection<TValue>.Clear() => throw new NotSupportedException();

        bool ICollection<TValue>.Contains(TValue item) => _dictionary.Any(x => Equals(x, item));

        IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
        {
            foreach (var kvp in _dictionary)
                yield return kvp.Value;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var kvp in _dictionary)
                yield return kvp.Value;
        }

        void ICollection.CopyTo(Array array, int index)
        {
            if (array is null)
                throw new ArgumentNullException(nameof(array));

            if (array.Rank != 1)
                throw new ArgumentException("Multidimensional array not supported");

            if (array.GetLowerBound(0) != 0)
                throw new ArgumentException("Non-zero lower array bound");

            if (index < 0 || index > array.Length)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (array.Length - index < _dictionary.Count)
                throw new ArgumentException("Array plus offset too small");

            if (array is TValue[] values)
            {
                CopyTo(values, index);
            }
            else
            {
                if (array is not object[] objects)
                    throw new ArgumentException("Invalid array type", nameof(array));

                try
                {
                    foreach (var kvp in _dictionary)
                        objects[index++] = kvp.Value!;
                }
                catch (ArrayTypeMismatchException)
                {
                    throw new ArgumentException("Invalid array type", nameof(array));
                }
            }
        }

        bool ICollection.IsSynchronized => false;
        object ICollection.SyncRoot => _dictionary.SyncRoot;
        public TValue this[int index] => _dictionary[index].Kvp.Value;
    }


    public sealed class KeyCollection : ICollection<TKey>, ICollection, IReadOnlyList<TKey>
    {
        private readonly MinMaxHeapDictionaryImpl<TArity, TComparer, TKey, TValue> _dictionary;

        public KeyCollection(MinMaxHeapDictionaryImpl<TArity, TComparer, TKey, TValue> dictionary)
        {
            _dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
        }

        public void CopyTo(TKey[]? array, int index)
        {
            if (array is null)
                throw new ArgumentNullException(nameof(array));

            if (index < 0 || index > array.Length)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (array.Length - index < _dictionary.Count)
                throw new ArgumentException("Array too small");

            foreach (var kvp in _dictionary)
                array[index++] = kvp.Key;
        }

        public int Count => _dictionary.Count;

        bool ICollection<TKey>.IsReadOnly => true;

        void ICollection<TKey>.Add(TKey item) => throw new NotSupportedException();

        bool ICollection<TKey>.Remove(TKey item) => throw new NotSupportedException();

        void ICollection<TKey>.Clear() => throw new NotSupportedException();

        bool ICollection<TKey>.Contains(TKey item) => _dictionary.ContainsKey(item);

        IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator()
        {
            foreach (var kvp in _dictionary)
                yield return kvp.Key;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var kvp in _dictionary)
                yield return kvp.Key;
        }

        void ICollection.CopyTo(Array array, int index)
        {
            if (array is null)
                throw new ArgumentNullException(nameof(array));

            if (array.Rank != 1)
                throw new ArgumentException("Multidimensional array not supported");

            if (array.GetLowerBound(0) != 0)
                throw new ArgumentException("Non-zero lower array bound");

            if (index < 0 || index > array.Length)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (array.Length - index < _dictionary.Count)
                throw new ArgumentException("Array plus offset too small");

            if (array is TKey[] keys)
            {
                CopyTo(keys, index);
            }
            else
            {
                if (array is not object[] objects)
                    throw new ArgumentException("Invalid array type", nameof(array));

                try
                {
                    foreach (var kvp in _dictionary)
                        objects[index++] = kvp.Key!;
                }
                catch (ArrayTypeMismatchException)
                {
                    throw new ArgumentException("Invalid array type", nameof(array));
                }
            }
        }

        bool ICollection.IsSynchronized => false;
        object ICollection.SyncRoot => _dictionary.SyncRoot;
        public TKey this[int index] => _dictionary[index].Kvp.Key;
    }
}