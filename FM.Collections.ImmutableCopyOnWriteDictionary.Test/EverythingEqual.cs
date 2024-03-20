using System.Collections;

namespace FM.Collections.Test;
/// <summary>
/// An equality comparer that considers all values to be equal.
/// </summary>
/// <typeparam name="T"></typeparam>
internal class EverythingEqual<T> : IEqualityComparer<T>, IEqualityComparer
{
    internal static readonly EverythingEqual<T> Default = new EverythingEqual<T>();

    private EverythingEqual() { }

    public bool Equals(T? x, T? y)
    {
        return true;
    }

    public int GetHashCode(T obj)
    {
        return 1;
    }

    bool IEqualityComparer.Equals(object? x, object? y)
    {
        return true;
    }

    int IEqualityComparer.GetHashCode(object obj)
    {
        return 1;
    }
}