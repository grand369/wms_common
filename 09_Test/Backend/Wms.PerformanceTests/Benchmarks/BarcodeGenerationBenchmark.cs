using BenchmarkDotNet.Attributes;
using Wms.BarcodeLabel.Domain.Aggregates;
using Wms.BarcodeLabel.Domain.Enums;

namespace Wms.PerformanceTests.Benchmarks;

[MemoryDiagnoser]
public class BarcodeGenerationBenchmark
{
    [Params(100, 1000, 10000)]
    public int IterationCount { get; set; }

    private BarcodeRule _rule = null!;
    private PrintTask _printTask = null!;
    private LabelTemplate _template = null!;

    [GlobalSetup]
    public void Setup()
    {
        _rule = new BarcodeRule(
            Guid.NewGuid(),
            "PERF-BarcodeRule",
            BarcodeType.Material,
            BarcodeFormat.Code128,
            "{PREFIX}{DATE:yyMMdd}{SEQ:6}",
            prefix: "MAT",
            description: "Performance test rule");

        _printTask = new PrintTask(
            Guid.NewGuid(),
            $"PT-{DateTime.Now:yyyyMMddHHmmss}-001",
            "Outbound",
            Guid.NewGuid(),
            Guid.NewGuid(),
            "{\"sku\":\"MAT-001\",\"qty\":10}",
            printQuantity: 5,
            templateName: "StandardLabel",
            maxRetryCount: 3);

        _template = new LabelTemplate(
            Guid.NewGuid(),
            "StandardLabel",
            LabelTemplateType.Product,
            "{\"width\":100,\"height\":50,\"fields\":[{\"name\":\"SKU\",\"x\":10,\"y\":10}]}",
            industryStandard: "GS1-128");
    }

    [Benchmark]
    public string BarcodeRule_GenerateNextCode()
    {
        string result = string.Empty;
        for (int i = 0; i < IterationCount; i++)
        {
            result = _rule.GenerateNextCode();
        }

        return result;
    }

    [Benchmark]
    public void PrintTask_FullLifecycle()
    {
        for (int i = 0; i < IterationCount; i++)
        {
            var task = new PrintTask(
                Guid.NewGuid(),
                $"PT-{DateTime.Now:yyyyMMddHHmmss}-{i:D6}",
                "Outbound",
                Guid.NewGuid(),
                Guid.NewGuid(),
                "{\"sku\":\"MAT-001\",\"qty\":10}",
                printQuantity: 1);

            task.MarkPrinting();
            task.MarkCompleted();
        }
    }

    [Benchmark]
    public void PrintTask_MarkCompleted()
    {
        for (int i = 0; i < IterationCount; i++)
        {
            var task = new PrintTask(
                Guid.NewGuid(),
                $"PT-{DateTime.Now:yyyyMMddHHmmss}-{i:D6}",
                "Outbound",
                Guid.NewGuid(),
                Guid.NewGuid(),
                "{\"sku\":\"MAT-001\",\"qty\":10}",
                printQuantity: 1);

            task.MarkPrinting();
            task.MarkCompleted();
        }
    }

    [Benchmark]
    public void PrintTask_RetryCycle()
    {
        for (int i = 0; i < IterationCount; i++)
        {
            var task = new PrintTask(
                Guid.NewGuid(),
                $"PT-{DateTime.Now:yyyyMMddHHmmss}-{i:D6}",
                "Outbound",
                Guid.NewGuid(),
                Guid.NewGuid(),
                "{\"sku\":\"MAT-001\",\"qty\":10}",
                printQuantity: 1);

            task.MarkPrinting();
            task.MarkFailed("Printer offline");
            task.Retry();
            task.MarkPrinting();
            task.MarkCompleted();
        }
    }

    [Benchmark]
    public void LabelTemplate_UpdateContent()
    {
        for (int i = 0; i < IterationCount; i++)
        {
            _template.UpdateContent($"{{\"width\":100,\"height\":50,\"version\":{i}}}");
        }
    }

    [Benchmark]
    public void BarcodeRule_CreateAndGenerate()
    {
        string result = string.Empty;
        for (int i = 0; i < IterationCount; i++)
        {
            var rule = new BarcodeRule(
                Guid.NewGuid(),
                $"Rule-{i}",
                BarcodeType.Material,
                BarcodeFormat.Code128,
                "{DATE:yyMMdd}{SEQ:6}",
                prefix: "MAT");

            result = rule.GenerateNextCode();
        }

        result.ToString();
    }
}
