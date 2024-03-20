namespace FM.Collections.Test;

public class ImmutableCopyOnWriteDictionaryBuilderTests
{
    [Test]
    public void test_add()
    {
        var builder = ImmutableCopyOnWriteDictionary.CreateBuilder<string, int>();
        builder.Add("five", 5);
        builder.Add(new KeyValuePair<string, int>("six", 6));

        Assert.That(builder["five"], Is.EqualTo(5));
        Assert.That(builder["six"], Is.EqualTo(6));
        Assert.That(builder.ContainsKey("four"), Is.False);
    }

    [Test]
    public void test_add_exact_duplicate()
    {
        var builder = ImmutableCopyOnWriteDictionary.CreateBuilder<string, int>();
        builder.Add("five", 5);
        builder.Add("five", 5);
        Assert.That(builder, Has.Count.EqualTo(1));
    }

    [Test]
    public void test_add_existing_Key_with_different_value()
    {
        IDictionary<string, int> builder = ImmutableCopyOnWriteDictionary.CreateBuilder<string, int>();
        builder.Add("five", 5);
        Assert.Throws<ArgumentException>(() => builder.Add("five", 6));
    }

    [Test]
    public void test_indexer()
    {
        var builder = ImmutableCopyOnWriteDictionary.CreateBuilder<string, int>();

        // Set and set again.
        builder["five"] = 5;
        Assert.That(builder["five"], Is.EqualTo(5));
        builder["five"] = 5;
        Assert.That(builder["five"], Is.EqualTo(5));

        // Set to a new value.
        builder["five"] = 50;
        Assert.That(builder["five"], Is.EqualTo(50));

        // Retrieve an invalid value.
        Assert.Throws<KeyNotFoundException>(() => { _ = builder["foo"]; });
    }

    [Test]
    public void test_contains_pair()
    {
        var map = ImmutableCopyOnWriteDictionary<string, int>.Empty.Add("five", 5);
        var builder = map.ToBuilder();
        Assert.That(builder.Contains(new KeyValuePair<string, int>("five", 5)), Is.True);
    }

    [Test]
    public void test_remove_pair()
    {
        var map = ImmutableCopyOnWriteDictionary<string, int>.Empty.Add("five", 5).Add("six", 6);
        var builder = map.ToBuilder();
        Assert.That(builder.Remove(new KeyValuePair<string, int>("five", 5)), Is.True);
        Assert.That(builder.Remove(new KeyValuePair<string, int>("foo", 1)), Is.False);
        Assert.That(builder, Has.Count.EqualTo(1));
        Assert.That(builder["six"], Is.EqualTo(6));
    }

    [Test]
    public void test_remove_key()
    {
        var map = ImmutableCopyOnWriteDictionary<string, int>.Empty.Add("five", 5).Add("six", 6);
        var builder = map.ToBuilder();
        builder.Remove("five");
        Assert.That(builder, Has.Count.EqualTo(1));
        Assert.That(builder["six"], Is.EqualTo(6));
    }

    [Test]
    public void test_copy_to()
    {
        var map = ImmutableCopyOnWriteDictionary<string, int>.Empty.Add("five", 5);
        var builder = map.ToBuilder();
        var array = new KeyValuePair<string, int>[2]; // intentionally larger than source.
        builder.CopyTo(array, 1);
        Assert.That(array[0], Is.EqualTo(new KeyValuePair<string, int>()));
        Assert.That(array[1], Is.EqualTo(new KeyValuePair<string, int>("five", 5)));
        Assert.Throws<ArgumentNullException>(() => builder.CopyTo(null, 0));
    }


    [Test]
    public void test_is_not_read_only()
    {
        var builder = ImmutableCopyOnWriteDictionary.CreateBuilder<string, int>();
        Assert.That(builder.IsReadOnly, Is.False);
    }

    [Test]
    public void test_keys()
    {
        var map = ImmutableCopyOnWriteDictionary<string, int>.Empty.Add("five", 5).Add("six", 6);
        var builder = map.ToBuilder();
        Assert.That(builder.Keys, Is.EquivalentTo(new[] { "five", "six" }));
        Assert.That(((IReadOnlyDictionary<string, int>)builder).Keys.ToArray(), Is.EquivalentTo(new[] { "five", "six" }));
    }

    [Test]
    public void test_values()
    {
        var map = ImmutableCopyOnWriteDictionary<string, int>.Empty.Add("five", 5).Add("six", 6);
        var builder = map.ToBuilder();
        Assert.That(builder.Values, Is.EquivalentTo(new[] { 5, 6 }));
        Assert.That(((IReadOnlyDictionary<string, int>)builder).Values.ToArray(), Is.EquivalentTo(new[] { 5, 6 }));
    }

