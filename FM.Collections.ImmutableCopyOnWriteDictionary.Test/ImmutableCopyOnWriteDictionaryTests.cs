#if NET5_0_OR_GREATER
using System.Collections.Immutable;
#endif
namespace FM.Collections.Test;

public class ImmutableCopyOnWriteDictionaryTests
{
    [Test]
    public void test_empty()
    {
        Assert.That(ImmutableCopyOnWriteDictionary<string, int>.Empty, Is.SameAs(ImmutableCopyOnWriteDictionary<string, int>.Empty.Clear()));
        Assert.That(ImmutableCopyOnWriteDictionary<string, int>.Empty, Has.Count.EqualTo(0));
        Assert.That(ImmutableCopyOnWriteDictionary<string, int>.Empty.Count(), Is.EqualTo(0));
        Assert.That(ImmutableCopyOnWriteDictionary<string, int>.Empty.Keys.Count(), Is.EqualTo(0));
        Assert.That(ImmutableCopyOnWriteDictionary<string, int>.Empty.Values.Count(), Is.EqualTo(0));
        Assert.That(ImmutableCopyOnWriteDictionary<string, int>.Empty.KeyComparer, Is.SameAs(EqualityComparer<string>.Default));
        Assert.That(ImmutableCopyOnWriteDictionary<string, int>.Empty.ContainsKey("hello"), Is.False);
        Assert.That(ImmutableCopyOnWriteDictionary<string, int>.Empty.Contains(new KeyValuePair<string, int>("hello", 0)), Is.False);
        Assert.That(ImmutableCopyOnWriteDictionary<string, int>.Empty.TryGetValue("hello", out var value), Is.False);
        Assert.That(value, Is.EqualTo(0));
        
#if NET5_0_OR_GREATER
        Assert.That(ImmutableCopyOnWriteDictionary<string, int>.Empty.GetValueOrDefault("hello"), Is.EqualTo(0));
#endif
    }

    [Test]
    public void test_contains()
    {
        Assert.That(ImmutableCopyOnWriteDictionary<string, int>.Empty.Contains(new KeyValuePair<string, int>("hello", 1)), Is.False);
        Assert.That(ImmutableCopyOnWriteDictionary<string, int>.Empty.Add("hello", 1).Contains(new KeyValuePair<string, int>("hello", 1)), Is.True);
        
#if NET8_0_OR_GREATER
        Assert.That(ImmutableCopyOnWriteDictionary<string, int>.Empty.Contains("hello", 1), Is.False);
        Assert.That(ImmutableCopyOnWriteDictionary<string, int>.Empty.Add("hello", 1).Contains("hello", 1), Is.True);
#endif
    }

    [Test]
    public void test_remove()
    {
        Assert.That(ImmutableCopyOnWriteDictionary<string, int>.Empty.Remove("hello"), Is.SameAs(ImmutableCopyOnWriteDictionary<string, int>.Empty));
        Assert.That(ImmutableCopyOnWriteDictionary<string, int>.Empty.RemoveRange(Enumerable.Empty<string>()), Is.SameAs(ImmutableCopyOnWriteDictionary<string, int>.Empty));

        var added = ImmutableCopyOnWriteDictionary<string, int>.Empty.Add("hello", 0);
        var removed = added.Remove("hello");

        Assert.That(added, Is.Not.SameAs(removed));
        Assert.That(removed.ContainsKey("hello"), Is.False);
    }

    [Test]
    public void test_setItem()
    {
        var map = ImmutableCopyOnWriteDictionary<string, int>
            .Empty
            .SetItem("Hello", 100)
            .SetItem("World", 50);

        Assert.That(map, Has.Count.EqualTo(2));

        map = map.SetItem("Hello", 200);
        Assert.That(map, Has.Count.EqualTo(2));
        Assert.That(map["Hello"], Is.EqualTo(200));

        Assert.That(map, Is.SameAs(map.SetItem("Hello", 200)));
    }

