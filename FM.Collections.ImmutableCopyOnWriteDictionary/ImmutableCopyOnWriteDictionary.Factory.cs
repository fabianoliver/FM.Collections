using System.Collections.Generic;

namespace FM.Collections;

public static class ImmutableCopyOnWriteDictionary
{
    public static ImmutableCopyOnWriteDictionary<TKey, TValue> Create<TKey, TValue>(IEqualityComparer<TKey>? keyComparer = null, IEqualityComparer<TValue>? valueComparer = null)
        where TKey : notnull
    {
        if(keyComparer == null && valueComparer == null)
            return ImmutableCopyOnWriteDictionary<TKey, TValue>.Empty;
        
        return new ImmutableCopyOnWriteDictionary<TKey, TValue>(keyComparer, valueComparer);
    }
    
    public static ImmutableCopyOnWriteDictionary<TKey, TValue> CreateRange<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> items) where TKey : notnull
    {
        return ImmutableCopyOnWriteDictionary<TKey, TValue>.Empty.AddRange(items);
    }
    
    public static ImmutableCopyOnWriteDictionary<TKey, TValue> CreateRange<TKey, TValue>(IEqualityComparer<TKey>? keyComparer, IEnumerable<KeyValuePair<TKey, TValue>> items) where TKey : notnull
    {
        return ImmutableCopyOnWriteDictionary<TKey, TValue>.Empty.WithComparers(keyComparer).AddRange(items);
    }

    public static ImmutableCopyOnWriteDictionary<TKey, TValue> CreateRange<TKey, TValue>(IEqualityComparer<TKey>? keyComparer, IEqualityComparer<TValue>? valueComparer, IEnumerable<KeyValuePair<TKey, TValue>> items) where TKey : notnull
    {
        return ImmutableCopyOnWriteDictionary<TKey, TValue>.Empty.WithComparers(keyComparer, valueComparer).AddRange(items);
    }

    public static ImmutableCopyOnWriteDictionary<TKey, TValue>.Builder CreateBuilder<TKey, TValue>(IEqualityComparer<TKey>? keyComparer = null, IEqualityComparer<TValue>? valueComparer = null) where TKey : notnull
    {
        if (keyComparer is null && valueComparer is null)
            return ImmutableCopyOnWriteDictionary<TKey, TValue>.Empty.ToBuilder();
        
        return new ImmutableCopyOnWriteDictionary<TKey, TValue>(keyComparer, valueComparer).ToBuilder();
    }
}