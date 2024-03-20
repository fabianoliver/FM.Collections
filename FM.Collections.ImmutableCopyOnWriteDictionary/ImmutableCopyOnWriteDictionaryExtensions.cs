using System;
using System.Collections.Generic;
using System.Linq;

namespace FM.Collections;

public static class ImmutableCopyOnWriteDictionaryExtensions
{
    public static ImmutableCopyOnWriteDictionary<TKey, TValue> ToImmutableCopyOnWriteDictionary<TKey, TValue>(
        this IEnumerable<KeyValuePair<TKey, TValue>> items,
        IEqualityComparer<TKey>? keyComparer = null,
        IEqualityComparer<TValue>? valueComparer = null) where TKey : notnull
    {
        if (items is ImmutableCopyOnWriteDictionary<TKey, TValue> immutable &&
            ReferenceEquals(keyComparer ?? EqualityComparer<TKey>.Default, immutable.KeyComparer) &&
            ReferenceEquals(valueComparer ?? EqualityComparer<TValue>.Default, immutable.ValueComparer))
            return immutable;
        
        return ImmutableCopyOnWriteDictionary.CreateRange(keyComparer, valueComparer, items);
    }
    
    public static ImmutableCopyOnWriteDictionary<TKey, TSource> ToImmutableCopyOnWriteDictionary<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        IEqualityComparer<TKey>? keyComparer = null,
        IEqualityComparer<TSource>? valueComparer = null) where TKey : notnull
    {
        return ToImmutableCopyOnWriteDictionary(source, keySelector, v => v, keyComparer, valueComparer);
    }
    
    public static ImmutableCopyOnWriteDictionary<TKey, TValue> ToImmutableCopyOnWriteDictionary<TSource, TKey, TValue>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        Func<TSource, TValue> elementSelector,
        IEqualityComparer<TKey>? keyComparer = null,
        IEqualityComparer<TValue>? valueComparer = null) where TKey : notnull
    {
        return ImmutableCopyOnWriteDictionary<TKey, TValue>
            .Empty
            .WithComparers(keyComparer, valueComparer)
            .AddRange(source
                .Select(element => new KeyValuePair<TKey, TValue>(
                    keySelector(element),
                    elementSelector(element))));
    }
}