    [Test]
    public void test_setItems()
    {
        var template = new Dictionary<string, int>
        {
            { "Hello", 100 },
            { "World", 50 },
        };
        var map = ImmutableCopyOnWriteDictionary<string, int>
            .Empty
            .SetItems(template);

        Assert.That(map, Has.Count.EqualTo(2));

        var changes = new Dictionary<string, int>
        {
            { "Hello", 150 },
            { "Dogs", 90 },
        };
        map = map.SetItems(changes);
        Assert.That(map, Has.Count.EqualTo(3));
        Assert.That(map["Hello"], Is.EqualTo(150));
        Assert.That(map["World"], Is.EqualTo(50));
        Assert.That(map["Dogs"], Is.EqualTo(90));

        map = map.SetItems(
            new[]
            {
                new KeyValuePair<string, int>("Hello", 80),
                new KeyValuePair<string, int>("Hello", 70),
            });
        Assert.That(map, Has.Count.EqualTo(3));
        Assert.That(map["Hello"], Is.EqualTo(70));
        Assert.That(map["World"], Is.EqualTo(50));
        Assert.That(map["Dogs"], Is.EqualTo(90));

        map = ImmutableCopyOnWriteDictionary<string, int>.Empty.SetItems(new[]
        {
            // use an array for code coverage
            new KeyValuePair<string, int>("a", 1), new KeyValuePair<string, int>("b", 2),
            new KeyValuePair<string, int>("a", 3),
        });
        Assert.That(map, Has.Count.EqualTo(2));
        Assert.That(map["a"], Is.EqualTo(3));
        Assert.That(map["b"], Is.EqualTo(2));
    }

    [Test]
    public void test_containsKey()
    {
        Assert.That(ImmutableCopyOnWriteDictionary<string, int>.Empty.ContainsKey("hello"), Is.False);
        Assert.That(ImmutableCopyOnWriteDictionary<string, int>.Empty.Add("hello", 1).ContainsKey("hello"), Is.True);
    }

    [Test]
    public void test_accessing_non_existing_key_throws()
    {
        Assert.Throws<KeyNotFoundException>(() => { _ = ImmutableCopyOnWriteDictionary<string, int>.Empty["hello"]; });
    }

    [Test]
    public void test_accessing_existing_key()
    {
        Assert.That(ImmutableCopyOnWriteDictionary<string, int>.Empty.Add("hello", 1)["hello"], Is.EqualTo(1));
    }

#if NET5_0_OR_GREATER
    [Test]
    public void test_tryGetKey()
    {
        IImmutableDictionary<string,int> uut = new ImmutableCopyOnWriteDictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            .Add("a", 1);

        string actualKey;
        Assert.That(uut.TryGetKey("a", out actualKey), Is.True);
        Assert.That(actualKey, Is.EqualTo("a"));

        Assert.That(uut.TryGetKey("A", out actualKey), Is.True);
        Assert.That(actualKey, Is.EqualTo("a"));

        Assert.That(uut.TryGetKey("b", out actualKey), Is.False);
        Assert.That(actualKey, Is.EqualTo("b"));
    }
#endif

    [Test]
    public void test_add_existing_key_with_same_value()
    {
        static void AddExistingKeySameValueTestHelper<TKey, TValue>(ImmutableCopyOnWriteDictionary<TKey, TValue> map, TKey key, TValue value1, TValue value2) where TKey : notnull
        {
            Assert.That(map, Is.Not.Null);
            Assert.That(key, Is.Not.Null);
            Assert.That(map.ValueComparer.Equals(value1, value2));

            var m = map.Add(key, value1);
            Assert.That(m, Is.SameAs(m.Add(key, value2)));
            Assert.That(m, Is.SameAs(m.AddRange(new[] { new KeyValuePair<TKey,TValue>(key, value2) })));
        }

        AddExistingKeySameValueTestHelper(new ImmutableCopyOnWriteDictionary<string, string>(StringComparer.Ordinal, StringComparer.Ordinal), "hello", "world", "world");
        AddExistingKeySameValueTestHelper(new ImmutableCopyOnWriteDictionary<string, string>(StringComparer.Ordinal, StringComparer.OrdinalIgnoreCase), "hello", "world", "WORLD");
    }

    [Test]
    public void test_add_existing_key_with_different_value()
    {
        var empty = new ImmutableCopyOnWriteDictionary<string, string>(StringComparer.Ordinal, StringComparer.Ordinal);
        var map1 = empty.Add("hello", "world");
        var map2 = empty.Add("hello", "WORLD");

        Assert.Throws<ArgumentException>(() => { _ = map1.Add("hello", "WORLD"); });
        Assert.Throws<ArgumentException>(() => { _ = map2.Add("hello", "world"); });
    }

