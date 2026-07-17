using BenchmarkDotNet.Running;
using Wms.PerformanceTests.Benchmarks;

namespace Wms.PerformanceTests;

class Program
{
    static void Main(string[] args)
    {
        BenchmarkRunner.Run<InventoryBalanceBenchmark>(args: args);
        BenchmarkRunner.Run<BarcodeGenerationBenchmark>(args: args);
        BenchmarkRunner.Run<NotificationBatchBenchmark>(args: args);
        BenchmarkRunner.Run<ApprovalFlowBenchmark>(args: args);
        BenchmarkRunner.Run<InventoryQueryBenchmark>(args: args);
    }
}
