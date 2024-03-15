using BenchmarkDotNet.Attributes;
using FM.Collections.Comparers;

namespace FM.Collections.Benchmark.HeapVsBcl;


[SimpleJob(iterationCount: 25)]
public class ConstructHeap
{
    [Params(100, 10_000, 10_000_000)]
    public int HeapSize { get; set; }

    private double[] _data;
    
    [GlobalSetup]
    public void Setup()
    {
        _data = new double[HeapSize];
        var random = new Random(0);
        for (var i = 0; i < _data.Length; i++)
            _data[i] = random.NextDouble();
    }
    
    
    [Benchmark]
    public void ConstructFmCollectionsHeap()
    {
        new MinMaxHeap<Arity.Two, ComparableComparer<double>, double>(default, _data);
    }
    
    [Benchmark]
    public void ConstructBclPriorityQueue()
    {
        new PriorityQueue<double, double>(_data.Select(x => (x, x)));
    }
}