    [Test]
    public void test_add_range_should_throw_on_null_key()
    {
        var items = new[] { new KeyValuePair<string, string>(default(string)!, "value") };

        var map = new ImmutableCopyOnWriteDictionary<string, string>(StringComparer.Ordinal, StringComparer.Ordinal);
        Assert.Throws<ArgumentNullException>(() => map.AddRange(items));

        map = new ImmutableCopyOnWriteDictionary<string, string>(new SameHashForEverythingComparer<string>(), new SameHashForEverythingComparer<string>());
        Assert.Throws<ArgumentNullException>(() => map.AddRange(items));
    }

    [Test]
    public void test_unordered_change()
    {
        var map = new ImmutableCopyOnWriteDictionary<string, string>(StringComparer.Ordinal, StringComparer.Ordinal)
            .Add("Johnny", "Appleseed")
            .Add("JOHNNY", "Appleseed");
        Assert.That(map, Has.Count.EqualTo(2));
        Assert.That(map.ContainsKey("Johnny"), Is.True);
        Assert.That(map.ContainsKey("johnny"), Is.False);

        var newMap = map.WithKeyComparer(StringComparer.OrdinalIgnoreCase);
        Assert.That(newMap, Has.Count.EqualTo(1));
        Assert.That(newMap.ContainsKey("Johnny"), Is.True);
        Assert.That(newMap.ContainsKey("johnny"), Is.True);
    }

    [Test]
    public void test_set_item_update_equal_key()
    {
        var map = ImmutableCopyOnWriteDictionary<string, int>.Empty
            .WithKeyComparer(StringComparer.OrdinalIgnoreCase)
            .SetItem("A", 1);

        map = map.SetItem("a", 2);

        // This is a different behaviour that ImmutableDictionary<,>; we are NOT updating the key here
        Assert.That(map.Keys.Single(), Is.EqualTo("A"));
        Assert.That(map["a"], Is.EqualTo(2));
    }


    [Test]
    public void test_set_items_throws_on_null_key()
    {
        var map = ImmutableCopyOnWriteDictionary<string, int>.Empty.WithKeyComparer(StringComparer.OrdinalIgnoreCase);
        KeyValuePair<string, int>[] items = new[] { new KeyValuePair<string, int>(null!, 0) };
        Assert.Throws<ArgumentNullException>(() => map.SetItems(items));
        map = map.WithKeyComparer(new SameHashForEverythingComparer<string>());
        Assert.Throws<ArgumentNullException>(() => map.SetItems(items));
    }

    [Test]
    public void test_set_item_update_equal_key_with_value_equality_by_comparer()
    {
        var map = ImmutableCopyOnWriteDictionary<string, CaseInsensitiveString>
            .Empty
            .WithComparers(StringComparer.OrdinalIgnoreCase, new MyStringOrdinalComparer());

        map = map.SetItem("key", new CaseInsensitiveString("Hello"));
        map = map.SetItem("key", new CaseInsensitiveString("hello"));
        Assert.That(map["key"].Value, Is.EqualTo("hello"));
        Assert.That(map.SetItem("key", new CaseInsensitiveString("hello")), Is.SameAs(map));
    }

    [Test]
    public void test_creation()
    {
        IEnumerable<KeyValuePair<string, string>> pairs = new Dictionary<string, string> { { "a", "b" } };
        StringComparer keyComparer = StringComparer.OrdinalIgnoreCase;
        StringComparer valueComparer = StringComparer.CurrentCulture;

        var dictionary = ImmutableCopyOnWriteDictionary.Create<string, string>();
        Assert.That(dictionary, Has.Count.EqualTo(0));
        Assert.That(dictionary.KeyComparer, Is.SameAs(EqualityComparer<string>.Default));
        Assert.That(dictionary.ValueComparer, Is.SameAs(EqualityComparer<string>.Default));

        dictionary = ImmutableCopyOnWriteDictionary.Create<string, string>(keyComparer);
        Assert.That(dictionary, Has.Count.EqualTo(0));
        Assert.That(dictionary.KeyComparer, Is.SameAs(keyComparer));
        Assert.That(dictionary.ValueComparer, Is.SameAs(EqualityComparer<string>.Default));

        dictionary = ImmutableCopyOnWriteDictionary.Create<string, string>(keyComparer, valueComparer);
        Assert.That(dictionary, Has.Count.EqualTo(0));
        Assert.That(dictionary.KeyComparer, Is.SameAs(keyComparer));
        Assert.That(dictionary.ValueComparer, Is.SameAs(valueComparer));

        dictionary = ImmutableCopyOnWriteDictionary.CreateRange(pairs);
        Assert.That(dictionary, Has.Count.EqualTo(1));
        Assert.That(dictionary.KeyComparer, Is.SameAs(EqualityComparer<string>.Default));
        Assert.That(dictionary.ValueComparer, Is.SameAs(EqualityComparer<string>.Default));

        dictionary = ImmutableCopyOnWriteDictionary.CreateRange(keyComparer, pairs);
        Assert.That(dictionary, Has.Count.EqualTo(1));
        Assert.That(dictionary.KeyComparer, Is.SameAs(keyComparer));
        Assert.That(dictionary.ValueComparer, Is.SameAs(EqualityComparer<string>.Default));

        dictionary = ImmutableCopyOnWriteDictionary.CreateRange(keyComparer, valueComparer, pairs);
        Assert.That(dictionary, Has.Count.EqualTo(1));
        Assert.That(dictionary.KeyComparer, Is.SameAs(keyComparer));
        Assert.That(dictionary.ValueComparer, Is.SameAs(valueComparer));
    }

