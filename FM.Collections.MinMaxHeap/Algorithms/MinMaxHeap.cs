using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace FM.Collections.Algorithms;

internal static class MinMaxHeap
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsMinLevel<TArity>(int index) where TArity : struct, IConstInt
    {
        return (Heap.GetLevel<TArity>(index) & 1) == 0;
    }
    
    public static bool IsMinMaxHeap<T, TArity>(IReadOnlyList<T> items, IComparer<T> comparer, TArity witness = default)
        where TArity : struct, IConstInt
    {
        if (items.Count <= 1)
            return true;

        for (var i = 0; i < items.Count; i++)
        {
            if (Heap.GetFirstChildIndex(new TArity().Value, i) >= items.Count)
                // No children
                break;

            if (IsMinLevel<TArity>(i))
            {
                for (var child = Heap.GetFirstChildIndex(new TArity().Value, i); child <= Heap.GetLastChildIndex(new TArity().Value, i); child++)
                {
                    if (child >= items.Count)
                        break;
                    if (!(comparer.Compare(items[i], items[child]) <= 0))
                        return false;
                }
                
                for (var grandchild = Heap.GetFirstGrandChildIndex(new TArity().Value, i); grandchild <= Heap.GetLastGrandChildIndex(new TArity().Value, i); grandchild++)
                {
                    if (grandchild >= items.Count)
                        break;
                    if (!(comparer.Compare(items[i], items[grandchild]) <= 0))
                        return false;
                }
            }
            else
            {
                for (var child = Heap.GetFirstChildIndex(new TArity().Value, i); child <= Heap.GetLastChildIndex(new TArity().Value, i); child++)
                {
                    if (child >= items.Count)
                        break;
                    if (!(comparer.Compare(items[i], items[child]) >= 0))
                        return false;
                }
                
                for (var grandchild = Heap.GetFirstGrandChildIndex(new TArity().Value, i); grandchild <= Heap.GetLastGrandChildIndex(new TArity().Value, i); grandchild++)
                {
                    if (grandchild >= items.Count)
                        break;
                    if (!(comparer.Compare(items[i], items[grandchild]) >= 0))
                        return false;
                }
            }
        }

        return true;
    }
}