using BenchmarkDotNet.Attributes;
using FM.Collections.Comparers;

namespace FM.Collections.Benchmark.HeapVsBcl;


[SimpleJob(iterationCount: 25)]
public class Debug
{
    [Params(100, 10_000, 10_000_000)]
    public int InitialHeapSize { get; set; }

    private PriorityQueue<double, double> _priorityQueue = default!;
    private MinMaxHeap<ComparableComparer<double>, double> _minMaxHeap;

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
        _minMaxHeap = new MinMaxHeap<ComparableComparer<double>, double>(default, _data);
    }

    
    [Benchmark]
    public void RemoveMinCustomHeap()
    {
        for(var i = 0; i < InitialHeapSize; i++)
            _minMaxHeap.RemoveMin();
    }
    
    [Benchmark]
    public void RemoveMinBcl()
    {
        for(var i = 0; i < InitialHeapSize; i++)
            _minMaxHeap.RemoveMin();
    }
    
      
    [Benchmark]
    public void AddNewMinCustomHeap()
    {
        for (var i = 0; i < 10_000; i++)
        {
            var value = -(double)i;
            _minMaxHeap.Add(value);
        }
    }
    
    [Benchmark]
    public void AddNewMinBcl()
    {
        for (var i = 0; i < 10_000; i++)
        {
            var value = -(double)i;
            _priorityQueue.Enqueue(value,value);
        }
    }

    [Benchmark]
    public void InitializeCustomHeap()
    {
        for (var i = 0; i < 10_000; i++)
            new MinMaxHeap<ComparableComparer<double>, double>(default, _data);
    }
    
    [Benchmark]
    public void InitializeBcl()
    {
        for (var i = 0; i < 10_000; i++)
            new PriorityQueue<double, double>(_data.Select(x => (x, x)));
    }
}