    [Test]
    public void test_to_immutable_copy_on_write_dictionary_extension()
    {
        IEnumerable<KeyValuePair<string, string>> pairs = new Dictionary<string, string> { { "a", "B" } };
        StringComparer keyComparer = StringComparer.OrdinalIgnoreCase;
        StringComparer valueComparer = StringComparer.CurrentCulture;

        var dictionary = pairs.ToImmutableCopyOnWriteDictionary();
        Assert.That(dictionary, Has.Count.EqualTo(1));
        Assert.That(dictionary.KeyComparer, Is.SameAs(EqualityComparer<string>.Default));
        Assert.That(dictionary.ValueComparer, Is.SameAs(EqualityComparer<string>.Default));

        dictionary = pairs.ToImmutableCopyOnWriteDictionary(keyComparer);
        Assert.That(dictionary, Has.Count.EqualTo(1));
        Assert.That(dictionary.KeyComparer, Is.SameAs(keyComparer));
        Assert.That(dictionary.ValueComparer, Is.SameAs(EqualityComparer<string>.Default));

        dictionary = pairs.ToImmutableCopyOnWriteDictionary(keyComparer, valueComparer);
        Assert.That(dictionary, Has.Count.EqualTo(1));
        Assert.That(dictionary.KeyComparer, Is.SameAs(keyComparer));
        Assert.That(dictionary.ValueComparer, Is.SameAs(valueComparer));

        dictionary = pairs.ToImmutableCopyOnWriteDictionary(p => p.Key.ToUpperInvariant(), p => p.Value.ToLowerInvariant());
        Assert.That(dictionary, Has.Count.EqualTo(1));
        Assert.That(dictionary.Keys.Single(), Is.EqualTo("A"));
        Assert.That(dictionary.Values.Single(), Is.EqualTo("b"));
        Assert.That(dictionary.KeyComparer, Is.SameAs(EqualityComparer<string>.Default));
        Assert.That(dictionary.ValueComparer, Is.SameAs(EqualityComparer<string>.Default));

        dictionary = pairs.ToImmutableCopyOnWriteDictionary(p => p.Key.ToUpperInvariant(), p => p.Value.ToLowerInvariant(), keyComparer);
        Assert.That(dictionary, Has.Count.EqualTo(1));
        Assert.That(dictionary.Keys.Single(), Is.EqualTo("A"));
        Assert.That(dictionary.Values.Single(), Is.EqualTo("b"));
        Assert.That(dictionary.KeyComparer, Is.SameAs(keyComparer));
        Assert.That(dictionary.ValueComparer, Is.SameAs(EqualityComparer<string>.Default));

        dictionary = pairs.ToImmutableCopyOnWriteDictionary(p => p.Key.ToUpperInvariant(), p => p.Value.ToLowerInvariant(), keyComparer, valueComparer);
        Assert.That(dictionary, Has.Count.EqualTo(1));
        Assert.That(dictionary.Keys.Single(), Is.EqualTo("A"));
        Assert.That(dictionary.Values.Single(), Is.EqualTo("b"));
        Assert.That(dictionary.KeyComparer, Is.SameAs(keyComparer));
        Assert.That(dictionary.ValueComparer, Is.SameAs(valueComparer));

        var list = new int[] { 1, 2 };
        var intDictionary = list.ToImmutableCopyOnWriteDictionary(n => (double)n);
        Assert.That(intDictionary, Has.Count.EqualTo(2));
        Assert.That(intDictionary[1.0], Is.EqualTo(1));
        Assert.That(intDictionary[2.0], Is.EqualTo(2));

        var stringIntDictionary = list.ToImmutableCopyOnWriteDictionary(n => n.ToString(), StringComparer.OrdinalIgnoreCase);
        Assert.That(stringIntDictionary.KeyComparer, Is.SameAs(StringComparer.OrdinalIgnoreCase));
        Assert.That(stringIntDictionary, Has.Count.EqualTo(2));
        Assert.That(stringIntDictionary["1"], Is.EqualTo(1));
        Assert.That(stringIntDictionary["2"], Is.EqualTo(2));
    }

