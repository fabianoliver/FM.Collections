using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace FM.Collections;

public sealed partial class ImmutableCopyOnWriteDictionary<TKey, TValue> where TKey : notnull
{
    public sealed partial class Builder : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
    {
        private ImmutableCopyOnWriteDictionary<TKey, TValue> _parent;
        private Dictionary<TKey, TValue>? _cloned = null;

        internal Builder(ImmutableCopyOnWriteDictionary<TKey, TValue> parent)
        {
            _parent = parent;
        }
        private Dictionary<TKey, TValue> EnsureCloned()
        {
            return _cloned ??= new Dictionary<TKey, TValue>(_parent._source, _parent._source.Comparer);
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(TKey key, TValue value)
        {
            if(Contains(new KeyValuePair<TKey, TValue>(key,value)))
                return;
            EnsureCloned().Add(key, value);
        }
        
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }
        
        public void Clear()
        {
            _cloned = new Dictionary<TKey, TValue>(comparer: _parent._source.Comparer);
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            if (_cloned is not null)
                return _cloned.TryGetValue(item.Key, out var value) && _parent.ValueComparer.Equals(value, item.Value);
            return _parent.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)(_cloned ?? _parent._source)).CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return TryGetValue(item.Key, out var value) && Equals(value, item.Value) && ((ICollection<KeyValuePair<TKey, TValue>>)EnsureCloned()).Remove(item);
        }

        public int Count => (_cloned ?? _parent._source).Count;
        public bool IsReadOnly => false;
     
        public bool ContainsKey(TKey key)
        {
            return (_cloned ?? _parent._source).ContainsKey(key);
        }

        public bool Remove(TKey key)
        {
            return ContainsKey(key) && EnsureCloned().Remove(key);
        }

#if NET5_0_OR_GREATER
        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)

#else
        public bool TryGetValue(TKey key, out TValue value)
#endif
        {
            return (_cloned ?? _parent._source).TryGetValue(key, out value);
        }

        public ICollection<TKey> Keys => (_cloned ?? _parent._source).Keys;
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;
        public ICollection<TValue> Values => (_cloned ?? _parent._source).Values;
        public IEqualityComparer<TKey> KeyComparer => _parent.KeyComparer;
        public IEqualityComparer<TValue> ValueComparer => _parent.ValueComparer;

        public TValue this[TKey key]
        {
            get => (_cloned ?? _parent._source)[key];
            set
            {
                if (_parent.Contains(new KeyValuePair<TKey, TValue>(key, value)))
                    return;
                EnsureCloned()[key] = value;
            }
        }
        
        public void RemoveRange(IEnumerable<TKey> keys)
        {
            foreach (var key in keys)
                Remove(key);
        }

        public void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> pairs)
        {
            Dictionary<TKey, TValue>? target = null;

            foreach (var kvp in pairs)
            {
                if (_parent.Contains(kvp))
                    continue;

                if (target is null)
                {
                    target = EnsureCloned();
#if NET6_0_OR_GREATER
                    if(pairs.TryGetNonEnumeratedCount(out var count))
                        target.EnsureCapacity(count);
#endif
                }
                
                target.Add(kvp.Key, kvp.Value);
            }
        }

#if NET5_0_OR_GREATER
        public ImmutableCopyOnWriteDictionary<TKey, TValue> ToImmutable(bool trimExcess = false)
#else
        public ImmutableCopyOnWriteDictionary<TKey, TValue> ToImmutable()
#endif
        {
            if (_cloned is { } cloned)
            {
#if NET5_0_OR_GREATER
                if(trimExcess)
                    cloned.TrimExcess();
#endif
                _cloned = null;
                _parent = new ImmutableCopyOnWriteDictionary<TKey, TValue>(cloned, _parent.ValueComparer);
                return _parent;
            }

            return _parent;
        }
    }
}