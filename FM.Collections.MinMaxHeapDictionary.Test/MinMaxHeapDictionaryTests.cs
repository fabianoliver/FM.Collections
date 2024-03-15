using FM.Collections.Comparers;

namespace FM.Collections.Test;

public class TwoAryMinMaxHeapDictionaryTests : MinMaxHeapDictionaryTests<Arity.Two>
{
    
}

public class FourAryMinMaxHeapDictionaryTests : MinMaxHeapDictionaryTests<Arity.Four>
{
    
}

public abstract class MinMaxHeapDictionaryTests<TArity>
    where TArity : struct, IConstInt
{
    [Test, TestCaseSource(nameof(NonEmptyDictionaries))]
    public void can_modify_item_from_middle_to_min(MinMaxHeapDictionary<TArity, KeyValuePairByComparableValueComparer<int, double>, int, double> value)
    {
        var mid= value.ElementAt(value.Count/2);
        
        var bottom = value.Min.Value - 1.0;
        value[mid.Key] = bottom;
        
        Assert.That(value.Min, Is.EqualTo(new KeyValuePair<int,double>(mid.Key, bottom)));
        Assert.That(Algorithms.MinMaxHeap.IsMinMaxHeap(value.Values.ToList(), Comparer<double>.Default, default(TArity)));
    }
    
    [Test, TestCaseSource(nameof(NonEmptyDictionaries))]
    public void can_modify_item_from_min_to_middle(MinMaxHeapDictionary<TArity, KeyValuePairByComparableValueComparer<int, double>, int, double> value)
    {
        var avg = (value.Min.Value + value.Max.Value) / 2.0;
        value[value.Min.Key] = avg + 0.001;
        Assert.That(value.Min, Is.EqualTo(value.OrderBy(x => x.Value).First()));
        Assert.That(Algorithms.MinMaxHeap.IsMinMaxHeap(value.Values.ToList(), Comparer<double>.Default, default(TArity)));
    }
        
    [Test, TestCaseSource(nameof(NonEmptyDictionaries))]
    public void can_modify_item_from_middle_to_max(MinMaxHeapDictionary<TArity, KeyValuePairByComparableValueComparer<int, double>, int, double> value)
    {
        var mid = value.ElementAt(value.Count / 2);
        
        var bottom = value.Max.Value + 1.0;
        value[mid.Key] = bottom;
        
        Assert.That(value.Max, Is.EqualTo(new KeyValuePair<int,double>(mid.Key, bottom)));
        Assert.That(Algorithms.MinMaxHeap.IsMinMaxHeap(value.Values.ToList(), Comparer<double>.Default, default(TArity)));
    }
    
    [Test, TestCaseSource(nameof(NonEmptyDictionaries))]
    public void can_modify_item_from_max_to_middle(MinMaxHeapDictionary<TArity, KeyValuePairByComparableValueComparer<int, double>, int, double> value)
    {
        var avg = (value.Min.Value + value.Max.Value) / 2.0;
        value[value.Max.Key] = avg - 0.001;
        Assert.That(value.Max, Is.EqualTo(value.OrderByDescending(x => x.Value).First()));
    }
    
    [Test, TestCaseSource(nameof(NonEmptyDictionaries))]
    public void can_remove_max_item(MinMaxHeapDictionary<TArity, KeyValuePairByComparableValueComparer<int, double>, int, double> value)
    {
        var max = value.Max;
        Assert.That(value.Remove(max), Is.True);
        Assert.That(value, Does.Not.ContainKey(max.Key));
        
        if(value.Any())
            Assert.That(value.Max.Key, Is.Not.EqualTo(max.Key));    
        
        Assert.That(Algorithms.MinMaxHeap.IsMinMaxHeap(value.RawValues, value.Comparer, default(TArity)), Is.True);
    }
    
    [Test, TestCaseSource(nameof(NonEmptyDictionaries))]
    public void can_remove_min_item(MinMaxHeapDictionary<TArity, KeyValuePairByComparableValueComparer<int, double>, int, double> value)
    {
        var min = value.Min;
        Assert.That(value.Remove(min), Is.True);
        Assert.That(value, Does.Not.ContainKey(min.Key));
        
        if(value.Any())
            Assert.That(value.Min.Key, Is.Not.EqualTo(min.Key));  
        Assert.That(Algorithms.MinMaxHeap.IsMinMaxHeap(value.Values.ToList(), Comparer<double>.Default, default(TArity)));
    }
    
       
    [Test, TestCaseSource(nameof(NonEmptyDictionaries))]
    public void can_remove_max_items_until_empty(MinMaxHeapDictionary<TArity, KeyValuePairByComparableValueComparer<int, double>, int, double> value)
    {
        while (value.Count > 0)
        {
            var max = value.Max.Key;
            Assert.That(value.Remove(max), Is.True);
            Assert.That(value, Does.Not.ContainKey(max));
            Assert.That(Algorithms.MinMaxHeap.IsMinMaxHeap(value.RawValues, value.Comparer, default(TArity)), Is.True);
        }
        Assert.That(value, Is.Empty);
    }
    
    [Test, TestCaseSource(nameof(NonEmptyDictionaries))]
    public void can_remove_min_items_until_empty(MinMaxHeapDictionary<TArity, KeyValuePairByComparableValueComparer<int, double>, int, double> value)
    {
        var check = value.ToDictionary(x => x.Key, x => x.Value);

        while (value.Count > 0)
        {
            var min = value.Min.Key;
            Assert.That(value.Remove(min), Is.True);
            check.Remove(min);
            Assert.That(value.ToDictionary(x => x.Key, x => x.Value), Is.EqualTo(check));
            Assert.That(value, Does.Not.ContainKey(min));
            Assert.That(Algorithms.MinMaxHeap.IsMinMaxHeap(value.RawValues, value.Comparer, default(TArity)), Is.True);
        }
        Assert.That(value, Is.Empty);
    }
    
    [Test, TestCaseSource(nameof(NonEmptyDictionaries))]
    public void can_remove_middle_items_until_empty(MinMaxHeapDictionary<TArity, KeyValuePairByComparableValueComparer<int, double>, int, double> value)
    {
        while (value.Count > 0)
        {
            Assert.That(value.Remove(value.Keys.ElementAt(value.Count / 2)), Is.True);
            Assert.That(Algorithms.MinMaxHeap.IsMinMaxHeap(value.RawValues, value.Comparer, default(TArity)), Is.True);
        }

        Assert.That(value, Is.Empty);
    }
    
    [Test]
    public void can_modify_item_from_middle_max_layer_to_min()
    {
        var sut = LargeDictionary(10_000);
        
        var i = (int)Math.Ceiling(Math.Pow(new TArity().Value, 3));
        Assert.That(Algorithms.MinMaxHeap.IsMinLevel<TArity>(i), Is.False);

        var item = sut.ElementAt(i);
        var minValue = sut.Min.Value - 1.0;
        sut[item.Key] = minValue;

        Assert.That(sut.Min, Is.EqualTo(new KeyValuePair<int,double>(item.Key, minValue)));
        Assert.That(Algorithms.MinMaxHeap.IsMinMaxHeap(sut.RawValues, sut.Comparer, default(TArity)), Is.True);
    }
    
    [Test]
    public void can_modify_item_from_middle_min_layer_to_max()
    {
        var sut = LargeDictionary(10_000);
        
        var i = (int)Math.Ceiling(Math.Pow(new TArity().Value, 4));
        Assert.That(Algorithms.MinMaxHeap.IsMinLevel<TArity>(i), Is.True);

        var item = sut.ElementAt(i);
        var maxValue = sut.Max.Value + 1.0;
        sut[item.Key] = maxValue;

        Assert.That(sut.Max, Is.EqualTo(new KeyValuePair<int,double>(item.Key, maxValue)));
        Assert.That(Algorithms.MinMaxHeap.IsMinMaxHeap(sut.Values.ToList(), Comparer<double>.Default, default(TArity)));
    }

    [Test, Ignore("time consuming")]
    public void random_mutations_leave_dictionary_in_correct_state()
    {
        var random = new Random(0);
        var sut = MinMaxHeapDictionary.CreateComparingValues<int, double>();
        
        for(var i = 0; i < 10_000; i++)
            sut[i] = random.NextDouble();
        
        Assert.That(Algorithms.MinMaxHeap.IsMinMaxHeap(sut.RawValues, sut.Comparer, default(TArity)));

        for (var i = 0; i < 50_000; i++)
        {
            var index = random.Next(0, sut.Count);
            var value = random.NextDouble();
            sut[index] = value;
            Assert.That(Algorithms.MinMaxHeap.IsMinMaxHeap(sut.RawValues, sut.Comparer, default(TArity)));
            Assert.That(sut.Min.Value, Is.EqualTo(sut.Min(x => x.Value)));
            Assert.That(sut.Max.Value, Is.EqualTo(sut.Max(x => x.Value)));
        }

        for (var i = 0; i < 50; i++)
        {
            var index = random.Next(0, sut.Count);
            var value = double.MinValue;
            sut[index] = value;
            Assert.That(Algorithms.MinMaxHeap.IsMinMaxHeap(sut.RawValues, sut.Comparer, default(TArity)));
            Assert.That(sut.Min.Value, Is.EqualTo(sut.Min(x => x.Value)));
            Assert.That(sut.Max.Value, Is.EqualTo(sut.Max(x => x.Value)));
        }
        
        for (var i = 0; i < 50; i++)
        {
            var index = random.Next(0, sut.Count);
            var value = double.MaxValue;
            sut[index] = value;
            Assert.That(Algorithms.MinMaxHeap.IsMinMaxHeap(sut.RawValues, sut.Comparer, default(TArity)));
            Assert.That(sut.Min.Value, Is.EqualTo(sut.Min(x => x.Value)));
            Assert.That(sut.Max.Value, Is.EqualTo(sut.Max(x => x.Value)));
        }
    }
    
    private static IEnumerable<TestCaseData> NonEmptyDictionaries()
    {
        yield return new TestCaseData(OneElementDictionary()) { TestName = "Single element dictionary" };
        yield return new TestCaseData(TwoElementDictionary()) { TestName = "Two element dictionary" };
        yield return new TestCaseData(ThreeElementDictionary()) { TestName = "Three element dictionary"};
        yield return new TestCaseData(LargeDictionary()) { TestName = "Large dictionary" };
    }

    private static MinMaxHeapDictionary<TArity, KeyValuePairByComparableValueComparer<int, double>, int, double> CreateDictionary(int n) => new(default, Enumerable.Range(0, n).ToDictionary(x => x, x => (double)x), default);
    private static MinMaxHeapDictionary<TArity, KeyValuePairByComparableValueComparer<int, double>, int, double> LargeDictionary(int n = 10_000) => CreateDictionary(n);
    private static MinMaxHeapDictionary<TArity, KeyValuePairByComparableValueComparer<int, double>, int, double> ThreeElementDictionary() => CreateDictionary(3);
    private static MinMaxHeapDictionary<TArity, KeyValuePairByComparableValueComparer<int, double>, int, double> TwoElementDictionary() => CreateDictionary(2);
    private static MinMaxHeapDictionary<TArity, KeyValuePairByComparableValueComparer<int, double>, int, double> OneElementDictionary() => CreateDictionary(1);

}