    [Test]
    public void test_to_immutable_copy_on_write_dictionary_optimized()
    {
        var dictionary = ImmutableCopyOnWriteDictionary.Create<string, string>();
        var result = dictionary.ToImmutableCopyOnWriteDictionary();
        Assert.That(result, Is.SameAs(dictionary));

        StringComparer cultureComparer = StringComparer.CurrentCulture;
        result = dictionary.WithComparers(cultureComparer, StringComparer.OrdinalIgnoreCase);
        Assert.That(cultureComparer, Is.SameAs(result.KeyComparer));
        Assert.That(StringComparer.OrdinalIgnoreCase, Is.SameAs(result.ValueComparer));
    }


    [Test]
    public void test_with_comparers()
    {
        var map = ImmutableCopyOnWriteDictionary.Create<string, string>().Add("a", "1").Add("B", "1");
        Assert.That(map.KeyComparer, Is.SameAs(EqualityComparer<string>.Default));
        Assert.That(map.ContainsKey("a"), Is.True);
        Assert.That(map.ContainsKey("Aa"), Is.False);

        map = map.WithComparers(StringComparer.OrdinalIgnoreCase);
        Assert.That(map.KeyComparer, Is.SameAs(StringComparer.OrdinalIgnoreCase));
        Assert.That(map, Has.Count.EqualTo(2));
        Assert.That(map.ContainsKey("a"), Is.True);
        Assert.That(map.ContainsKey("A"), Is.True);
        Assert.That(map.ContainsKey("b"), Is.True);

        var cultureComparer = StringComparer.CurrentCulture;
        map = map.WithComparers(StringComparer.OrdinalIgnoreCase, cultureComparer);
        Assert.That(map.KeyComparer, Is.SameAs(StringComparer.OrdinalIgnoreCase));
        Assert.That(map.ValueComparer, Is.SameAs(cultureComparer));
        Assert.That(map, Has.Count.EqualTo(2));
        Assert.That(map.ContainsKey("a"), Is.True);
        Assert.That(map.ContainsKey("A"), Is.True);
        Assert.That(map.ContainsKey("b"), Is.True);
    }
    
    [Test]
    public void test_with_comparers_collisions()
    {
        // First check where collisions have matching values.
        var map = ImmutableCopyOnWriteDictionary
            .Create<string, string>()
            .Add("a", "1")
            .Add("A", "1");
        
        map = map.WithComparers(StringComparer.OrdinalIgnoreCase);
        Assert.That(StringComparer.OrdinalIgnoreCase, Is.SameAs(map.KeyComparer));
        Assert.That(map, Has.Count.EqualTo(1));
        Assert.That(map.ContainsKey("a"), Is.True);
        Assert.That(map["a"], Is.EqualTo("1"));

        // Now check where collisions have conflicting values.
        map = ImmutableCopyOnWriteDictionary
            .Create<string, string>()
            .Add("a", "1")
            .Add("A", "2")
            .Add("b", "3");
        Assert.Throws<ArgumentException>(() => map.WithComparers(StringComparer.OrdinalIgnoreCase));

        // Force all values to be considered equal.
        map = map.WithComparers(StringComparer.OrdinalIgnoreCase, EverythingEqual<string>.Default);
        Assert.That(StringComparer.OrdinalIgnoreCase, Is.SameAs(map.KeyComparer));
        Assert.That(EverythingEqual<string>.Default, Is.SameAs(map.ValueComparer));
        Assert.That(map, Has.Count.EqualTo(2));
        Assert.That(map.ContainsKey("a"), Is.True);
        Assert.That(map.ContainsKey("b"), Is.True);
    }
    
#if NET5_0_OR_GREATER
    [Test]
    public void test_collision_exception_message_contains_key()
    {
        var map = ImmutableCopyOnWriteDictionary
            .Create<string, string>()
            .Add("firstKey", "1")
            .Add("secondKey", "2");
        
        var exception = Assert.Throws<ArgumentException>(() => { _ = map.Add("firstKey", "3"); });
        Assert.That(exception!.Message, Does.Contain("firstKey"));
    }
#endif
    