    [Test]
    public void test_try_get_value()
    {
        var map = ImmutableCopyOnWriteDictionary<string, int>.Empty.Add("five", 5).Add("six", 6);
        var builder = map.ToBuilder();
        int value;
        Assert.That(builder.TryGetValue("five", out value) && value == 5, Is.True);
        Assert.That(builder.TryGetValue("six", out value) && value == 6, Is.True);
        Assert.That(builder.TryGetValue("four", out value), Is.False);
        Assert.That(value, Is.EqualTo(0));
    }

    [Test]
    public void test_enumerate()
    {
        var map = ImmutableCopyOnWriteDictionary<string, int>.Empty.Add("five", 5).Add("six", 6);
        var builder = map.ToBuilder();

        using var enumerator = builder.GetEnumerator();
        Assert.That(enumerator.MoveNext(), Is.True);
        Assert.That(enumerator.MoveNext(), Is.True);
        Assert.That(enumerator.MoveNext(), Is.False);
    }

    [Test]
    public void test_create_builder()
    {
        var builder = ImmutableCopyOnWriteDictionary.CreateBuilder<string, string>();
        Assert.That(builder.KeyComparer, Is.SameAs(EqualityComparer<string>.Default));
        Assert.That(builder.ValueComparer, Is.SameAs(EqualityComparer<string>.Default));

        builder = ImmutableCopyOnWriteDictionary.CreateBuilder<string, string>(StringComparer.Ordinal);
        Assert.That(builder.KeyComparer, Is.SameAs(StringComparer.Ordinal));
        Assert.That(builder.ValueComparer, Is.SameAs(EqualityComparer<string>.Default));

        builder = ImmutableCopyOnWriteDictionary.CreateBuilder<string, string>(StringComparer.Ordinal, StringComparer.OrdinalIgnoreCase);
        Assert.That(builder.KeyComparer, Is.SameAs(StringComparer.Ordinal));
        Assert.That(builder.ValueComparer, Is.SameAs(StringComparer.OrdinalIgnoreCase));
    }

    [Test]
    public void test_to_builder()
    {
        var builder = ImmutableCopyOnWriteDictionary<int, string>.Empty.ToBuilder();
        builder.Add(3, "3");
        builder.Add(5, "5");
        Assert.That(builder, Has.Count.EqualTo(2));
        Assert.That(builder.ContainsKey(3), Is.True);
        Assert.That(builder.ContainsKey(5), Is.True);
        Assert.That(builder.ContainsKey(7), Is.False);

        var set = builder.ToImmutable();
        Assert.That(builder.Count, Is.EqualTo(set.Count));

        builder.Add(8, "8");
        Assert.That(builder, Has.Count.EqualTo(3));
        Assert.That(set, Has.Count.EqualTo(2));
        Assert.That(builder.ContainsKey(8), Is.True);
        Assert.That(set.ContainsKey(8), Is.False);

        var set2 = builder.ToImmutable();
        Assert.That(set2, Has.Count.EqualTo(3));
        Assert.That(set2.ContainsKey(8), Is.True);
    }

    [Test]
    public void test_builder_add_range_throws_when_adding_null_key()
    {
        var set = ImmutableCopyOnWriteDictionary<string, int>.Empty.Add("1", 1);
        var builder = set.ToBuilder();
        var items = new[] { new KeyValuePair<string, int>(null!, 0) };
        Assert.Throws<ArgumentNullException>(() => builder.AddRange(items));
    }

    [Test]
    public void test_builder_from_map()
    {
        var set = ImmutableCopyOnWriteDictionary<int, string>.Empty.Add(1, "1");
        var builder = set.ToBuilder();
        Assert.That(builder.ContainsKey(1), Is.True);
        builder.Add(3, "3");
        builder.Add(5, "5");
        Assert.That(builder, Has.Count.EqualTo(3));
        Assert.That(builder.ContainsKey(3), Is.True);
        Assert.That(builder.ContainsKey(5), Is.True);
        Assert.That(builder.ContainsKey(7), Is.False);

        var set2 = builder.ToImmutable();
        Assert.That(builder, Has.Count.EqualTo(set2.Count));
        Assert.That(set2.ContainsKey(1), Is.True);
        builder.Add(8, "8");
        Assert.That(builder, Has.Count.EqualTo(4));
        Assert.That(set2, Has.Count.EqualTo(3));
        Assert.That(builder.ContainsKey(8), Is.True);

        Assert.That(set.ContainsKey(8), Is.False);
        Assert.That(set2.ContainsKey(8), Is.False);
    }

