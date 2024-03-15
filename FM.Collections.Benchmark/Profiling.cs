using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnostics.dotTrace;

namespace FM.Collections.Benchmark;



[DotTraceDiagnoser]
[SimpleJob, InProcess]
public class Profiling
{
    private readonly Dictionary<int, double> _data = new();
    private MinMaxHeap<Comparer, KeyValuePair<int, double>> _minMaxHeap;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(0);
        while(_data.Count < 10_000_000)
            _data[random.Next()] = random.NextDouble();
    }

    [IterationSetup]
    public void IterationSetup()
    {
        _minMaxHeap = new MinMaxHeap<Comparer, KeyValuePair<int, double>>(default);
        foreach(var item in _data)
            _minMaxHeap.Add(item);
    }

    
    [Benchmark]
    public void BenchmarkMinMaxHeap()
    {
        for (var i = 0; i < 100_000; i++)
            _minMaxHeap.RemoveMin();
    }
    
    private readonly struct Comparer : IComparer<KeyValuePair<int, double>>
    {
        public int Compare(KeyValuePair<int, double> x, KeyValuePair<int, double> y) => x.Value.CompareTo(y.Value);
    }
}