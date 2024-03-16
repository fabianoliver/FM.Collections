using System.Collections.Generic;

namespace FM.Collections;

/// <summary>
/// A dictionary that provides O(1) access to its minimum and maximum element, whereby this order is defined by a custom comparer.
/// O(N) in memory.
/// Insert, delete and update operations are all worst case O(logN).
/// This differs from standard BCL sorted dictionaries mainly in so far as that sort order can depend on the value of an entry, not just on its key.
/// </summary>
/// <typeparam name="TArity">Type of the arity. See <see cref="Arity"/> - but feel free to add your own if needed. Used in lieu of const generics to allow the JIT to better optimise for specific arity.</typeparam>
/// <typeparam name="TComparer">Type of the comparer. Can just be <see cref="IComparer{T}"/>, but can also be something more specific to allow the JIT to better optimise for any specific implementation.</typeparam>
/// <typeparam name="TKey">Key type</typeparam>
/// <typeparam name="TValue">Value type</typeparam>

public sealed class MinMaxHeapDictionary<TArity, TComparer, TKey, TValue> : MinMaxHeapDictionaryBase<TArity, TComparer, TKey, TValue>
    where TArity : struct, IConstInt
    where TComparer : IComparer<KeyValuePair<TKey, TValue>>
    where TKey : notnull
{
    public MinMaxHeapDictionary(TComparer comparer, IEqualityComparer<TKey>? keyComparer = null, int initialCapacity = -1)
        : base(comparer, keyComparer, initialCapacity)
    {
    }

    public MinMaxHeapDictionary(TComparer comparer, IEnumerable<KeyValuePair<TKey, TValue>> values, IEqualityComparer<TKey>? keyComparer = null)
        : base(comparer, values, keyComparer)
    {
    }
    
    public MinMaxHeapDictionary(MinMaxHeapDictionary<TArity, TComparer, TKey, TValue> self) : base(self)
    {
    }
}

public sealed class MinMaxHeapDictionary<TComparer, TKey, TValue> : MinMaxHeapDictionaryBase<Arity.Two, TComparer, TKey, TValue>
    where TKey : notnull
    where TComparer : IComparer<KeyValuePair<TKey, TValue>>

{
    public MinMaxHeapDictionary(TComparer comparer, IEqualityComparer<TKey>? keyComparer = null, int initialCapacity = -1)
        : base(comparer, keyComparer, initialCapacity)
    {
    }

    public MinMaxHeapDictionary(TComparer comparer, IEnumerable<KeyValuePair<TKey, TValue>> values, IEqualityComparer<TKey>? keyComparer = null)
        : base(comparer, values, keyComparer)
    {
    }
    
    public MinMaxHeapDictionary(MinMaxHeapDictionary<TComparer, TKey, TValue> self) : base(self)
    {
    }
}

public sealed class MinMaxHeapDictionary<TKey, TValue> : MinMaxHeapDictionaryBase<Arity.Two, IComparer<KeyValuePair<TKey, TValue>>, TKey, TValue>
    where TKey : notnull
{
    public MinMaxHeapDictionary(IComparer<KeyValuePair<TKey, TValue>> comparer, IEqualityComparer<TKey>? keyComparer = null, int initialCapacity = -1)
        : base(comparer, keyComparer, initialCapacity)
    {
    }

    public MinMaxHeapDictionary(IComparer<KeyValuePair<TKey, TValue>> comparer, IEnumerable<KeyValuePair<TKey, TValue>> values, IEqualityComparer<TKey>? keyComparer = null)
        : base(comparer, values, keyComparer)
    {
    }

    public MinMaxHeapDictionary(MinMaxHeapDictionary<TKey, TValue> self) : base(self)
    {
    }
}