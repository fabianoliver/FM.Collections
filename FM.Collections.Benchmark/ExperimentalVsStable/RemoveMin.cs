using BenchmarkDotNet.Attributes;
using FM.Collections.Comparers;

namespace FM.Collections.Benchmark.HeapVsBcl;


[SimpleJob(iterationCount: 25)]
public class ExperimentalRemoveMin
{
    [Params(2_000_000)]
    public int HeapSize { get; set; }
    
    [Params(100)]
    public double PercentToRemove { get; set; }
    
    private MinMaxHeap<Arity.Two, ComparableComparer<double>, double> _minMaxHeap;
    private MinMaxHeap<Arity.Four, ComparableComparer<double>, double> _minMaxHeap4;

    private int _itemsToRemove;

    private double[] _data;
    
    [GlobalSetup]
    public void Setup()
    {
        _data = new double[HeapSize];
        var random = new Random(0);
        for (var i = 0; i < _data.Length; i++)
            _data[i] = random.NextDouble();
        _itemsToRemove = Math.Min(Math.Max(1, (int)(_data.Length * PercentToRemove)), _data.Length);
    }

    [IterationSetup]
    public void IterationSetup()
    {
        _minMaxHeap = new MinMaxHeap<Arity.Two, ComparableComparer<double>, double>(default, _data);
        _minMaxHeap4 = new MinMaxHeap<Arity.Four, ComparableComparer<double>, double>(default, _data);
    }
    
    [Benchmark(Baseline = true)]
    public void Stable()
    {
        for(var i = 0; i < _itemsToRemove; i++)
            _minMaxHeap.RemoveMin();
    }
    
    [Benchmark]
    public void StableArity4()
    {
        for(var i = 0; i < _itemsToRemove; i++)
            _minMaxHeap4.RemoveMin();
    }
}