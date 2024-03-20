using System.Collections;
using System.Collections.Generic;

namespace FM.Collections;

public sealed partial class ImmutableCopyOnWriteDictionary<TKey, TValue> where TKey : notnull
{
    public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
    {
        private readonly Dictionary<TKey, TValue> _sourceDict;
        private Dictionary<TKey, TValue>.Enumerator _source;

        internal Enumerator(Dictionary<TKey, TValue> sourceDict)
        {
            _source = sourceDict.GetEnumerator();
            _sourceDict = sourceDict;
        }

        public void Dispose()
        {
            _source.Dispose();
        }

        public bool MoveNext()
        {
            return _source.MoveNext();
        }

        public void Reset()
        {
            _source = _sourceDict.GetEnumerator();
        }

        public KeyValuePair<TKey, TValue> Current => _source.Current;

        object IEnumerator.Current => Current;
    }
}