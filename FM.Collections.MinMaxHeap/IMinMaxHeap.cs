using System.Collections.Generic;

namespace FM.Collections;

public interface IMinMaxHeap<T> : ICollection<T>
{
    IComparer<T> Comparer { get; }
    T Min { get; }
    T Max { get; }
    T RemoveMin();
    T RemoveMax();
}