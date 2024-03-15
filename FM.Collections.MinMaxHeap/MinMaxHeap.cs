using System.Collections;
using System.Collections.Generic;

namespace FM.Collections;

/// <summary>
/// A generic minmaxheap of configurable arity and comparer implementation type
/// </summary>
/// <typeparam name="TArity">Type of the arity. See <see cref="Arity"/> - but feel free to add your own if needed. Used in lieu of const generics to allow the JIT to better optimise for specific arity.</typeparam>
/// <typeparam name="TComparer">Type of the comparer. Can just be <see cref="IComparer{T}"/>, but can also be something more specific to allow the JIT to better optimise for any specific implementation.</typeparam>
/// <typeparam name="T">Element type</typeparam>
public sealed class MinMaxHeap<TArity, TComparer, T> : MinMaxHeapBase<TArity, TComparer, T>, IMinMaxHeap<T>, IReadOnlyCollection<T>
    where TArity : struct, IConstInt
    where TComparer : IComparer<T>
{
    IComparer<T> IMinMaxHeap<T>.Comparer => Comparer;
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
    public MinMaxHeap(TComparer comparer, int capacity = -1) : base(comparer, capacity)
    {
    }
    
    public MinMaxHeap(TComparer comparer, IEnumerable<T> items) : base(comparer, items)
    {
    }
}

/// <summary>
/// Two-ary minmax heap using a specific comparer type
/// </summary>
/// <typeparam name="TComparer">Type of the comparer. Can just be <see cref="IComparer{T}"/>, but can also be something more specific to allow the JIT to better optimise for any specific implementation.</typeparam>
/// <typeparam name="T">Element type</typeparam>
public sealed class MinMaxHeap<TComparer, T> : MinMaxHeapBase<Arity.Two, TComparer, T>, IMinMaxHeap<T>, IReadOnlyCollection<T>
    where TComparer : IComparer<T>
{
    IComparer<T> IMinMaxHeap<T>.Comparer => Comparer;
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
    public MinMaxHeap(TComparer comparer, int capacity = -1) : base(comparer, capacity)
    {
    }
    
    public MinMaxHeap(TComparer comparer, IEnumerable<T> items) : base(comparer, items)
    {
    }
}

/// <summary>
/// Two-ary minmax heap
/// </summary>
/// <typeparam name="T">Element type</typeparam>
public sealed class MinMaxHeap<T> : MinMaxHeapBase<Arity.Two, IComparer<T>, T>, IMinMaxHeap<T>, IReadOnlyCollection<T>
{
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public MinMaxHeap(IComparer<T> comparer, int capacity = -1) : base(comparer, capacity)
    {
    }
    
    public MinMaxHeap(IComparer<T> comparer, IEnumerable<T> items) : base(comparer, items)
    {
    }
}