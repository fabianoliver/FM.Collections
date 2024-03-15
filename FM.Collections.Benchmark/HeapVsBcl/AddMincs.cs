using BenchmarkDotNet.Attributes;
using FM.Collections.Comparers;

namespace FM.Collections.Benchmark.HeapVsBcl;


[SimpleJob(iterationCount: 25)]
public class AddMin
{
    [Params(0, 100, 10_000, 10_000_000)]
    public int InitialHeapSize { get; set; }
    
    [Params(100, 10_000, 10_000_000)]
    public int ItemsToAdd { get; set; }

    private PriorityQueue<double, double> _priorityQueue = default!;
    private MinMaxHeap<Arity.Two, ComparableComparer<double>, double> _minMaxHeap;
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
        _priorityQueue = new PriorityQueue<double, double>(_data.Select(x => (x,x)));
        _minMaxHeap = new MinMaxHeap<Arity.Two, ComparableComparer<double>, double>(default, _data);
    }
    
    [Benchmark]
    public void AddMinFmCollectionsHeap()
    {
        for(var i = 0; i < ItemsToAdd; i++)
            _minMaxHeap.Add(-(double)i);
    }
    
    [Benchmark]
    public void AddMinBclPriorityQueue()
    {
        for (var i = 0; i < ItemsToAdd; i++)
        {
            var value = -(double)i;
            _priorityQueue.Enqueue(value, value);
        }
    }
}