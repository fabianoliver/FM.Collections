// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Running;
using FM.Collections.Benchmark.HeapVsBcl;

 BenchmarkRunner.Run(new Type[]
{
    typeof(AddMin),
    typeof(RemoveMin),
    typeof(ConstructHeap)
}, DefaultConfig.Instance
    .WithOption(ConfigOptions.JoinSummary, true)
    .AddExporter(CsvExporter.Default)
    .AddExporter(CsvMeasurementsExporter.Default)
    );


/*
BenchmarkRunner.Run(new Type[]
    {
        typeof(ExperimentalRemoveMin),
        typeof(ExperimentalAddMin),
    }, DefaultConfig.Instance
        .WithOption(ConfigOptions.JoinSummary, true)
        .AddExporter(CsvExporter.Default)
        .AddExporter(CsvMeasurementsExporter.Default)
);
*/