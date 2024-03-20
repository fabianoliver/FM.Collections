using System;
using System.Collections;
using System.Collections.Generic;

namespace FM.Collections;

public sealed partial class ImmutableCopyOnWriteDictionary<TKey, TValue> where TKey : notnull
{
    public sealed partial class Builder
    {
        public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
        {
            private readonly Builder _source;
            private bool _hasChanges;
            private Dictionary<TKey, TValue>.Enumerator _enumerator;

            public Enumerator(Builder source)
            {
                _source = source;
                Reset();
            }


            public bool MoveNext()
            {
                if (BuilderHasChanges() != _hasChanges)
                    throw new InvalidOperationException("The underlying collection has been modified while iterating");
                return _enumerator.MoveNext();
            }

            public void Reset()
            {
                _hasChanges = BuilderHasChanges();
                _enumerator = (_source._cloned ?? _source._parent._source).GetEnumerator();
            }

            public KeyValuePair<TKey, TValue> Current => _enumerator.Current;

            object IEnumerator.Current => Current;

            public void Dispose()
            {
            }
            
            private bool BuilderHasChanges() => _source._cloned is not null;
        }
    }
}