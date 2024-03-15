using System;
using System.Collections.Generic;
using FM.Collections.Comparers;

namespace FM.Collections;

public static class MinMaxHeapDictionary
{
    public static MinMaxHeapDictionary<Arity.Two, TComparer, TKey, TValue> Create<TComparer, TKey, TValue>(TComparer comparer, IEqualityComparer<TKey>? keyComparer = null, int initialCapacity = -1)
        where TComparer : IComparer<KeyValuePair<TKey, TValue>>
        where TKey : notnull
    {
        return new MinMaxHeapDictionary<Arity.Two, TComparer, TKey, TValue>(comparer, keyComparer, -1);
    }
    
    public static MinMaxHeapDictionary<Arity.Two, TComparer, TKey, TValue> Create<TComparer, TKey, TValue>(TComparer comparer, IEnumerable<KeyValuePair<TKey, TValue>> items, IEqualityComparer<TKey>? keyComparer = null)
        where TComparer : IComparer<KeyValuePair<TKey, TValue>>
        where TKey : notnull
    {
        return new MinMaxHeapDictionary<Arity.Two, TComparer, TKey, TValue>(comparer, items, keyComparer);
    }
    
    public static MinMaxHeapDictionary<Arity.Two, KeyValuePairByComparableValueComparer<TKey, TValue>, TKey, TValue> CreateComparingValues<TKey, TValue>(IEqualityComparer<TKey>? keyComparer = null, int initialCapacity = -1)
        where TValue : IComparable<TValue>
        where TKey : notnull
    {
        return new MinMaxHeapDictionary<Arity.Two, KeyValuePairByComparableValueComparer<TKey, TValue>, TKey, TValue>(default, keyComparer, -1);
    }
    
    public static MinMaxHeapDictionary<Arity.Two, KeyValuePairByComparableValueComparer<TKey, TValue>, TKey, TValue> CreateComparingValues<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> items, IEqualityComparer<TKey>? keyComparer = null)
        where TValue : IComparable<TValue>
        where TKey : notnull
    {
        return new MinMaxHeapDictionary<Arity.Two, KeyValuePairByComparableValueComparer<TKey, TValue>, TKey, TValue>(default, items, keyComparer);
    }
    
    public static MinMaxHeapDictionary<Arity.Two, KeyValuePairByValueComparer<TComparer, TKey, TValue>, TKey, TValue> CreateComparingValues<TComparer, TKey, TValue>(TComparer valueComparer, IEqualityComparer<TKey>? keyComparer = null, int initialCapacity = -1)
        where TComparer : IComparer<TValue>
        where TKey : notnull
    {
        return new MinMaxHeapDictionary<Arity.Two, KeyValuePairByValueComparer<TComparer, TKey, TValue>, TKey, TValue>(new(valueComparer), keyComparer, -1);
    }
    
    public static MinMaxHeapDictionary<Arity.Two, KeyValuePairByValueComparer<TComparer, TKey, TValue>, TKey, TValue> CreateComparingValues<TComparer, TKey, TValue>(TComparer valueComparer, IEnumerable<KeyValuePair<TKey,TValue>> items, IEqualityComparer<TKey>? keyComparer = null)
        where TComparer : IComparer<TValue>
        where TKey : notnull
    {
        return new MinMaxHeapDictionary<Arity.Two, KeyValuePairByValueComparer<TComparer, TKey, TValue>, TKey, TValue>(new(valueComparer), items, keyComparer);
    }

    
    public static MinMaxHeapDictionary<Arity.Two, KeyValuePairByComparableKeyComparer<TKey, TValue>, TKey, TValue> CreateComparingKeys<TKey, TValue>(IEqualityComparer<TKey>? keyComparer = null, int initialCapacity = -1)
        where TKey : IComparable<TKey>
    {
        return new MinMaxHeapDictionary<Arity.Two, KeyValuePairByComparableKeyComparer<TKey, TValue>, TKey, TValue>(default, keyComparer, -1);
    }
    
    public static MinMaxHeapDictionary<Arity.Two, KeyValuePairByComparableKeyComparer<TKey, TValue>, TKey, TValue> CreateComparingKeys<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> items, IEqualityComparer<TKey>? keyComparer = null)
        where TKey : IComparable<TKey>
    {
        return new MinMaxHeapDictionary<Arity.Two, KeyValuePairByComparableKeyComparer<TKey, TValue>, TKey, TValue>(default, items, keyComparer);
    }
    
    public static MinMaxHeapDictionary<Arity.Two, KeyValuePairByKeyComparer<TComparer, TKey, TValue>, TKey, TValue> CreateComparingKeys<TComparer, TKey, TValue>(TComparer valueComparer, IEqualityComparer<TKey>? keyComparer = null, int initialCapacity = -1)
        where TComparer : IComparer<TKey>
        where TKey : notnull
    {
        return new MinMaxHeapDictionary<Arity.Two, KeyValuePairByKeyComparer<TComparer, TKey, TValue>, TKey, TValue>(new(valueComparer), keyComparer, -1);
    }
    
    public static MinMaxHeapDictionary<Arity.Two, KeyValuePairByKeyComparer<TComparer, TKey, TValue>, TKey, TValue> CreateComparingKeys<TComparer, TKey, TValue>(TComparer valueComparer, IEnumerable<KeyValuePair<TKey,TValue>> items, IEqualityComparer<TKey>? keyComparer = null)
        where TComparer : IComparer<TKey>
        where TKey : notnull
    {
        return new MinMaxHeapDictionary<Arity.Two, KeyValuePairByKeyComparer<TComparer, TKey, TValue>, TKey, TValue>(new(valueComparer), items, keyComparer);
    }
}