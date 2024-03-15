using FM.Collections.Comparers;

namespace FM.Collections.Test;

public class MinMaxHeapArityTwoTests : MinMaxHeapTests<Arity.Two>
{
    
}

public class MinMaxHeapArityThreeTests : MinMaxHeapTests<Arity.Three>
{
    
}

public class MinMaxHeapArityFourTests : MinMaxHeapTests<Arity.Four>
{
    
}

public abstract class MinMaxHeapTests<TArity> where TArity : struct, IConstInt
{
    [Test, TestCaseSource(nameof(NonEmptyHeaps))]
    public void can_create_heaps(MinMaxHeap<TArity, ComparableComparer<double>, double> value)
    {
        Assert.That(FM.Collections.Algorithms.MinMaxHeap.IsMinMaxHeap(value.RawValues, value.Comparer, default(TArity)), Is.True);
    }
    
    [Test, TestCaseSource(nameof(NonEmptyHeaps))]
    public void can_modify_item_from_middle_to_min(MinMaxHeap<TArity, ComparableComparer<double>, double> value)
    {
        var bottom = value.Min - 1.0;
        value[value.Count/2] = bottom;
        Assert.That(value.Min, Is.EqualTo(bottom));
        Assert.That(FM.Collections.Algorithms.MinMaxHeap.IsMinMaxHeap(value.RawValues, value.Comparer, default(TArity)), Is.True);
    }
    
    [Test, TestCaseSource(nameof(NonEmptyHeaps))]
    public void can_modify_item_from_min_to_middle(MinMaxHeap<TArity, ComparableComparer<double>, double> value)
    {
        var avg = (value.Min + value.Max) / 2.0;
        value[0] = avg;
        Assert.That(value.Min, Is.EqualTo(value.Min()));
        Assert.That(FM.Collections.Algorithms.MinMaxHeap.IsMinMaxHeap(value.RawValues, value.Comparer, default(TArity)), Is.True);
    }
        
    [Test, TestCaseSource(nameof(NonEmptyHeaps))]
    public void can_modify_item_from_middle_to_max(MinMaxHeap<TArity, ComparableComparer<double>, double> value)
    {
        var top = value.Max + 1.0;
        value[value.Count/2] = top;
        Assert.That(value.Max, Is.EqualTo(value.Max()));
        Assert.That(value.Max, Is.EqualTo(top));
        Assert.That(FM.Collections.Algorithms.MinMaxHeap.IsMinMaxHeap(value.RawValues, value.Comparer, default(TArity)), Is.True);
    }
    
    [Test, TestCaseSource(nameof(NonEmptyHeaps))]
    public void can_modify_item_from_max_to_middle(MinMaxHeap<TArity, ComparableComparer<double>, double> value)
    {
        Assert.That(FM.Collections.Algorithms.MinMaxHeap.IsMinMaxHeap(value.RawValues, value.Comparer, default(TArity)), Is.True);
        var avg = (value.Min + value.Max) / 2.0;
        value[value.MaxIndex] = avg - 0.001;
        Assert.That(value.Max, Is.EqualTo(value.Max()));
        Assert.That(FM.Collections.Algorithms.MinMaxHeap.IsMinMaxHeap(value.RawValues, value.Comparer, default(TArity)), Is.True);
    }
    
    [Test, TestCaseSource(nameof(NonEmptyHeaps))]
    public void can_remove_max_item(MinMaxHeap<TArity, ComparableComparer<double>, double> value)
    {
        var items = value.ToList();
        var max = value.Max;
        var removed = value.RemoveMax();
        items.Remove(max);
        Assert.That(max, Is.EqualTo(removed));
        Assert.That(value, Is.EquivalentTo(items));
        Assert.That(FM.Collections.Algorithms.MinMaxHeap.IsMinMaxHeap(value.RawValues, value.Comparer, default(TArity)), Is.True);
    }
    
    [Test, TestCaseSource(nameof(NonEmptyHeaps))]
    public void can_remove_min_item(MinMaxHeap<TArity, ComparableComparer<double>, double> value)
    {
        var items = value.ToList();
        var min = value.Min;
        var removed = value.RemoveMin();
        items.Remove(min);
        Assert.That(min, Is.EqualTo(removed));
        Assert.That(value, Is.EquivalentTo(items));
        Assert.That(FM.Collections.Algorithms.MinMaxHeap.IsMinMaxHeap(value.RawValues, value.Comparer, default(TArity)), Is.True);
    }
    
       
    [Test, TestCaseSource(nameof(NonEmptyHeaps))]
    public void can_remove_max_items_until_empty(MinMaxHeap<TArity, ComparableComparer<double>, double> value)
    {
        while (value.Count > 0)
        {
            var max = value.Max;
            var removed = value.RemoveMax();
            Assert.That(max, Is.EqualTo(removed));
            Assert.That(FM.Collections.Algorithms.MinMaxHeap.IsMinMaxHeap(value.RawValues, value.Comparer, default(TArity)), Is.True);
        }
        Assert.That(value, Is.Empty);
    }
    
    [Test, TestCaseSource(nameof(NonEmptyHeaps))]
    public void can_remove_middle_items_until_empty(MinMaxHeap<TArity, ComparableComparer<double>, double> value)
    {
        while (value.Count > 0)
        {
            value.Remove(value.Count / 2);
            Assert.That(FM.Collections.Algorithms.MinMaxHeap.IsMinMaxHeap(value.RawValues, value.Comparer, default(TArity)), Is.True);
        }

        Assert.That(value, Is.Empty);
    }
    