    [Test]
    public void test_several_changes()
    {
        var mutable = ImmutableCopyOnWriteDictionary<int, string>.Empty.ToBuilder();
        var immutable1 = mutable.ToImmutable();
        Assert.That(immutable1, Is.SameAs(mutable.ToImmutable()));

        mutable.Add(1, "a");
        var immutable2 = mutable.ToImmutable();
        Assert.That(immutable1, Is.Not.SameAs(immutable2)); // "Mutating the collection did not reset the Immutable property."
        Assert.That(immutable2, Is.SameAs(mutable.ToImmutable())); // "The Immutable property getter is creating new objects without any differences."
        Assert.That(immutable2, Has.Count.EqualTo(1));
    }

    [Test]
    public void test_enumerate_builder_while_mutating()
    {
        var builder = ImmutableCopyOnWriteDictionary<int, string>.Empty
            .AddRange(Enumerable.Range(1, 10).Select(n => new KeyValuePair<int, string>(n, string.Empty)))
            .ToBuilder();

        Assert.That(
            Enumerable.Range(1, 10).Select(n => new KeyValuePair<int, string>(n, string.Empty)),
            Is.EquivalentTo(builder));

        var enumerator = builder.GetEnumerator();
        Assert.That(enumerator.MoveNext());
        builder.Add(11, string.Empty);

        // Verify that a new enumerator will succeed.
        Assert.That(
            Enumerable.Range(1, 11).Select(n => new KeyValuePair<int, string>(n, string.Empty)),
            Is.EquivalentTo(builder));

        // Try enumerating further with the previous enumerable now that we've changed the collection.
        Assert.Throws<InvalidOperationException>(() => enumerator.MoveNext());
        enumerator.Reset();
        enumerator.MoveNext(); // resetting should fix the problem.

        // Verify that by obtaining a new enumerator, we can enumerate all the contents.
        Assert.That(
            Enumerable.Range(1, 11).Select(n => new KeyValuePair<int, string>(n, string.Empty)),
            Is.EquivalentTo(builder));
    }

    [Test]
    public void test_builder_reuses_unchanged_immutable_instances()
    {
        var collection = ImmutableCopyOnWriteDictionary<int, string>.Empty.Add(1, string.Empty);
        var builder = collection.ToBuilder();
        Assert.That(builder.ToImmutable(), Is.SameAs(collection));
        builder.Add(2, string.Empty);

        var newImmutable = builder.ToImmutable();
        Assert.That(newImmutable, Is.Not.SameAs(collection));
        Assert.That(builder.ToImmutable(), Is.SameAs(newImmutable));
    }

    [Test]
    public void test_add_range()
    {
        var builder = ImmutableCopyOnWriteDictionary.Create<string, int>().ToBuilder();
        builder.AddRange(new Dictionary<string, int> { { "a", 1 }, { "b", 2 } });
        Assert.That(builder, Has.Count.EqualTo(2));
        Assert.That(builder["a"], Is.EqualTo(1));
        Assert.That(builder["b"], Is.EqualTo(2));
    }

    [Test]
    public void test_remove_range()
    {
        var builder = ImmutableCopyOnWriteDictionary.Create<string, int>()
            .AddRange(new Dictionary<string, int> { { "a", 1 }, { "b", 2 }, { "c", 3 } })
            .ToBuilder();
        Assert.That(builder, Has.Count.EqualTo(3));
        builder.RemoveRange(new[] { "a", "b" });
        Assert.That(builder, Has.Count.EqualTo(1));
        Assert.That(builder["c"], Is.EqualTo(3));
    }
    
    [Test]
    public void test_clear()
    {
        var builder = ImmutableCopyOnWriteDictionary.Create<string, int>().ToBuilder();
        builder.Add("five", 5);
        Assert.That(builder, Has.Count.EqualTo(1));
        builder.Clear();
        Assert.That(builder, Has.Count.EqualTo(0));
    }
    
    [Test]
    public void test_key_comparer()
    {
        var builder = ImmutableCopyOnWriteDictionary
            .Create<string, string>()
            .Add("a", "1")
            .Add("B", "1")
            .ToBuilder();

        Assert.That(builder.KeyComparer, Is.SameAs(EqualityComparer<string>.Default));
        Assert.That(builder.ContainsKey("a"), Is.True);
        Assert.That(builder.ContainsKey("A"), Is.False);
    }

#if NET5_0_OR_GREATER
    [Test]
    public void test_get_value_or_default_of_concrete_type()
    {
        var empty = ImmutableCopyOnWriteDictionary.Create<string, int>().ToBuilder();
        var populated = ImmutableCopyOnWriteDictionary.Create<string, int>().Add("a", 5).ToBuilder();
        Assert.That(empty.GetValueOrDefault("a"), Is.EqualTo(0));
        Assert.That(empty.GetValueOrDefault("a", 1), Is.EqualTo(1));
        Assert.That(populated.GetValueOrDefault("a"), Is.EqualTo(5));
        Assert.That(populated.GetValueOrDefault("a", 1), Is.EqualTo(5));
    }
#endif
}