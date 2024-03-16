using BenchmarkDotNet.Attributes;
using FM.Collections.Comparers;

namespace FM.Collections.Benchmark.HeapVsBcl;


[SimpleJob(iterationCount: 25)]
public class ExperimentalAddMin
{
    [Params(0)]
    public int InitialHeapSize { get; set; }
    
    [Params(10_000_000)]
    public int ItemsToAdd { get; set; }

    private MinMaxHeap<Arity.Two, ComparableComparer<double>, double> _minMaxHeap;
    private MinMaxHeap<Arity.Four, ComparableComparer<double>, double> _minMaxHeap4;
    private double[] _data;
    
    [GlobalSetup]
    public void Setup()
    {
        _data = new double[InitialHeapSize];
        var random = new Random(0);
        for (var i = 0; i < _data.Length; i++)
            _data[i] = random.NextDouble();
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
        for(var i = 0; i < ItemsToAdd; i++)
            _minMaxHeap.Add(-(double)i);
    }
    
    [Benchmark]
    public void StableArity4()
    {
        for(var i = 0; i < ItemsToAdd; i++)
            _minMaxHeap4.Add(-(double)i);
    }
}