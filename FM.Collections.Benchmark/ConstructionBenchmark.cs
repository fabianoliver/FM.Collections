using BenchmarkDotNet.Attributes;
using Perfolizer.Mathematics.OutlierDetection;

namespace FM.Collections.Benchmark;

[SimpleJob(iterationCount: 15)]
[Outliers(OutlierMode.DontRemove)]
public class ConstructionBenchmark
{
    private readonly Dictionary<int, double> _data = new();
    
    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(0);
        while(_data.Count < 10_000)
            _data[random.Next()] = random.NextDouble();
    }

    [Benchmark]
    public void BenchmarkMinMaxHeapDictionary()
    {
        var sut = MinMaxHeapDictionary.CreateComparingValues(_data);
    }
    
    [Benchmark]
    public void BenchmarkDotNetPriorityQueue()
    {
        var sut = new PriorityQueue<KeyValuePair<int, double>, double>(_data.Select(x => (x, x.Value)));
    }
}