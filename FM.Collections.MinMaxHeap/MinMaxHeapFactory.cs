using System;
using System.Collections.Generic;
using FM.Collections.Comparers;

namespace FM.Collections;

public static class MinMaxHeap
{
    public static MinMaxHeap<ComparableComparer<T>, T> Create<T>(int capacity = -1)
        where T : IComparable<T>
    {
        return new MinMaxHeap<ComparableComparer<T>, T>(default, capacity);
    }
    
    public static MinMaxHeap<ComparableComparer<T>, T> Create<T>(IEnumerable<T> items)
        where T : IComparable<T>
    {
        return new MinMaxHeap<ComparableComparer<T>, T>( default, items);
    }

    public static MinMaxHeap<TComparer, T> Create<TComparer, T>(TComparer comparer, int capacity = -1)
        where TComparer : IComparer<T>
    {
        return new MinMaxHeap<TComparer, T>(comparer, capacity);
    }
    
    public static MinMaxHeap<TComparer, T> Create<TComparer, T>(TComparer comparer, IEnumerable<T> items)
        where TComparer : IComparer<T>
    {
        return new MinMaxHeap<TComparer, T>(comparer, items);
    }
    
    public static MinMaxHeap<T> Create<T>(IComparer<T> comparer, int capacity = -1)
    {
        return new MinMaxHeap<T>(comparer, capacity);
    }
    
    public static MinMaxHeap<T> Create<T>(IComparer<T> comparer, IEnumerable<T> items)
    {
        return new MinMaxHeap<T>(comparer, items);
    }
}