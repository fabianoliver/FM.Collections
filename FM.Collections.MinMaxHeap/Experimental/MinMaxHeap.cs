using System.Collections;
using System.Collections.Generic;

namespace FM.Collections.Experimental;

public sealed class ExperimentalMinMaxHeap<TComparer, T> : MinMaxHeapBase<Arity.Two, TComparer, T>, IMinMaxHeap<T>
    where TComparer : IComparer<T>
{
    IComparer<T> IMinMaxHeap<T>.Comparer => Comparer;
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
    public ExperimentalMinMaxHeap(TComparer comparer, int capacity = -1) : base(comparer, capacity)
    {
    }
    
    public ExperimentalMinMaxHeap(TComparer comparer, IEnumerable<T> items) : base(comparer, items)
    {
    }
}