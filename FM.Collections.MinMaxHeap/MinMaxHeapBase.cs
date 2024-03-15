using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using FM.Collections.Algorithms;
using Math = System.Math;

namespace FM.Collections;

public abstract class MinMaxHeapBase<TArity, T>
    where TArity : struct, IConstInt
{
    private T[] _values;
    private int _count;

    public int Count => _count;
    public bool IsReadOnly => false;

    internal IReadOnlyList<T> RawValues => new ArraySegment<T>(_values, 0, _count);

    protected MinMaxHeapBase(int capacity = -1)
    {
        if (capacity < 0)
            capacity = 1024;
        _values = new T[Math.Max(capacity, 1)];
    }

    protected void InitializeWith(IEnumerable<T> items)
    {
#if NET6_0_OR_GREATER
        if(items.TryGetNonEnumeratedCount(out var count))
            EnsureCapacity(count);
#else
        if (items is ICollection<T> collection)
            EnsureCapacity(collection.Count);
        else if (items is IReadOnlyCollection<T> readOnlyCollection)
            EnsureCapacity(readOnlyCollection.Count);
        else if (items is IReadOnlyList<T> readOnlyList)
            EnsureCapacity(readOnlyList.Count);
#endif

        foreach (var item in items)
            AddToArray(item);

        for (var i = _count / 2; i >= 0; i--)
            PushDown(i);
    }

    public virtual void Add(T item)
    {
        AddToArray(item);
        PushUp(Count - 1);
    }

    private void AddToArray(T item)
    {
        EnsureCapacity(_count + 1);
        _values[_count++] = item;
    }

    private void EnsureCapacity(int minCapacity)
    {
        if (_values.Length >= minCapacity)
            return;

        if (minCapacity < _count)
            throw new ArgumentException("Capacity must be >= Count", nameof(minCapacity));

        var newValues = new T[_values.Length * 2];
        Array.Copy(_values, newValues, _count);
        _values = newValues;
    }

    private T RemoveLastFromArray()
    {
        var result = _values[--_count];
        _values[_count] = default!;

        if (_values.Length > 3 * _count)
        {
            var newValues = new T[_count];
            Array.Copy(_values, newValues, _count);
            _values = newValues;
        }

        return result;
    }

    protected abstract int Compare(T left, T right);

    protected internal virtual T this[int index]
    {
        get { return _values[index]; }
        set
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException();

            var prior = _values[index];
            SetValue(_values, index, value);

            if (Compare(prior, value) == 0)
                // The priority of the item hasn't changed
                return;

            if (index == 0)
            {
                // Root of min layer
                PushDownMin(index);
            }
            else if (index <= new TArity().Value)
            {
                // Root of max layer
                EnsureConsistencyWithParent(index);
                PushDownMax(index);
            }
            else if (Heap.GetFirstChildIndex(new TArity().Value, index) >= Count)
            {
                // We're a leaf node, so we can just treat this similar to a regular insertion and sift up
                PushUp(index);
            }
            else if (Algorithms.MinMaxHeap.IsMinLevel<TArity>(index) && Compare(_values[index], _values[Heap.GetGrandParentIndex(new TArity().Value, index)]) < 0)
            {
                // Min level, and our value is smaller than our grandparent - we can just sift up
                PushUpMin(index);
            }
            else if (!Algorithms.MinMaxHeap.IsMinLevel<TArity>(index) && Compare(_values[index], _values[Heap.GetGrandParentIndex(new TArity().Value, index)]) > 0)
            {
                // Max level, and our value is greater than our grandparent - we can just sift up
                PushUpMax(index);
            }
            else
            {
                EnsureConsistencyWithParent(index);
                PushDown(index);
            }
        }
    }

    public void Clear()
    {
        _count = 0;
    }

    public bool Contains(T item)
    {
        return new ArraySegment<T>(_values, 0, _count).Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        Array.Copy(_values, 0, array, arrayIndex, _count);
    }

    public bool Remove(T item)
    {
        var index = Array.IndexOf(_values, item);

        if (index < 0)
            return false;

        Remove(index);
        return true;
    }

    public T RemoveMin() => Remove(MinIndex);
    public T RemoveMax() => Remove(MaxIndex);

    protected internal virtual T Remove(int index)
    {
        if (index < 0 || index >= Count)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (_count == 1)
        {
            // We're removing the only available value in the dictionary
            var result = _values[0];
            Clear();
            return result;
        }

        if (index == _count - 1)
        {
            // We're removing the last value in the list, no need to swap in anything else
            var result = _values[index];
            RemoveLastFromArray();
            return result;
        }

        var value = _values[index];
        var replacement = RemoveLastFromArray();
        this[index] = replacement;
        return value;
    }


    private void EnsureConsistencyWithParent(int index)
    {
        var values = _values;
        var parent = Heap.GetParentIndex(new TArity().Value, index);

        if (Algorithms.MinMaxHeap.IsMinLevel<TArity>(index))
        {
            if (Compare(values[index], values[parent]) > 0)
            {
                Swap(values, index, parent);
                PushUpMax(parent);
            }
        }
        else
        {
            if (Compare(_values[index], _values[parent]) < 0)
            {
                Swap(values, index, parent);
                PushUpMin(parent);
            }
        }
    }

    public T Min => _values[MinIndex];
    public T Max => _values[MaxIndex];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Swap(T[] values, int i, int j)
    {
        var tmp = values[i];
        SetValue(values, i, values[j]);
        SetValue(values, j, tmp);
    }


    internal int MinIndex => Count switch
    {
        0 => throw new InvalidOperationException("Heap is empty"),
        _ => 0
    };

    internal int MaxIndex
    {
        get
        {
            if (Count == 0)
                throw new InvalidOperationException("Heap is empty");
            if (Count == 1)
                return 0;
            if (Count == 2)
                return 1;

            var values = _values;
            var result = 1;
            var end = Math.Min(new TArity().Value, Count - 1);
            for (var i = 2; i <= end; i++)
            {
                if (Compare(values[i], values[result]) > 0)
                    result = i;
            }

            return result;
        }
    }

    private void PushDown(int index)
    {
        if (Algorithms.MinMaxHeap.IsMinLevel<TArity>(index))
            PushDownMin(index);
        else
            PushDownMax(index);
    }

    private int IndexOfMinChildOrGrandchild(T[] values, int count, int index)
    {
        var firstChild = Heap.GetFirstChildIndex(new TArity().Value, index);
        var lastChild = Heap.GetLastChildIndex(new TArity().Value, index);

        var firstGrandChild = Heap.GetFirstGrandChildIndex(new TArity().Value, index);
        var lastGrandChild = Heap.GetLastGrandChildIndex(new TArity().Value, index);

        var result = firstChild;

        for (var i = firstChild + 1; i <= lastChild; i++)
        {
            if (i >= count)
                return result;
            if (Compare(values[i], values[result]) < 0)
                result = i;
        }

        for (var i = firstGrandChild; i <= lastGrandChild; i++)
        {
            if (i >= count)
                return result;
            if (Compare(values[i], values[result]) < 0)
                result = i;
        }

        return result;
    }

    private int IndexOfMaxChildOrGrandchild(T[] values, int count, int index)
    {
        var firstChild = Heap.GetFirstChildIndex(new TArity().Value, index);
        var lastChild = Heap.GetLastChildIndex(new TArity().Value, index);

        var firstGrandChild = Heap.GetFirstGrandChildIndex(new TArity().Value, index);
        var lastGrandChild = Heap.GetLastGrandChildIndex(new TArity().Value, index);

        var result = firstChild;

        for (var i = firstChild + 1; i <= lastChild; i++)
        {
            if (i >= count)
                return result;
            if (Compare(values[i], values[result]) > 0)
                result = i;
        }

        for (var i = firstGrandChild; i <= lastGrandChild; i++)
        {
            if (i >= count)
                return result;
            if (Compare(values[i], values[result]) > 0)
                result = i;
        }

        return result;
    }

    private void PushDownMin(int index)
    {
        var values = _values;
        var count = _count;
        var value = values[index];

        while (true)
        {
            var firstGrandChild = Heap.GetFirstGrandChildIndex(new TArity().Value, index);
            var lastGrandchild = Heap.GetLastGrandChildIndex(new TArity().Value, index);

            int indexOfMinChildOrGrandChild;
            bool isGrandChild;

            if (lastGrandchild < count)
            {
                // This node has all children & grand children
                // Since we're in a min layer, we know its smallest descendant will be a grandchild
                // So we don't need to check the immediate child layer
                indexOfMinChildOrGrandChild = MinAmongAllGrandChildren(values, firstGrandChild, lastGrandchild);
                isGrandChild = true;
            }
            else if (Heap.GetFirstChildIndex(new TArity().Value, index) >= count)
            {
                break;
            }
            else
            {
                indexOfMinChildOrGrandChild = IndexOfMinChildOrGrandchild(values, count, index);
                isGrandChild = indexOfMinChildOrGrandChild >= firstGrandChild && indexOfMinChildOrGrandChild <= lastGrandchild;
            }

            if (isGrandChild)
            {
                var parent = Heap.GetParentIndex(new TArity().Value, indexOfMinChildOrGrandChild);

                if (Compare(values[indexOfMinChildOrGrandChild], value) < 0)
                {
                    SetValue(values, index, values[indexOfMinChildOrGrandChild]);

                    if (Compare(value, values[parent]) > 0)
                    {
                        var tmp = value;
                        value = values[parent];
                        SetValue(values, parent, tmp);
                    }

                    index = indexOfMinChildOrGrandChild;
                    continue;
                }
            }
            else
            {
                if (Compare(values[indexOfMinChildOrGrandChild], value) < 0)
                {
                    SetValue(values, index, values[indexOfMinChildOrGrandChild]);
                    index = indexOfMinChildOrGrandChild;
                }
            }

            break;
        }

        SetValue(values, index, value);
    }

    private void PushDownMax(int index)
    {
        var values = _values;
        var count = _count;
        var value = values[index];

        while (true)
        {
            var firstGrandChild = Heap.GetFirstGrandChildIndex(new TArity().Value, index);
            var lastGrandchild = Heap.GetLastGrandChildIndex(new TArity().Value, index);
            int indexOfMaxChildOrGrandchild;
            bool isGrandChild;

            if (lastGrandchild < count)
            {
                // This node has all children & grand children
                // Since we're in a min layer, we know its smallest descendant will be a grandchild
                // So we don't need to check the immediate child layer
                indexOfMaxChildOrGrandchild = MaxAmongAllGrandChildren(values, firstGrandChild, lastGrandchild);
                isGrandChild = true;
            }
            else if (Heap.GetFirstChildIndex(new TArity().Value, index) >= count)
            {
                break;
            }
            else
            {
                indexOfMaxChildOrGrandchild = IndexOfMaxChildOrGrandchild(values, count, index);
                isGrandChild = indexOfMaxChildOrGrandchild >= firstGrandChild && indexOfMaxChildOrGrandchild <= lastGrandchild;
            }

            if (isGrandChild)
            {
                var parent = Heap.GetParentIndex(new TArity().Value, indexOfMaxChildOrGrandchild);

                if (Compare(values[indexOfMaxChildOrGrandchild], value) > 0)
                {
                    SetValue(values, index, values[indexOfMaxChildOrGrandchild]);

                    if (Compare(value, values[parent]) < 0)
                    {
                        var tmp = value;
                        value = values[parent];
                        SetValue(values, parent, tmp);
                    }

                    index = indexOfMaxChildOrGrandchild;
                    continue;
                }
            }
            else
            {
                if (Compare(values[indexOfMaxChildOrGrandchild], value) > 0)
                {
                    SetValue(values, index, values[indexOfMaxChildOrGrandchild]);
                    index = indexOfMaxChildOrGrandchild;
                }
            }

            break;
        }

        SetValue(values, index, value);
    }

    private void PushUp(int index)
    {
        if (index == 0)
            return;

        var values = _values;
        var parent = Heap.GetParentIndex(new TArity().Value, index);

        if (Algorithms.MinMaxHeap.IsMinLevel<TArity>(index))
        {
            if (Compare(values[index], values[parent]) > 0)
            {
                Swap(values, index, parent);
                PushUpMax(parent);
            }
            else
            {
                PushUpMin(index);
            }
        }
        else
        {
            if (Compare(values[index], values[parent]) < 0)
            {
                Swap(values, index, parent);
                PushUpMin(parent);
            }
            else
            {
                PushUpMax(index);
            }
        }
    }

    private void PushUpMin(int index)
    {
        var values = _values;
        var value = values[index];

        while (index > 0)
        {
            var grandparentIndex = Heap.GetGrandParentIndex(new TArity().Value, index);
            var grandparent = values[grandparentIndex];

            if (Compare(value, grandparent) < 0)
            {
                SetValue(values, index, grandparent);
                index = grandparentIndex;
            }
            else
            {
                break;
            }
        }

        SetValue(values, index, value);
    }

    private void PushUpMax(int index)
    {
        var values = _values;
        var value = values[index];

        while (index > new TArity().Value)
        {
            var grandparentIndex = Heap.GetGrandParentIndex(new TArity().Value, index);
            var grandparent = values[grandparentIndex];

            if (Compare(value, grandparent) > 0)
            {
                SetValue(values, index, grandparent);
                index = grandparentIndex;
            }
            else
            {
                break;
            }
        }

        SetValue(values, index, value);
    }

    protected virtual void SetValue(T[] values, int index, T newValue)
    {
        values[index] = newValue;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MinAmongAllGrandChildren(T[] values, int iFirstGrandChild, int iLastGrandChild)
    {
        var result = iFirstGrandChild;

        for (var i = iFirstGrandChild + 1; i <= iLastGrandChild; i++)
        {
            if (Compare(values[i], values[result]) < 0)
                result = i;
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MaxAmongAllGrandChildren(T[] values, int iFirstGrandChild, int iLastGrandChild)
    {
        var result = iFirstGrandChild;

        for (var i = iFirstGrandChild + 1; i <= iLastGrandChild; i++)
        {
            if (Compare(values[i], values[result]) > 0)
                result = i;
        }

        return result;
    }


    public IEnumerator<T> GetEnumerator() => _values.Take(_count).GetEnumerator();
}

public abstract class MinMaxHeapBase<TArity, TComparer, T> : MinMaxHeapBase<TArity, T>
    where TArity : struct, IConstInt
    where TComparer : IComparer<T>
{
    private readonly TComparer _comparer;

    public TComparer Comparer => _comparer;

    protected MinMaxHeapBase(TComparer comparer, int capacity = -1) : base(capacity)
    {
        _comparer = comparer;
    }

    protected MinMaxHeapBase(TComparer comparer, IEnumerable<T> items)
    {
        _comparer = comparer;
        InitializeWith(items);
    }

    protected override int Compare(T left, T right)
    {
        return _comparer.Compare(left, right);
    }
}