    [Test]
    public void test_with_comparers_empty_collection()
    {
        var map = ImmutableCopyOnWriteDictionary.Create<string, string>();
        Assert.That(EqualityComparer<string>.Default, Is.SameAs(map.KeyComparer));
        map = map.WithComparers(StringComparer.OrdinalIgnoreCase);
        Assert.That(StringComparer.OrdinalIgnoreCase, Is.SameAs(map.KeyComparer));
    }

#if NET5_0_OR_GREATER
    [Test]
    public void test_get_value_or_default_of_IImmutableDictionary()
    {
        IImmutableDictionary<string, int> empty = ImmutableCopyOnWriteDictionary.Create<string, int>();
        IImmutableDictionary<string, int> populated = ImmutableCopyOnWriteDictionary.Create<string, int>().Add("a", 5);
        Assert.That(empty.GetValueOrDefault("a"), Is.EqualTo(0));
        Assert.That(empty.GetValueOrDefault("a", 1), Is.EqualTo(1));
        Assert.That(populated.GetValueOrDefault("a"), Is.EqualTo(5));
        Assert.That(populated.GetValueOrDefault("a", 1), Is.EqualTo(5));
    }
    
    [Test]
    public void test_get_value_or_defaultO_of_concrete_type()
    {
        var empty = ImmutableCopyOnWriteDictionary.Create<string, int>();
        var populated = ImmutableCopyOnWriteDictionary.Create<string, int>().Add("a", 5);
        Assert.That(empty.GetValueOrDefault("a"), Is.EqualTo(0));
        Assert.That(empty.GetValueOrDefault("a", 1), Is.EqualTo(1));
        Assert.That(populated.GetValueOrDefault("a"), Is.EqualTo(5));
        Assert.That(populated.GetValueOrDefault("a", 1), Is.EqualTo(5));
    }
#endif
    
    [Test]
    public void test_enumerator()
    {
        var collection = ImmutableCopyOnWriteDictionary.Create<int, int>().Add(5, 3);
        var enumerator = collection.GetEnumerator();
        Assert.That(enumerator.MoveNext(), Is.True);
        Assert.That(enumerator.MoveNext(), Is.False);
        enumerator.Dispose();
    }
    
    [Test]
    public void test_clear_no_comparer_returns_empty_without_comparer()
    {
        var dictionary = new Dictionary<string, int>
        {
            { "a", 1 }
        }.ToImmutableCopyOnWriteDictionary();
        Assert.That(dictionary.Clear(), Is.SameAs(ImmutableCopyOnWriteDictionary<string, int>.Empty));
        Assert.That(dictionary, Is.Not.Empty);
    }
    
    [Test]
    public void test_clear_has_comparer_returns_empty_with_original_comparer()
    {
        var dictionary = new Dictionary<string, int>
        {
            { "a", 1 }
        }.ToImmutableCopyOnWriteDictionary(StringComparer.OrdinalIgnoreCase);

        var clearedDictionary = dictionary.Clear();
        Assert.That(clearedDictionary.Clear(), Is.Not.SameAs(ImmutableCopyOnWriteDictionary<string, int>.Empty));
        Assert.That(dictionary, Is.Not.Empty);

        clearedDictionary = clearedDictionary.Add("a", 1);
        Assert.That(clearedDictionary.ContainsKey("A"), Is.True);
    }
    
#if NET5_0_OR_GREATER
    [Test]
    public void text_indexer_key_not_found_exception_contains_key_in_message()
    {
        var map = ImmutableCopyOnWriteDictionary.Create<string, string>()
            .Add("a", "1").Add("b", "2");
        var exception = Assert.Throws<KeyNotFoundException>(() => { _ = map["c"];});
        Assert.That(exception!.Message, Does.Contain("'c'"));
    }
#endif
}