    [Test, TestCaseSource(nameof(NonEmptyHeaps))]
    public void can_remove_min_items_until_empty(MinMaxHeap<TArity, ComparableComparer<double>, double> value)
    {
        while (value.Count > 0)
        {
            var min = value.Min;
            var removed = value.RemoveMin();
            Assert.That(min, Is.EqualTo(removed));
            Assert.That(FM.Collections.Algorithms.MinMaxHeap.IsMinMaxHeap(value.RawValues, value.Comparer, default(TArity)), Is.True);
        }
        Assert.That(value, Is.Empty);
    }
    
    [Test]
    public void can_modify_item_from_middle_max_layer_to_min()
    {
        var sut = CreateHeap(10_000);

        var i = (int)Math.Ceiling(Math.Pow(new TArity().Value, 3));
        Assert.That(FM.Collections.Algorithms.MinMaxHeap.IsMinLevel<TArity>(i), Is.False);

        var minValue = sut.Min - 1.0;
        sut[i] = minValue;

        Assert.That(sut.Min, Is.EqualTo(minValue));
        Assert.That(FM.Collections.Algorithms.MinMaxHeap.IsMinMaxHeap(sut.RawValues, sut.Comparer, default(TArity)), Is.True);
    }
    
    [Test]
    public void can_modify_item_from_middle_min_layer_to_max()
    {
        var sut = CreateHeap(10_000);
        
        var i = (int)Math.Ceiling(Math.Pow(new TArity().Value, 4));
        Assert.That(FM.Collections.Algorithms.MinMaxHeap.IsMinLevel<TArity>(i), Is.True);

        var maxValue = sut.Max + 1.0;
        sut[i] = maxValue;

        Assert.That(sut.Max, Is.EqualTo(maxValue));
        Assert.That(FM.Collections.Algorithms.MinMaxHeap.IsMinMaxHeap(sut.RawValues, sut.Comparer, default(TArity)), Is.True);
    }

    [TestCase(0, 0, 50_000)]
    [TestCase(1, 0, 50_000)]
    [TestCase(2, 0, 50_000)]
    [TestCase(0, 1, 50_000)]
    [TestCase(0, 2, 50_000)]
    [TestCase(0, 3, 50_000)]
    [TestCase(0, 10, 50_000)]
    [TestCase(0, 15, 50_000)]
    [TestCase(0, 10_000, 50_000)]
    [Ignore("time consuming")]
    public void random_mutations_leave_heap_in_correct_state(int seed, int initialHeapSize, int mutations)
    {
        var random = new Random(seed);
        var sut = new MinMaxHeap<TArity, ComparableComparer<double>, double>(default, Enumerable.Range(0, initialHeapSize).Select(_ => random.NextDouble()).ToList());
        
        Assert.That(FM.Collections.Algorithms.MinMaxHeap.IsMinMaxHeap(sut.RawValues, sut.Comparer, default(TArity)), Is.True);
        
        for (var i = 0; i < 50_000; i++)
        {
            var op = random.NextDouble();

            if (sut.Count == 0 || op < 0.1)
            {
                // Add a new random element
                var value = random.NextDouble();
                sut.Add(value);
            }
            else if (op < 0.20)
            {
                // Mutate an element
                var index = random.Next(0, sut.Count);
                var value = random.NextDouble();
                sut[index] = value;
            } else if (op < 0.30)
            {
                // Set random value to the existing min
                var index = random.Next(0, sut.Count);
                sut[index] = sut.Min;
            } else if (op < 0.40)
            {
                // Set random value to the existing max
                var index = random.Next(0, sut.Count);
                sut[index] = sut.Max;
            } else if (op < 0.50)
            {
                // Replace some random element to make it the new min
                var index = random.Next(0, sut.Count);
                sut[index] = sut.Min - 0.1;
            } else if (op < 0.60)
            {
                // Replace some random element to make it the new max
                var index = random.Next(0, sut.Count);
                sut[index] = sut.Max + 0.1;
            }  else if (op < 0.70)
            {
                // Delete some random element
                var index = random.Next(0, sut.Count);
                sut.Remove(index);
            } else if (op < 0.80)
            {
                // Add a new element that is the same as the current max
                sut.Add(sut.Max);
            } else if (op < 0.90)
            {
                // Add a new element that is the same as the current min
                sut.Add(sut.Min);
            } else if (op < 0.95)
            {
                // Add a new element that will become the new minimum
                sut.Add(sut.Min - 0.1);
            }
            else
            {
                // Add a new element that will become the new maximum
                sut.Add(sut.Max + 0.1);
            }
            
            Assert.That(FM.Collections.Algorithms.MinMaxHeap.IsMinMaxHeap(sut.RawValues, sut.Comparer, default(TArity)), Is.True);
            
            if (sut.Count > 0)
            {
                Assert.That(sut.Min, Is.EqualTo(sut.Min()));
                Assert.That(sut.Max, Is.EqualTo(sut.Max()));
            }
        }
    }
    
    private static IEnumerable<TestCaseData> NonEmptyHeaps()
    {
        yield return new TestCaseData(CreateHeap(1)) { TestName = "Single element heap" };
        yield return new TestCaseData(CreateHeap(2)) { TestName = "Two element heap" };
        yield return new TestCaseData(CreateHeap(3)) { TestName = "Three element heap"};
        yield return new TestCaseData(CreateHeap(6)) { TestName = "Six element heap"};
        yield return new TestCaseData(CreateHeap(7)) { TestName = "Seven element heap"};
        yield return new TestCaseData(CreateHeap(10_000)) { TestName = "Large heap" };
    }

    private static MinMaxHeap<TArity, ComparableComparer<double>, double> CreateHeap(int n = 10_000) => new(default, Enumerable.Range(0, n).Select(x => (double)x).ToList());
}