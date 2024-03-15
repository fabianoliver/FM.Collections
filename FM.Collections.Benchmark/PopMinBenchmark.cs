using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.InProcess.NoEmit;
using FM.Collections.Comparers;
using Perfolizer.Mathematics.OutlierDetection;

namespace FM.Collections.Benchmark;

public class AntiVirusFriendlyConfig : ManualConfig
{
    public AntiVirusFriendlyConfig()
    {
        AddJob(Job.MediumRun
            .WithToolchain(InProcessNoEmitToolchain.Instance));
    }
}

[SimpleJob(iterationCount: 25)]
public class PopMinBenchmark
{
    private readonly Dictionary<int, double> _data = new();
    private PriorityQueue<KeyValuePair<int, double>, double> _priorityQueue = default!;
    private MinMaxHeapDictionary<Arity.Two, KeyValuePairByComparableValueComparer<int, double>, int, double> _minMaxHeapDictionary;
    private MinMaxHeap<Comparer, KeyValuePair<int, double>> _minMaxHeap;


    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(0);
        while(_data.Count < 1_000_000)
            _data[random.Next()] = random.NextDouble();
    }

    [IterationSetup]
    public void IterationSetup()
    {
        _priorityQueue = new PriorityQueue<KeyValuePair<int, double>, double>(_data.Select(x => (x, x.Value)));
        _minMaxHeapDictionary = MinMaxHeapDictionary.CreateComparingValues(_data);
        _minMaxHeap = new MinMaxHeap<Comparer, KeyValuePair<int, double>>(default, _data);
    }

    /*[Benchmark]
    public void BenchmarkMinMaxHeapDictionary()
    {
        for(var i = 0; i < 10_000; i++)
            _minMaxHeapDictionary.Remove(_minMaxHeapDictionary.Min.Key);
    }*/
    
    [Benchmark]
    public void RemoveMinCustomHeap()
    {
        for (var i = 0; i < 10_000; i++)
            _minMaxHeap.RemoveMin();
    }


    
    /*[Benchmark]
    public void RemoveMinCustomDictionary()
    {
        for (var i = 0; i < 10_000; i++)
            _minMaxHeapDictionary.RemoveMin();
    }*/
    
    [Benchmark]
    public void RemoveMinBcl()
    {
        for(var i = 0; i < 10_000; i++)
            _priorityQueue.Dequeue();
    }

  
    
    private readonly struct Comparer : IComparer<KeyValuePair<int, double>>
    {
        public int Compare(KeyValuePair<int, double> x, KeyValuePair<int, double> y) => x.Value.CompareTo(y.Value);
    }

}