namespace FM.Collections.Test;

internal class SameHashForEverythingComparer<T> : IEqualityComparer<T>
{
    private readonly IEqualityComparer<T> _equalityComparer;

    internal SameHashForEverythingComparer(IEqualityComparer<T>? equalityComparer = null)
    {
        _equalityComparer = equalityComparer ?? EqualityComparer<T>.Default;
    }

    public bool Equals(T? x, T? y)
    {
        return _equalityComparer.Equals(x, y);
    }

    public int GetHashCode(T obj)
    {
        return 1;
    }
}