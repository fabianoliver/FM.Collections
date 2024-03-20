using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if NET5_0_OR_GREATER
using System.Collections.Immutable;
#endif

namespace FM.Collections;

public sealed partial class ImmutableCopyOnWriteDictionary<TKey, TValue>
#if NET5_0_OR_GREATER
    : IImmutableDictionary<TKey, TValue>
#else
    : IReadOnlyDictionary<TKey, TValue>
#endif
    where TKey : notnull
{
    public static readonly ImmutableCopyOnWriteDictionary<TKey, TValue> Empty = new ImmutableCopyOnWriteDictionary<TKey, TValue>(new Dictionary<TKey, TValue>());
    
    private readonly Dictionary<TKey, TValue> _source;
    public IEqualityComparer<TValue> ValueComparer { get; }

    private ImmutableCopyOnWriteDictionary(Dictionary<TKey, TValue> source, IEqualityComparer<TValue> valueComparer)
    {
        _source = source;
        ValueComparer = valueComparer;
    }

    internal ImmutableCopyOnWriteDictionary(IEqualityComparer<TKey>? keyComparer = null, IEqualityComparer<TValue>? valueComparer = null)
        : this(new Dictionary<TKey, TValue>(keyComparer), valueComparer ?? EqualityComparer<TValue>.Default)
    {
    }
    
    internal ImmutableCopyOnWriteDictionary(IEnumerable<KeyValuePair<TKey, TValue>> source, IEqualityComparer<TKey>? keyComparer = null, IEqualityComparer<TValue>? valueComparer = null)
        : this(ToDictionary(source, keyComparer, valueComparer ?? EqualityComparer<TValue>.Default), valueComparer ?? EqualityComparer<TValue>.Default)
    {
    }

    private static Dictionary<TKey, TValue> ToDictionary(IEnumerable<KeyValuePair<TKey, TValue>> source, IEqualityComparer<TKey>? keyComparer, IEqualityComparer<TValue> valueComparer)
    {
#if NET6_0_OR_GREATER
        var result = source.TryGetNonEnumeratedCount(out var count)
            ? new Dictionary<TKey, TValue>(count, keyComparer)
            : new Dictionary<TKey, TValue>(keyComparer);
#else
        var result = new Dictionary<TKey, TValue>(keyComparer);
#endif
        
        foreach (var kvp in source)
        {
            if (result.TryGetValue(kvp.Key, out var existingValue) && valueComparer.Equals(kvp.Value, existingValue))
                continue;
            result.Add(kvp.Key, kvp.Value);
        }

        return result;
    }

    public ImmutableCopyOnWriteDictionary<TKey, TValue> WithKeyComparer(IEqualityComparer<TKey>? keyComparer = null)
    {
        return WithComparers(keyComparer, ValueComparer);
    }
    
    public ImmutableCopyOnWriteDictionary<TKey, TValue> WithComparers(IEqualityComparer<TKey>? keyComparer = null, IEqualityComparer<TValue>? valueComparer = null)
    {
        keyComparer ??= EqualityComparer<TKey>.Default;
        valueComparer ??= EqualityComparer<TValue>.Default;
        
        return ReferenceEquals(keyComparer, _source.Comparer)  && ReferenceEquals(valueComparer, ValueComparer)
            ? this
            : new ImmutableCopyOnWriteDictionary<TKey, TValue>(_source, keyComparer, valueComparer);
    }

    public IEqualityComparer<TKey> KeyComparer => _source.Comparer;

    public Builder ToBuilder() => new Builder(this);
    
    public Enumerator GetEnumerator() => new Enumerator(_source);
    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public int Count => _source.Count;
    public bool ContainsKey(TKey key) => _source.ContainsKey(key);
    public bool TryGetValue(TKey key, out TValue value) => _source.TryGetValue(key, out value);
    public TValue this[TKey key] => _source[key];
    public IEnumerable<TKey> Keys => _source.Keys;
    public IEnumerable<TValue> Values => _source.Values;
    
    public ImmutableCopyOnWriteDictionary<TKey, TValue> Add(TKey key, TValue value)
    {
        var builder = ToBuilder();
        builder.Add(key, value);
        return builder.ToImmutable();
    }
    
    public ImmutableCopyOnWriteDictionary<TKey, TValue> AddRange(IEnumerable<KeyValuePair<TKey, TValue>> pairs)
    {
        var builder = ToBuilder();
        builder.AddRange(pairs);
        return builder.ToImmutable();
    }
    
    public ImmutableCopyOnWriteDictionary<TKey, TValue> Clear()
    {
        return ReferenceEquals(_source.Comparer, EqualityComparer<TKey>.Default) && ReferenceEquals(ValueComparer, EqualityComparer<TValue>.Default)
            ? Empty
            : new ImmutableCopyOnWriteDictionary<TKey, TValue>(new Dictionary<TKey, TValue>(_source.Comparer), ValueComparer);
    }


    public bool Contains(KeyValuePair<TKey, TValue> pair) => _source.TryGetValue(pair.Key, out var value) && ValueComparer.Equals(value, pair.Value);

    public ImmutableCopyOnWriteDictionary<TKey, TValue> Remove(TKey key)
    {
        var builder = ToBuilder();
        builder.Remove(key);
        return builder.ToImmutable();
    }

    public ImmutableCopyOnWriteDictionary<TKey, TValue> RemoveRange(IEnumerable<TKey> keys)
    {
        var builder = ToBuilder();
        foreach (var key in keys)
            builder.Remove(key);
        return builder.ToImmutable();
    }

    
    public ImmutableCopyOnWriteDictionary<TKey, TValue> SetItem(TKey key, TValue value)
    {
        var builder = ToBuilder();
        builder[key] = value;
        return builder.ToImmutable();
    }

    public ImmutableCopyOnWriteDictionary<TKey, TValue> SetItems(IEnumerable<KeyValuePair<TKey, TValue>> items)
    {
        var builder = ToBuilder();
        foreach(var kvp in items)
            builder[kvp.Key] = kvp.Value;
        return builder.ToImmutable();
    }
    
#if NET5_0_OR_GREATER
    IImmutableDictionary<TKey, TValue> IImmutableDictionary<TKey, TValue>.Add(TKey key, TValue value)
    {
        return Add(key, value);
    }
    
    IImmutableDictionary<TKey, TValue>IImmutableDictionary<TKey, TValue>.AddRange(IEnumerable<KeyValuePair<TKey, TValue>> pairs)
    {
        return AddRange(pairs);
    }
    
    IImmutableDictionary<TKey, TValue> IImmutableDictionary<TKey, TValue>.Clear()
    {
        return Clear();
    }
    
    IImmutableDictionary<TKey, TValue> IImmutableDictionary<TKey, TValue>.Remove(TKey key)
    {
        var builder = ToBuilder();
        builder.Remove(key);
        return builder.ToImmutable();
    }
    
    IImmutableDictionary<TKey, TValue> IImmutableDictionary<TKey, TValue>.RemoveRange(IEnumerable<TKey> keys)
    {
        return RemoveRange(keys);
    }
    
    
    IImmutableDictionary<TKey, TValue> IImmutableDictionary<TKey,TValue>.SetItem(TKey key, TValue value)
    {
        return SetItem(key, value);
    }

    IImmutableDictionary<TKey, TValue> IImmutableDictionary<TKey, TValue>.SetItems(IEnumerable<KeyValuePair<TKey, TValue>> items)
    {
        var builder = ToBuilder();
        foreach(var (key, value) in items)
            builder[key] = value;
        return builder.ToImmutable();
    }

    /// <summary>
    /// Careful: O(N) operation
    /// </summary>
    bool IImmutableDictionary<TKey,TValue>.TryGetKey(TKey equalKey, out TKey actualKey)
    {
        if (ContainsKey(equalKey))
        {
            foreach (var key in _source.Keys)
            {
                if (_source.Comparer.Equals(key, equalKey))
                {
                    actualKey = key;
                    return true;
                }
            }
        }

        actualKey = equalKey;
        return false;
    }
#endif
}