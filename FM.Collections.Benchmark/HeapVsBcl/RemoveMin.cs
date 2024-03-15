using BenchmarkDotNet.Attributes;
using FM.Collections.Comparers;

namespace FM.Collections.Benchmark.HeapVsBcl;


[SimpleJob(iterationCount: 25)]
public class RemoveMin
{
    [Params(100, 10_000, 10_000_000)]
    public int HeapSize { get; set; }
    
    [Params(0.1, 1, 10, 100)]
    public double PercentToRemove { get; set; }

    private PriorityQueue<double, double> _priorityQueue = default!;
    private MinMaxHeap<Arity.Two, ComparableComparer<double>, double> _minMaxHeap;

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
        _priorityQueue = new PriorityQueue<double, double>(_data.Select(x => (x,x)));
        _minMaxHeap = new MinMaxHeap<Arity.Two, ComparableComparer<double>, double>(default, _data);
    }
    
    [Benchmark]
    public void RemoveMinFmCollectionsHeap()
    {
        for(var i = 0; i < _itemsToRemove; i++)
            _minMaxHeap.RemoveMin();
    }
    
    [Benchmark]
    public void RemoveMinBclPriorityQueue()
    {
        for(var i = 0; i < _itemsToRemove; i++)
            _priorityQueue.Dequeue();
    }
}